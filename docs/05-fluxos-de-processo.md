# 05 — Fluxos de Processo

> Fluxos ponta a ponta com os sistemas envolvidos. Nomes de endpoints e eventos seguem `06-arquitetura-tecnica.md`.

---

## 1. Encomenda: do montador ao sinal pago

```mermaid
sequenceDiagram
    actor Ana
    participant PWA
    participant API
    participant Cap as Capacidade
    participant Pix as Gateway Pix
    participant Bus as Message Bus
    participant WA as WhatsApp

    Ana->>PWA: Monta bolo (opções)
    PWA->>API: POST /orcamentos/simular
    API-->>PWA: Preço + carga de produção
    Ana->>PWA: Escolhe data
    PWA->>API: GET /capacidade?de=..&ate=..&carga=6
    API->>Cap: Dias/turnos com Disponivel() >= carga
    Cap-->>API: Lista de slots
    API-->>PWA: Calendário (dias lotados desabilitados)
    Ana->>PWA: Confirma e informa telefone
    PWA->>API: POST /encomendas
    API->>API: Encomenda.Criar() → RN-10, RN-03
    API->>API: Persistir → publicar EncomendaCriadaEvent
    API->>Pix: Criar cobrança do sinal (50 %)
    Pix-->>API: QR + txid (expira 30 min)
    API-->>PWA: Encomenda + QR
    API->>Bus: EncomendaCriadaEvent
    Bus->>WA: Envia orçamento + link
    Ana->>Pix: Paga
    Pix->>API: Webhook pagamento
    API->>API: Encomenda.RegistrarSinal() → Confirmada
    API->>Cap: Reservar(carga) (via IImpactoCapacidadeStrategy)
    API->>Bus: EncomendaConfirmadaEvent
    Bus->>WA: "Confirmado! Entrega sáb 15h"
    Bus-->>PWA: SignalR → KDS recebe nova encomenda
```

## 2. Produção no KDS

```mermaid
flowchart TD
    A[EncomendaConfirmadaEvent] --> B[Aparece no KDS<br/>coluna 'A fazer', ordenado por data/hora]
    B --> C[Confeiteiro toca 'Iniciar']
    C --> D[Status EmProducao<br/>etapa 1: massa]
    D --> E[Checklist da etapa<br/>insumos listados]
    E --> F{Última etapa?}
    F -->|Não| G[Avança etapa<br/>foto opcional]
    G --> H{Etapa = decorado?}
    H -->|Sim| I[Foto obrigatória<br/>→ cliente recebe]
    H -->|Não| E
    I --> E
    F -->|Sim| J[Marcar PRONTA]
    J --> K[EncomendaProntaEvent]
    K --> L[Cliente recebe QR de retirada<br/>ou entregador é atribuído]
    K --> M[Balcão vê em 'Retiradas do dia']
```

## 3. Venda de pronta entrega com estoque por lote

```mermaid
sequenceDiagram
    participant PDV
    participant API
    participant Est as Estoque
    participant Fid as Fidelidade

    PDV->>API: POST /pedidos {itens, telefoneCliente}
    API->>Est: Verificar disponibilidade por produto (soma de lotes válidos)
    Est-->>API: OK
    API->>API: Pedido.Criar(); aplicar IDescontoStrategy
    API-->>PDV: Pedido AguardandoPagamento + QR Pix
    Note over PDV: Cliente paga
    API->>API: Pedido.ConfirmarPagamento() → Pago
    API->>Est: Baixar FEFO (lote mais próximo do vencimento primeiro)
    API->>Fid: Creditar selos (RN-40)
    Fid-->>API: ClienteSubiuDeNivelEvent? (se aplicável)
    API-->>PDV: Recibo + saldo de selos
```

## 4. Cancelamento e estorno

