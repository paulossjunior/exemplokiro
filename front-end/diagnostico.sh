#!/bin/bash

echo "🔍 Diagnóstico do Dashboard"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Cores
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Verificar arquivos mockados
echo "📊 Verificando arquivos de dados mockados..."
if [ -f "public/mock-api/dashboard-data.json" ]; then
    echo -e "${GREEN}✅ dashboard-data.json encontrado${NC}"
    echo "   Tamanho: $(wc -c < public/mock-api/dashboard-data.json) bytes"
else
    echo -e "${RED}❌ dashboard-data.json NÃO encontrado${NC}"
fi

if [ -f "public/mock-api/transactions.json" ]; then
    echo -e "${GREEN}✅ transactions.json encontrado${NC}"
    echo "   Tamanho: $(wc -c < public/mock-api/transactions.json) bytes"
    echo "   Transações: $(grep -o '"id"' public/mock-api/transactions.json | wc -l)"
else
    echo -e "${RED}❌ transactions.json NÃO encontrado${NC}"
fi

echo ""

# Verificar estrutura de diretórios
echo "📁 Verificando estrutura de diretórios..."
if [ -d "src/components/dashboard" ]; then
    echo -e "${GREEN}✅ src/components/dashboard/ existe${NC}"
    echo "   Componentes: $(ls src/components/dashboard/*.vue 2>/dev/null | wc -l)"
else
    echo -e "${RED}❌ src/components/dashboard/ NÃO existe${NC}"
fi

if [ -d "src/services" ]; then
    echo -e "${GREEN}✅ src/services/ existe${NC}"
    echo "   Serviços: $(ls src/services/*.ts 2>/dev/null | wc -l)"
else
    echo -e "${RED}❌ src/services/ NÃO existe${NC}"
fi

echo ""

# Verificar arquivos principais
echo "📄 Verificando arquivos principais..."
files=(
    "src/main.ts"
    "src/App.vue"
    "src/views/ExpenseTrackingDashboard.vue"
    "src/composables/useDashboard.ts"
    "src/services/mockDashboardService.ts"
    "index.html"
    "vite.config.ts"
    "package.json"
)

for file in "${files[@]}"; do
    if [ -f "$file" ]; then
        echo -e "${GREEN}✅${NC} $file"
    else
        echo -e "${RED}❌${NC} $file"
    fi
done

echo ""

# Verificar node_modules
echo "📦 Verificando dependências..."
if [ -d "node_modules" ]; then
    echo -e "${GREEN}✅ node_modules instalado${NC}"
    if [ -d "node_modules/vue" ]; then
        echo -e "${GREEN}✅ Vue instalado${NC}"
    else
        echo -e "${RED}❌ Vue NÃO instalado${NC}"
    fi
else
    echo -e "${RED}❌ node_modules NÃO encontrado${NC}"
    echo -e "${YELLOW}   Execute: npm install${NC}"
fi

echo ""

# Verificar JSON válido
echo "🔍 Validando arquivos JSON..."
if command -v jq &> /dev/null; then
    if [ -f "public/mock-api/dashboard-data.json" ]; then
        if jq empty public/mock-api/dashboard-data.json 2>/dev/null; then
            echo -e "${GREEN}✅ dashboard-data.json é JSON válido${NC}"
        else
            echo -e "${RED}❌ dashboard-data.json tem erro de sintaxe${NC}"
        fi
    fi
    
    if [ -f "public/mock-api/transactions.json" ]; then
        if jq empty public/mock-api/transactions.json 2>/dev/null; then
            echo -e "${GREEN}✅ transactions.json é JSON válido${NC}"
        else
            echo -e "${RED}❌ transactions.json tem erro de sintaxe${NC}"
        fi
    fi
else
    echo -e "${YELLOW}⚠️  jq não instalado (opcional)${NC}"
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "💡 Próximos passos:"
echo ""
echo "1. Se tudo está ✅, rode: npm run dev"
echo "2. Acesse: http://localhost:5173"
echo "3. Abra DevTools (F12) e veja o Console"
echo "4. Procure por logs: 'Fetching transactions...'"
echo ""
echo "Se houver ❌, corrija os problemas antes de iniciar."
echo ""
