# Expense Tracking Dashboard - Front-End

Dashboard de prestação de contas com visualização de orçamento e transações.

## 🚀 Rodando com Docker

### Modo Desenvolvimento (com hot-reload)

```bash
# Construir e iniciar o container de desenvolvimento
docker-compose up dev

# Ou usando docker diretamente
docker build -f Dockerfile.dev -t expense-dashboard-dev .
docker run -p 5173:5173 -v $(pwd):/app -v /app/node_modules expense-dashboard-dev
```

Acesse: http://localhost:5173

### Modo Produção

```bash
# Construir e iniciar o container de produção
docker-compose up prod

# Ou usando docker diretamente
docker build -t expense-dashboard .
docker run -p 8080:80 expense-dashboard
```

Acesse: http://localhost:8080

## 🛠️ Rodando Localmente (sem Docker)

### Pré-requisitos

- Node.js 20+
- npm ou yarn

### Instalação

```bash
# Instalar dependências
npm install

# Rodar em modo desenvolvimento
npm run dev

# Build para produção
npm run build

# Preview do build de produção
npm run preview
```

## 📊 Dados Mockados

O dashboard está configurado para usar dados mockados localizados em:
- `/public/mock-api/dashboard-data.json` - Projetos, categorias e métricas
- `/public/mock-api/transactions.json` - Transações

## 🎨 Funcionalidades

- ✅ Gráfico termômetro mostrando consumo do orçamento
- ✅ Filtros por projeto, data, status e categoria
- ✅ Tabela de transações com paginação
- ✅ Badges de status coloridos
- ✅ Tema escuro (zinc)
- ✅ Responsivo (mobile, tablet, desktop)
- ✅ Acessibilidade WCAG 2.0 Level AA

## 🏗️ Estrutura do Projeto

```
front-end/
├── public/
│   └── mock-api/          # Dados mockados
├── src/
│   ├── components/
│   │   ├── common/        # Componentes reutilizáveis
│   │   └── dashboard/     # Componentes do dashboard
│   ├── composables/       # Vue composables
│   ├── router/            # Vue Router
│   ├── services/          # Serviços de API
│   ├── types/             # TypeScript types
│   ├── utils/             # Funções utilitárias
│   └── views/             # Páginas
├── Dockerfile             # Build de produção
├── Dockerfile.dev         # Build de desenvolvimento
└── docker-compose.yml     # Orquestração Docker
```

## 🔧 Tecnologias

- Vue 3 (Composition API)
- TypeScript
- Tailwind CSS
- Vite
- Vue Router
- Docker & Nginx

## 📝 Notas

- O projeto usa o serviço mockado (`mockDashboardService.ts`) por padrão
- Para conectar com a API real, altere o import em `src/composables/useDashboard.ts`:
  ```typescript
  // De:
  import * as dashboardService from '@/services/mockDashboardService'
  // Para:
  import * as dashboardService from '@/services/dashboardService'
  ```
