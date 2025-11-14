# ⚡ Comandos Rápidos

## 🚀 Iniciar Dashboard

### Opção 1: Script Automático (Recomendado)
```bash
./check-and-start.sh
```

### Opção 2: Docker Desenvolvimento
```bash
docker-compose up dev
# Acesse: http://localhost:5173
```

### Opção 3: Docker Produção
```bash
docker-compose up prod
# Acesse: http://localhost:8080
```

### Opção 4: Local (sem Docker)
```bash
npm install
npm run dev
# Acesse: http://localhost:5173
```

## 🛑 Parar

```bash
# Pressione Ctrl+C
# Ou:
docker-compose down
```

## 🔧 Outros Comandos

### Rebuild completo
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up
```

### Limpar tudo
```bash
docker-compose down
docker system prune -f
rm -rf node_modules
```

### Ver logs
```bash
docker-compose logs -f
```

### Build de produção local
```bash
npm run build
npm run preview
```

## 📝 Editar Dados Mockados

```bash
# Edite os arquivos:
nano public/mock-api/dashboard-data.json
nano public/mock-api/transactions.json

# As mudanças aparecem automaticamente (hot-reload)
```

## 🐛 Troubleshooting

### Porta ocupada
```bash
# Edite docker-compose.yml
# Mude "5173:5173" para "3000:5173"
```

### Erro de permissão
```bash
chmod +x *.sh
```

### Container não inicia
```bash
docker-compose down
docker system prune -f
docker-compose up --build
```

---

**Dica**: Use `./check-and-start.sh` para verificar tudo automaticamente! ✨