```mermaid
flowchart TD
    A[Cliente ou atendente solicita cancelamento] --> B{EncomendaCancelavelSpecification}
    B -->|Não| C[Erro 409: não cancelável<br/>ex.: já entregue]
    B -->|Sim| D[IPoliticaEstornoStrategy<br/>por antecedência]
    D --> E{Horas até entrega}
    E -->|> 72h| F[Estorno 100 %]
    E -->|24h a 72h| G[Estorno 50 %]
    E -->|< 24h| H[Sem estorno]
    F & G & H --> I[Encomenda.Cancelar motivo]
    I --> J[Persistir]
    J --> K[EncomendaCanceladaEvent]
    K --> L[Capacidade.Liberar carga]
    K --> M[Gateway: estorno Pix]
    K --> N[Estoque: liberar reserva de insumos]
    K --> O[WhatsApp: confirmação + valor estornado]
```

## 5. Ciclo de vida do lote e "Últimas horas"

```mermaid
sequenceDiagram
    participant Job as Job de validade (a cada 15 min)
    participant API
    participant Cat as Catálogo
    participant Push

    Job->>API: Lotes com validade - agora <= 4h e status Disponivel
    API->>API: Lote → UltimasHoras
    API->>Cat: Preço dinâmico: -20 % agora, -40 % faltando 1h
    API->>Push: "6 bolos de pote com 20 % off até 18h" (clientes num raio de 2 km, opt-in)
    Job->>API: Lotes vencidos
    API->>API: Lote → Vencido; registra perda com custo
    API-->>Job: LoteVencidoEvent → relatório de desperdício
```

## 6. Assinatura mensal

```mermaid
flowchart LR
    A[Job diário] --> B{Assinaturas com<br/>ProximaEntrega = hoje + 2}
    B --> C[Assinatura.GerarPedidoDoCiclo]
    C --> D[Cobrança recorrente cartão]
    D -->|Pago| E[Pedido Pago → reserva capacidade<br/>de pronta entrega do dia]
    D -->|Falhou| F[Tentativa 2 e 3 em 24h]
    F -->|Falhou 3x| G[Assinatura Pausada + aviso]
    E --> H[Entrega na janela escolhida]
```

## 7. Sincronização offline do PDV

```mermaid
sequenceDiagram
    participant PDV as PDV (offline)
    participant SW as Service Worker + IndexedDB
    participant API

    PDV->>SW: Vende (dinheiro/cartão off) → pedido com ClientId UUID
    SW->>SW: Enfileira operação (outbox local)
    Note over PDV,SW: Internet volta
    SW->>API: POST /pedidos (Idempotency-Key = ClientId)
    alt Já existe
        API-->>SW: 200 (idempotente)
    else Novo
        API-->>SW: 201
    end
    SW->>API: Próxima operação da fila
    API-->>PDV: Estoque e caixa atualizados (SignalR)
```

## 8. Capacidade: impacto por ação (reaproveitando o conceito de mapa de ocupação da spec)

```mermaid
flowchart LR
    subgraph Ações
        A1[Confirmar]
        A2[Cancelar]
        A3[Reagendar]
        A4[Alterar itens]
    end
    subgraph Strategies[IImpactoCapacidadeStrategy]
        S1[ConfirmarStrategy: +carga no dia/turno]
        S2[CancelarStrategy: -carga]
        S3[ReagendarStrategy: -carga antiga, +carga nova]
        S4[AlterarStrategy: delta de carga]
    end
    A1 --> S1
    A2 --> S2
    A3 --> S3
    A4 --> S4
    S1 & S2 & S3 & S4 --> R[Resolver aplica impactos<br/>na CapacidadeDia]
    R --> V{Capacidade excedida?}
    V -->|Sim, sem aprovação| X[Result.Fail Conflict<br/>+ sugestões de datas]
    V -->|Sim, gerente aprova overbooking ≤ 10 %| OK
    V -->|Não| OK[Persistir + CapacidadeAlteradaEvent]
```
