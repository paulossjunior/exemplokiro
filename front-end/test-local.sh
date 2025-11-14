#!/bin/bash

echo "🧪 Testando Dashboard Localmente (sem Docker)..."
echo ""

# Verificar se Node.js está instalado
if ! command -v node &> /dev/null; then
    echo "❌ Node.js não está instalado. Por favor, instale o Node.js 20+ primeiro."
    exit 1
fi

echo "✅ Node.js $(node --version) encontrado"
echo ""

# Verificar se node_modules existe
if [ ! -d "node_modules" ]; then
    echo "📦 Instalando dependências..."
    npm install
    echo ""
fi

echo "🚀 Iniciando servidor de desenvolvimento..."
echo "📍 Acesse: http://localhost:5173"
echo ""
echo "Pressione Ctrl+C para parar"
echo ""

npm run dev
