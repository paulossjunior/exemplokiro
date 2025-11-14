# 📊 Expense Tracking Dashboard - Guia Completo de Configuração

## 🎯 Visão Geral

Dashboard de prestação de contas com dados mockados, pronto para demonstração ao usuário.

**Funcionalidades implementadas:**
- ✅ Gráfico termômetro de consumo de orçamento
- ✅ Filtros por projeto, pesquisa, data, status e categoria
- ✅ Tabela de transações com paginação
- ✅ Badges de status coloridos
- ✅ Tema escuro (zinc) responsivo
- ✅ Acessibilidade WCAG 2.0 Level AA
- ✅ Dados mockados para demonstração

## 🚀 Início Rápido

### Opção 1: Docker (Recomendado para Demonstração)

```bash
cd front-end
./start.sh
```

Escolha a opção desejada:
- **1** - Desenvolvimento (hot-reload, porta 5173)
- **2** - Produção (otimizado, porta 8080)

### Opção 2: Local (Sem Docker)

```bash
cd front-end
./test-local.sh
```

Ou manualmente:
```bash
cd front-end
npm install
npm run dev
```

## 📱 Acessando o Dashboard

Após iniciar, acesse:
- **Desenvolvimento**: http://localhost:5173
- **Produção**: http://localhost:8080

## 🎨 O que Você Verá

### 1. Gráfico Termômetro
- **Consumido**: R$ 283.212,30
- **Restante**: R$ 105.887,70
- **Rendimento**: R$ 4.752,25
- **Total**: R$ 389.100,00
- **Progresso**: 72,8%

### 2. Filtros Disponíveis
- **Projetos**: 4 projetos mockados (Conecta Fapes, Sistema de Gestão, etc.)
- **Pesquisa**: Busca por método de pagamento ou CNPJ
- **Data**: Seletor de data
- **Status**: Em Validação, Pendente, Validado, Revisar
- **Categoria**: Equipamentos, Serviços, Material de Consumo, Passagens, Bolsas

### 3. Tabela de Transações
- 20 transações mockadas
- Paginação (10 por página)
- Colunas: Pagamento, Valor, Data, CNPJ, Status
- Hover effects e transições suaves

## 📊 Dados Mockados

Os dados estão localizados em:

```
front-end/public/mock-api/
├── dashboard-data.json    # Projetos, categorias, métricas
└── transactions.json      # 20 transações de exemplo
```

**Você pode editar esses arquivos para testar diferentes cenários!**

## 🏗️ Estrutura do Projeto

```
front-end/
├── public/
│   └── mock-api/              # 📊 Dados mockados
├── src/
│   ├── components/
│   │   ├── common/
│   │   │   └── StatusBadge.vue
│   │   └── dashboard/
│   │       ├── ThermometerChart.vue
│   │       ├── DashboardFilters.vue
│   │       └── TransactionTable.vue
│   ├── composables/
│   │   └── useDashboard.ts    # 🎯 Lógica principal
│   ├── services/
│   │   ├── dashboardService.ts      # API real (não usado)
│   │   └── mockDashboardService.ts  # ✅ API mockada (em uso)
│   ├── types/
│   │   └── dashboard.ts       # TypeScript types
│   ├── utils/
│   │   └── formatters.ts      # Formatação de moeda e data
│   ├── views/
│   │   └── ExpenseTrackingDashboard.vue
│   ├── router/
│   │   └── index.ts
│   ├── App.vue
│   ├── main.ts
│   └── style.css
├── Dockerfile                 # Build de produção
├── Dockerfile.dev             # Build de desenvolvimento
├── docker-compose.yml         # Orquestração Docker
├── start.sh                   # 🚀 Script de início rápido
└── test-local.sh              # 🧪 Teste local sem Docker
```

## 🔧 Comandos Úteis

### Docker

```bash
# Iniciar em desenvolvimento
docker-compose up dev

# Iniciar em produção
docker-compose up prod

# Parar containers
docker-compose down

# Rebuild completo
docker-compose build --no-cache
docker-compose up
```

### Local

```bash
# Instalar dependências
npm install

# Desenvolvimento
npm run dev

# Build de produção
npm run build

# Preview do build
npm run preview
```

## 🎯 Demonstração ao Usuário

### Roteiro Sugerido:

1. **Mostrar o Gráfico Termômetro**
   - Explicar o consumo de 72,8% do orçamento
   - Destacar os valores de consumido, restante e rendimento

2. **Demonstrar os Filtros**
   - Selecionar um projeto
   - Fazer uma busca (ex: "Pix")
   - Filtrar por status (ex: "Validado")
   - Filtrar por categoria (ex: "Equipamentos")
   - Clicar em "Buscar" para aplicar

3. **Navegar pela Tabela**
   - Mostrar as transações filtradas
   - Destacar os badges de status coloridos
   - Demonstrar a paginação
   - Mostrar o hover effect nas linhas

4. **Responsividade**
   - Redimensionar a janela do navegador
   - Mostrar como funciona em mobile/tablet

## 🔄 Alternando entre Mock e API Real

O projeto está configurado para usar dados mockados. Para conectar com a API real:

1. Abra `front-end/src/composables/useDashboard.ts`
2. Altere a linha:
   ```typescript
   // De:
   import * as dashboardService from '@/services/mockDashboardService'
   
   // Para:
   import * as dashboardService from '@/services/dashboardService'
   ```
3. Configure a variável de ambiente `VITE_API_BASE_URL` no arquivo `.env`

## 🛑 Parando o Dashboard

### Docker
```bash
# Pressione Ctrl+C no terminal
# Ou:
docker-compose down
```

### Local
```bash
# Pressione Ctrl+C no terminal
```

## 🐛 Solução de Problemas

### Porta já em uso
```bash
# Edite docker-compose.yml e mude a porta
# De: "5173:5173"
# Para: "3000:5173"
```

### Erro ao instalar dependências
```bash
# Limpe o cache do npm
rm -rf node_modules package-lock.json
npm install
```

### Container não inicia
```bash
# Limpe containers e imagens antigas
docker-compose down
docker system prune -f
docker-compose up --build
```

### Página em branco
```bash
# Verifique o console do navegador (F12)
# Verifique se os arquivos mock existem em public/mock-api/
```

## 📝 Tecnologias Utilizadas

- **Vue 3** - Framework JavaScript
- **TypeScript** - Tipagem estática
- **Tailwind CSS** - Framework CSS
- **Vite** - Build tool
- **Vue Router** - Roteamento
- **Docker** - Containerização
- **Nginx** - Servidor web (produção)

## 📞 Próximos Passos

Após a demonstração, você pode:

1. **Conectar com a API real** (seguir instruções acima)
2. **Customizar os dados mockados** (editar arquivos JSON)
3. **Adicionar novas funcionalidades** (seguir a estrutura existente)
4. **Deploy em produção** (usar o Dockerfile de produção)

## ✅ Checklist de Demonstração

- [ ] Dashboard carrega sem erros
- [ ] Gráfico termômetro exibe corretamente
- [ ] Filtros funcionam
- [ ] Tabela exibe transações
- [ ] Paginação funciona
- [ ] Status badges aparecem coloridos
- [ ] Responsivo em diferentes tamanhos
- [ ] Hover effects funcionam
- [ ] Sem erros no console do navegador

---

**Pronto para demonstrar! 🎉**

Para qualquer dúvida, consulte os arquivos:
- `front-end/README.md` - Documentação completa
- `front-end/QUICK_START.md` - Guia rápido
