# 🚀 Como Rodar o Dashboard de Prestação de Contas

## Início Rápido (1 comando)

```bash
cd front-end
./check-and-start.sh
```

Siga as instruções na tela e escolha:
- **Opção 1**: Docker Desenvolvimento (recomendado)
- **Opção 2**: Docker Produção
- **Opção 3**: Local sem Docker

## Acesso

Abra seu navegador em:
- http://localhost:5173 (desenvolvimento)
- http://localhost:8080 (produção)

## O que está implementado

✅ **Front-End Completo com Dados Mockados**
- Gráfico termômetro de orçamento
- Filtros por projeto, data, status e categoria
- Tabela de transações com paginação
- Tema escuro responsivo
- 20 transações de exemplo

✅ **Docker Configurado**
- Dockerfile para produção (Nginx)
- Dockerfile.dev para desenvolvimento
- docker-compose.yml pronto

✅ **Scripts Automatizados**
- `check-and-start.sh` - Verifica ambiente e inicia
- `start.sh` - Inicia direto
- `test-local.sh` - Testa sem Docker

## Estrutura de Arquivos

```
front-end/
├── public/mock-api/          # 📊 Dados mockados (EDITE AQUI)
│   ├── dashboard-data.json   # Projetos e métricas
│   └── transactions.json     # 20 transações
├── src/
│   ├── components/dashboard/ # Componentes do dashboard
│   ├── services/             # API mockada
│   └── views/                # Página principal
├── Dockerfile                # Build de produção
├── docker-compose.yml        # Orquestração
└── check-and-start.sh        # 🚀 Script de início
```

## Editando os Dados Mockados

Para testar com dados diferentes, edite:
- `front-end/public/mock-api/dashboard-data.json`
- `front-end/public/mock-api/transactions.json`

As mudanças aparecem automaticamente (hot-reload).

## Parando o Dashboard

Pressione `Ctrl+C` no terminal onde está rodando.

## Problemas?

### Porta já em uso
Edite `docker-compose.yml` e mude a porta:
```yaml
ports:
  - "3000:5173"  # Mudou de 5173 para 3000
```

### Docker não instalado
- **Windows/Mac**: https://www.docker.com/products/docker-desktop
- **Linux**: `sudo apt install docker.io docker-compose`

### Rodar sem Docker
```bash
cd front-end
npm install
npm run dev
```

## Documentação Completa

- `front-end/INICIO_RAPIDO.md` - Guia rápido
- `DASHBOARD_SETUP.md` - Guia completo
- `front-end/README.md` - Documentação técnica

---

**Dúvidas? Todos os comandos estão documentados nos arquivos acima! 📚**
