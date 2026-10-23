# 08 — Roadmap, Métricas e Riscos

## 1. Roadmap

```mermaid
gantt
    title Roadmap DoceFlow (estimativa em semanas)
    dateFormat  YYYY-MM-DD
    axisFormat  %b/%y

    section Fundação
    Setup repo, CI, Docker, esqueleto Clean Arch       :f1, 2026-11-02, 2w
    Auth (OTP cliente, JWT equipe), PWA shell          :f2, after f1, 2w

    section MVP — Vender e produzir
    Catálogo, opções, fichas técnicas                  :m1, after f2, 2w
    Capacidade de produção + calendário                :m2, after m1, 2w
    Encomendas: montador, orçamento, sinal Pix         :m3, after m2, 3w
    KDS: fila, etapas, fotos, plano do dia             :m4, after m3, 2w
    PDV offline + lotes + Pix dinâmico                 :m5, after m3, 3w
    Notificações WhatsApp/push, timeline pública       :m6, after m4, 1w
    Fidelidade (selos, níveis), NPS                    :m7, after m6, 1w
    Piloto na loja                                     :milestone, after m7, 0d

    section Fase 2 — Crescer
    Últimas horas + preço dinâmico                     :s1, after m7, 2w
    Assinatura Clube do Brigadeiro                     :s2, after s1, 2w
    Entregador PWA + janelas + rotas                   :s3, after s1, 3w
    B2B (crédito, fatura mensal, repetir pedido)       :s4, after s2, 2w
    Painel de margem real + campanhas                  :s5, after s4, 2w

    section Fase 3 — Diferenciar
    Assistente de orçamento IA (WhatsApp)              :c1, after s5, 3w
    Calculadora de festa com IA                        :c2, after c1, 1w
    Vaquinha do bolo, cartão-presente                  :c3, after c2, 2w
    Previsão de demanda sazonal                        :c4, after c3, 3w
    Quiosque e balança                                 :c5, after c4, 2w
```

### Escopo do MVP (piloto)

| Inclui                                                                 | Não inclui                                  |
| ---------------------------------------------------------------------- | ------------------------------------------- |
| Vitrine, montador de bolo, calendário de capacidade, sinal Pix         | Delivery próprio (somente retirada no MVP)  |
| KDS com etapas e fotos; plano do dia; lotes com validade               | Assinatura, B2B, IA                         |
| PDV offline com Pix dinâmico e retirada por QR                         | Multi-loja, NFC-e                           |
| WhatsApp + push; timeline pública; fidelidade básica; NPS              | Previsão de demanda                         |

## 2. Métricas de produto

| Área          | Métrica                                          | Fonte                              | Alvo pós-piloto |
| ------------- | ------------------------------------------------ | ---------------------------------- | --------------- |
| Conversão     | Simulações de orçamento → encomendas confirmadas | `EncomendaCriada` / `Confirmada`   | > 35 %          |
| Capacidade    | Ocupação média / dias com overbooking            | `tblCapacidadeDias`                | 85 % / < 5 %    |
| Pontualidade  | Encomendas prontas antes da hora de entrega      | `EncomendaProntaEvent` vs `DataEntrega` | > 98 %     |
| Desperdício   | Custo de lotes vencidos / custo produzido        | `LoteVencidoEvent`                 | < 5 %           |
| Últimas horas | % de lotes em "últimas horas" vendidos           | `tblLotes`                         | > 70 %          |
| Fidelidade    | Clientes com ≥ 2 pedidos em 90 dias              | `tblPedidos`                       | > 45 %          |
| Operação      | Tempo médio de venda no PDV                      | Telemetria front                   | < 40 s          |
| Offline       | Operações sincronizadas sem conflito             | `IdempotencyMiddleware`            | > 99 %          |
| Encantamento  | NPS; % com foto na avaliação                     | `tblAvaliacoes`                    | > 70; > 30 %    |
| Margem        | Produtos abaixo da margem alvo                   | `tblFichasTecnicas`                | 0               |

## 3. Riscos e mitigações

| Risco                                                    | Prob. | Impacto | Mitigação                                                                                     |
| -------------------------------------------------------- | :---: | :-----: | --------------------------------------------------------------------------------------------- |
| Confeiteiro não atualiza etapas no KDS                   | Alta  | Alto    | UX de 1 toque, modo mãos ocupadas, fotos trazem valor visível (cliente elogia)                |
| Carga de produção mal calibrada (capacidade irreal)      | Alta  | Alto    | Começar conservador, relatório "estimado vs real" por produto, ajuste semanal                  |
| Webhook Pix atrasa/falha                                 | Média | Alto    | Polling de fallback a cada 1 min para cobranças pendentes; idempotência por `txid`             |
| Conflito de estoque no PDV offline                       | Média | Médio   | Alerta explícito; vitrine web mostra "sujeito a confirmação" quando PDV está offline há > 10 min |
| Template WhatsApp reprovado / API indisponível           | Média | Médio   | Cadeia de fallback push → e-mail → SMS; templates genéricos aprovados cedo                     |
| Equipe pequena sobrecarregada com notificações           | Média | Baixo   | Agrupamento de alertas; apenas eventos acionáveis no KDS                                       |
| Cliente aceita orçamento sem ler restrições/alergênicos  | Baixa | Alto    | Confirmação explícita (RN-05) + registro de consentimento                                      |
| Fotos de produção vazam dados de outros clientes         | Baixa | Médio   | Foto vinculada a `EncomendaId`, URL assinada e expirável                                        |

## 4. Critérios de pronto do MVP

- [ ] Encomenda do montador ao "pronta" sem intervenção manual fora do KDS
- [ ] Data lotada não aparece como disponível em nenhum canal
- [ ] PDV vende 50 pedidos offline e sincroniza sem duplicar
- [ ] Webhook Pix duplicado não gera pagamento duplicado
- [ ] Lote que vence entra em "últimas horas" e some da vitrine ao vencer
- [ ] Cobertura de testes de domínio ≥ 90 % (entidades, specs, strategies)
- [ ] Lighthouse PWA ≥ 90 em mobile e desktop
- [ ] ADRs 001–007 registradas
