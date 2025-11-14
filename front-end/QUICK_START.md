# 🚀 Guia Rápido - Expense Tracking Dashboard

## Opção 1: Usando o Script Automático (Recomendado)

```bash
cd front-end
./start.sh
```

Escolha:
- **1** para modo desenvolvimento (com hot-reload)
- **2** para modo produção (otimizado)

## Opção 2: Comandos Docker Diretos

### Desenvolvimento (porta 5173)
```bash
cd front-end
docker-compose up dev
```

### Produção (porta 8080)
```bash
cd front-end
docker-compose up prod
```

## Opção 3: Sem Docker (Local)

```bash
cd front-end
npm install
npm run dev
```

## 📱 Acessando o Dashboard

- **Desenvolvimento**: http://localhost:5173
- **Produção**: http://localhost:8080

## 🎯 O que você verá

1. **Gráfico Termômetro**: Mostra consumo do orçamento (R$ 283.212,30 consumido de R$ 389.100,00)
2. **Filtros**: Selecione projeto, pesquise, filtre por data, status e categoria
3. **Tabela de Transações**: 20 transações mockadas com paginação
4. **Status Coloridos**:
   - 🔵 Em Validação
   - 🔴 Pendente
   - 🟢 Validado
   - 🟠 Revisar

## 🛑 Parando o Container

```bash
# Pressione Ctrl+C no terminal
# Ou em outro terminal:
docker-compose down
```

## 🔧 Problemas Comuns

### Porta já em uso
```bash
# Mudar a porta no docker-compose.yml
# De: "5173:5173"
# Para: "3000:5173"
```

### Container não inicia
```bash
# Limpar containers antigos
docker-compose down
docker system prune -f

# Tentar novamente
docker-compose up dev
```

## 📊 Dados Mockados

Os dados estão em:
- `public/mock-api/dashboard-data.json`
- `public/mock-api/transactions.json`

Você pode editar esses arquivos para testar diferentes cenários!
