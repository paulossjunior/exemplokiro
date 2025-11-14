# 🔧 Troubleshooting - Dashboard

## Problema: Transações não aparecem na tabela

### Passo 1: Verificar Console do Navegador

1. Abra o dashboard no navegador
2. Pressione `F12` para abrir DevTools
3. Vá na aba **Console**
4. Procure por:
   - ✅ "Fetching transactions with params:"
   - ✅ "Raw transaction data:"
   - ✅ "Returning transactions:"
   - ❌ Erros em vermelho

### Passo 2: Verificar Network

1. Na aba **Network** do DevTools
2. Recarregue a página (`Ctrl+R`)
3. Procure por:
   - `transactions.json` - deve retornar **200 OK**
   - `dashboard-data.json` - deve retornar **200 OK**

### Passo 3: Verificar Arquivos Mockados

```bash
# Verificar se os arquivos existem
ls -la front-end/public/mock-api/

# Deve mostrar:
# - dashboard-data.json
# - transactions.json
```

### Passo 4: Verificar Conteúdo dos Arquivos

```bash
# Ver conteúdo do transactions.json
cat front-end/public/mock-api/transactions.json | head -20

# Deve mostrar JSON válido com array "transactions"
```

## Soluções Comuns

### Solução 1: Recarregar Página
```
Ctrl+Shift+R (hard reload)
```

### Solução 2: Limpar Cache do Navegador
1. DevTools aberto (F12)
2. Clique com botão direito no ícone de reload
3. Escolha "Empty Cache and Hard Reload"

### Solução 3: Verificar Caminho dos Arquivos
Os arquivos devem estar em:
```
front-end/public/mock-api/transactions.json
front-end/public/mock-api/dashboard-data.json
```

### Solução 4: Reiniciar Servidor
```bash
# Parar (Ctrl+C)
# Iniciar novamente
npm run dev
```

### Solução 5: Reinstalar Dependências
```bash
rm -rf node_modules package-lock.json
npm install
npm run dev
```

## Verificações Adicionais

### Verificar se o componente está montado
No console do navegador, digite:
```javascript
// Deve mostrar o array de transações
console.log(document.querySelector('table'))
```

### Verificar estado do Vue
Instale a extensão **Vue DevTools** e verifique:
- Estado do componente `ExpenseTrackingDashboard`
- Valor de `transactions` (deve ser um array)
- Valor de `loading` (deve ser false após carregar)
- Valor de `error` (deve ser null)

## Logs Úteis

O serviço mockado agora tem logs. Você deve ver no console:

```
Fetching transactions with params: {page: 1, pageSize: 20}
Raw transaction data: {transactions: Array(20), pagination: {...}}
Returning transactions: {data: Array(20), currentPage: 1, ...}
```

Se não vir esses logs, o problema está na inicialização.

## Ainda não funciona?

1. **Verifique a URL**: Deve ser `http://localhost:5173`
2. **Verifique erros no terminal**: Onde está rodando `npm run dev`
3. **Tente outro navegador**: Chrome, Firefox, Edge
4. **Verifique permissões**: Os arquivos JSON devem ser legíveis

## Contato de Emergência

Se nada funcionar, compartilhe:
1. Screenshot do console (F12)
2. Screenshot da aba Network
3. Output do terminal onde roda `npm run dev`
4. Conteúdo de `ls -la front-end/public/mock-api/`
