---
inclusion: manual
---

# Front-End Technology Stack

## Tech Stack

- **Framework**: Vue 3 (Composition API)
- **Language**: TypeScript
- **CSS Framework**: Tailwind CSS
- **Build Tool**: Vite
- **Package Manager**: npm or pnpm
- **State Management**: Pinia (when needed)
- **HTTP Client**: Axios or Fetch API
- **Form Validation**: VeeValidate or custom composables
- **Accessibility**: WCAG 2.0 Level AA compliance

## Project Structure

```
front-end/
├── src/
│   ├── assets/          # Static assets (images, fonts)
│   ├── components/      # Reusable Vue components
│   │   ├── common/      # Generic UI components
│   │   ├── forms/       # Form components
│   │   └── layout/      # Layout components
│   ├── composables/     # Vue composables (reusable logic)
│   ├── views/           # Page components
│   ├── router/          # Vue Router configuration
│   ├── stores/          # Pinia stores
│   ├── services/        # API service layer
│   ├── types/           # TypeScript type definitions
│   ├── utils/           # Utility functions
│   ├── App.vue          # Root component
│   └── main.ts          # Application entry point
├── public/              # Public static files
├── index.html           # HTML entry point
├── tailwind.config.js   # Tailwind configuration
├── tsconfig.json        # TypeScript configuration
├── vite.config.ts       # Vite configuration
└── package.json         # Dependencies
```

## Development Setup

### Prerequisites

- Node.js 18+ and npm/pnpm
- Modern browser with Vue DevTools extension

### Installation

```bash
# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run linter
npm run lint

# Run type checking
npm run type-check
```

## Build Configuration

### Vite Configuration

```typescript
// vite.config.ts
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  },
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true
      }
    }
  }
})
```

### Tailwind Configuration

```javascript
// tailwind.config.js
module.exports = {
  content: [
    './index.html',
    './src/**/*.{vue,js,ts,jsx,tsx}'
  ],
  theme: {
    extend: {
      colors: {
        primary: '#3B82F6',
        secondary: '#10B981',
        danger: '#EF4444'
      }
    }
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography')
  ]
}
```

## TypeScript Configuration

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "module": "ESNext",
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "jsx": "preserve",
    "strict": true,
    "moduleResolution": "node",
    "resolveJsonModule": true,
    "esModuleInterop": true,
    "skipLibCheck": true,
    "baseUrl": ".",
    "paths": {
      "@/*": ["./src/*"]
    }
  },
  "include": ["src/**/*.ts", "src/**/*.d.ts", "src/**/*.tsx", "src/**/*.vue"],
  "exclude": ["node_modules"]
}
```
