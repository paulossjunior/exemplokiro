#!/bin/bash

echo "🔍 Verificando ambiente..."
echo ""

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Verificar Docker
if command -v docker &> /dev/null; then
    echo -e "${GREEN}✅ Docker instalado:${NC} $(docker --version)"
else
    echo -e "${RED}❌ Docker não encontrado${NC}"
    echo "   Instale em: https://docs.docker.com/get-docker/"
fi

# Verificar docker-compose
if command -v docker-compose &> /dev/null; then
    echo -e "${GREEN}✅ docker-compose instalado:${NC} $(docker-compose --version)"
else
    echo -e "${RED}❌ docker-compose não encontrado${NC}"
    echo "   Instale em: https://docs.docker.com/compose/install/"
fi

# Verificar Node.js
if command -v node &> /dev/null; then
    echo -e "${GREEN}✅ Node.js instalado:${NC} $(node --version)"
else
    echo -e "${YELLOW}⚠️  Node.js não encontrado${NC}"
    echo "   (Opcional - necessário apenas para rodar sem Docker)"
fi

echo ""

# Verificar arquivos mockados
if [ -f "public/mock-api/dashboard-data.json" ]; then
    echo -e "${GREEN}✅ Dados mockados encontrados${NC}"
else
    echo -e "${RED}❌ Arquivos mockados não encontrados${NC}"
    exit 1
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "🎯 Escolha como deseja iniciar:"
echo ""
echo "1) 🐳 Docker - Desenvolvimento (hot-reload, porta 5173)"
echo "2) 🐳 Docker - Produção (otimizado, porta 8080)"
echo "3) 💻 Local - Sem Docker (porta 5173)"
echo "4) ❌ Cancelar"
echo ""
read -p "Digite sua escolha (1-4): " choice

case $choice in
    1)
        echo ""
        echo "🚀 Iniciando com Docker em modo DESENVOLVIMENTO..."
        echo "📍 Acesse: http://localhost:5173"
        echo ""
        docker-compose up dev
        ;;
    2)
        echo ""
        echo "🚀 Iniciando com Docker em modo PRODUÇÃO..."
        echo "📍 Acesse: http://localhost:8080"
        echo ""
        docker-compose up prod
        ;;
    3)
        if ! command -v node &> /dev/null; then
            echo -e "${RED}❌ Node.js não está instalado. Instale primeiro ou use Docker.${NC}"
            exit 1
        fi
        echo ""
        echo "🚀 Iniciando LOCALMENTE..."
        if [ ! -d "node_modules" ]; then
            echo "📦 Instalando dependências..."
            npm install
        fi
        echo "📍 Acesse: http://localhost:5173"
        echo ""
        npm run dev
        ;;
    4)
        echo "👋 Até logo!"
        exit 0
        ;;
    *)
        echo -e "${RED}❌ Opção inválida${NC}"
        exit 1
        ;;
esac
