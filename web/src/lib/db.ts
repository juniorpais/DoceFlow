import Dexie, { type EntityTable } from 'dexie'

export interface OutboxItem {
  id?: number
  rota: string
  metodo: 'POST' | 'PUT' | 'PATCH' | 'DELETE'
  corpo: unknown
  chave: string
  criadoEm: number
  tentativas: number
}

class DoceFlowDb extends Dexie {
  outbox!: EntityTable<OutboxItem, 'id'>

  constructor() {
    super('doceflow')
    this.version(1).stores({
      outbox: '++id, chave, criadoEm',
    })
  }
}

export const db = new DoceFlowDb()
