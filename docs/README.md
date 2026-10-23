# DoceFlow — Especificação de Negócio e Arquitetura

Sistema omnicanal para doceria com **doces prontos** e **sob encomenda**, entregue como um único PWA responsivo (web, mobile e desktop), com back-end .NET 8 seguindo a [especificação técnica base](../../ESPECIFICACOES-TECNICAS.md).

## Documentos

| #   | Documento                                                              | Conteúdo                                                                 |
| --- | ---------------------------------------------------------------------- | ------------------------------------------------------------------------ |
| 01  | [Visão e modelo de negócio](01-visao-e-modelo-de-negocio.md)           | Problema, proposta de valor, linhas de negócio, canvas, KPIs             |
| 02  | [Personas e jornadas](02-personas-e-jornadas.md)                       | 7 personas, jornadas (cliente, balcão, cozinha, entrega), RBAC           |
| 03  | [Domínio e regras de negócio](03-dominio-e-regras-de-negocio.md)       | Contextos, agregados, máquinas de estado, 40+ regras, eventos, specs     |
| 04  | [Catálogo de features](04-features.md)                                 | ~55 features priorizadas (MoSCoW), 15 apostas disruptivas 🚀             |
| 05  | [Fluxos de processo](05-fluxos-de-processo.md)                         | Sequências ponta a ponta: encomenda, KDS, PDV, cancelamento, offline     |
| 06  | [Arquitetura técnica](06-arquitetura-tecnica.md)                       | Aderência à spec, estrutura, API, PWA, tempo real, segurança             |
| 07  | [Modelo de dados](07-modelo-de-dados.md)                               | ER completo, índices, concorrência                                       |
| 08  | [Roadmap, métricas e riscos](08-roadmap-metricas-e-riscos.md)          | Gantt, escopo do MVP, métricas, riscos, critérios de pronto              |
| 09  | [Checklist de desenvolvimento do MVP](09-checklist-desenvolvimento-mvp.md) | 10 etapas, critérios de saída, checklist por feature            |
| ADR | [Decisões arquiteturais](adr/)                                         | 001–008                                                                  |

## Os 5 diferenciais em uma frase

1. **Calendário de capacidade**: o cliente só escolhe datas que a cozinha realmente consegue entregar.
2. **Montador de bolo com preço em tempo real**: orçamento sem ida e volta no WhatsApp.
3. **Timeline com fotos da produção**: ninguém mais liga para perguntar "já está pronto?".
4. **Últimas horas**: lote perto da validade vira oportunidade, não perda.
5. **Ficha técnica → margem real**: todo doce sabe quanto custa e quanto lucra.

## Como ler os diagramas

Todos os diagramas são Mermaid. GitHub, GitLab, VS Code (extensão *Markdown Preview Mermaid Support*) e Obsidian renderizam nativamente.
