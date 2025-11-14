#!/bin/bash

echo "🚀 Iniciando Expense Tracking Dashboard..."
echo ""

# Verificar se Docker está instalado
if ! command -v docker &> /dev/null; then
    echo "❌ Docker não está instalado. Por favor, instale o Docker primeiro."
    exit 1
fi

# Verificar se docker-compose está instalado
if ! command -v docker-compose &> /dev/null; then
    echo "❌ docker-compose não está instalado. Por favor, instale o docker-compose primeiro."
    exit 1
fi

echo "✅ Docker e docker-compose encontrados"
echo ""

# Perguntar qual modo rodar
echo "Escolha o modo de execução:"
echo "1) Desenvolvimento (hot-reload, porta 5173)"
echo "2) Produção (otimizado, porta 8080)"
read -p "Digite 1 ou 2: " mode

if [ "$mode" = "1" ]; then
    echo ""
    echo "🔨 Iniciando em modo DESENVOLVIMENTO..."
    echo "📍 Acesse: http://localhost:5173"
    echo ""
    docker-compose up dev
elif [ "$mode" = "2" ]; then
    echo ""
    echo "🏭 Iniciando em modo PRODUÇÃO..."
    echo "📍 Acesse: http://localhost:8080"
    echo ""
    docker-compose up prod
else
    echo "❌ Opção inválida. Use 1 ou 2."
    exit 1
fi
