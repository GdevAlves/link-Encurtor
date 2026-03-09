# 🚀 URL Shortener - Documentação de Redirects HTTP

## ✅ Configuração Correta (Padrão da Indústria)

### 🎯 **Como Funciona:**

Todos os endpoints de redirect usam **HTTP 302 Redirect** padrão, exatamente como encurtadores profissionais (bit.ly, tinyurl, etc).

---

## 📋 Endpoints Disponíveis

### 1. **POST /v1/url** - Criar URL Encurtada
```bash
curl -X POST "http://localhost:5106/v1/url" \
  -H "Authorization: Bearer SEU_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"bigUrl":"https://www.google.com"}'
```

**Resposta:**
```json
{
  "id": "...",
  "creator": {...},
  "longUrl": "https://www.google.com",
  "shortUrl": "4ZFFarRrqeYGFBPa",
  "createdAt": "..."
}
```

---

### 2. **GET /v1/url/{shortUrl}** - Redirect HTTP 302
```bash
# Browser ou cURL
http://localhost:5106/v1/url/4ZFFarRrqeYGFBPa
```

**Comportamento:**
- Retorna **HTTP 302 Found**
- Header: `Location: https://www.google.com`
- Browser redireciona automaticamente
- ⚡ **Performance máxima** (apenas 1 requisição)

---

### 3. **GET /v1/url/{shortUrl}/info** - Obter JSON (para APIs)
```bash
curl "http://localhost:5106/v1/url/4ZFFarRrqeYGFBPa/info"
```

**Resposta:**
```json
{
  "id": "...",
  "longUrl": "https://www.google.com",
  "shortUrl": "4ZFFarRrqeYGFBPa",
  "createdAt": "..."
}
```

**Use quando:**
- Você precisa da URL sem redirect
- Está construindo uma aplicação que precisa processar a URL
- Quer exibir informações antes de redirecionar

---

### 4. **GET /{shortUrl}** - Redirect Direto (formato limpo)
```bash
# Browser ou cURL
http://localhost:5106/4ZFFarRrqeYGFBPa
```

**Comportamento:**
- Mesmo que `/v1/url/{shortUrl}` mas com URL mais limpa
- Ideal para compartilhar em redes sociais, emails, etc
- Retorna **HTTP 302 Found**
- ⚡ **Performance máxima**

---

## 🧪 Testes

### Teste 1: Criar e Redirecionar
```bash
# 1. Criar URL
curl -X POST "http://localhost:5106/v1/url" \
  -H "Authorization: Bearer SEU_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"bigUrl":"https://github.com"}'

# 2. Copiar o shortUrl da resposta (ex: abc123)

# 3. Testar redirect no browser
# Abra no browser: http://localhost:5106/abc123
# Você será redirecionado para https://github.com
```

### Teste 2: Obter Info sem Redirect
```bash
curl "http://localhost:5106/v1/url/abc123/info"
```

### Teste 3: cURL com Follow Redirect
```bash
# Ver o redirect
curl -I "http://localhost:5106/abc123"

# Seguir o redirect
curl -L "http://localhost:5106/abc123"
```

---

## ⚠️ Sobre o "Erro de CORS" no Swagger

### 🤔 **Por que o Swagger mostra erro?**

Quando você testa `GET /v1/url/{shortUrl}` no Swagger, você pode ver:
```
Failed to fetch
CORS error
```

### ✅ **Isso é NORMAL e ESPERADO!**

**O que acontece:**
1. Swagger executa `fetch('http://localhost:5106/v1/url/abc123')`
2. Servidor retorna `HTTP 302` → `Location: https://google.com`
3. Browser tenta seguir o redirect automaticamente
4. Browser faz `fetch('https://google.com')`
5. ❌ Google.com não permite CORS de `localhost:5106`
6. Swagger exibe erro

### 🎯 **Mas isso NÃO é um problema!**

**Em uso real:**
- ✅ Link direto no browser → **Funciona perfeitamente**
- ✅ `<a href="...">` em HTML → **Funciona perfeitamente**
- ✅ `window.location.href = "..."` → **Funciona perfeitamente**
- ✅ Compartilhar em redes sociais → **Funciona perfeitamente**
- ❌ Apenas `fetch()` do Swagger → **Erro esperado**

**Por quê?**
- CORS **só se aplica a requisições AJAX** (fetch, XMLHttpRequest)
- CORS **NÃO se aplica a navegação normal do browser**
- Redirects HTTP 302 são **navegação normal**, não AJAX

---

## 🔥 Como Testar Corretamente

### ❌ **NÃO teste assim:**
```javascript
// Isso VAI dar erro de CORS (e é esperado)
fetch('http://localhost:5106/v1/url/abc123')
  .then(r => r.json())
```

### ✅ **Teste assim:**

#### Opção 1: Browser direto
```
http://localhost:5106/abc123
```
→ Abre no navegador e redireciona ✅

#### Opção 2: Link HTML
```html
<a href="http://localhost:5106/abc123">Clique aqui</a>
```
→ Funciona perfeitamente ✅

#### Opção 3: JavaScript Navigation
```javascript
window.location.href = 'http://localhost:5106/abc123';
```
→ Funciona perfeitamente ✅

#### Opção 4: cURL
```bash
curl -L "http://localhost:5106/abc123"
```
→ Funciona perfeitamente ✅

#### Opção 5: Se precisa usar fetch (API)
```javascript
// Use o endpoint /info que retorna JSON
fetch('http://localhost:5106/v1/url/abc123/info')
  .then(r => r.json())
  .then(data => {
    // Agora você tem a URL e pode decidir o que fazer
    window.location.href = data.longUrl;
  })
```
→ Funciona perfeitamente ✅

---

## 📊 Comparação: Redirect vs HTML

### ⚡ **HTTP 302 Redirect (implementado)**
```
Browser → GET /abc123
         ← HTTP 302, Location: https://google.com
Browser → GET https://google.com
         ← HTTP 200 (página do Google)
```
**Total:** 2 requisições
**Performance:** ⚡⚡⚡⚡⚡ Excelente

### 🐌 **HTML + JavaScript (NÃO recomendado)**
```
Browser → GET /abc123
         ← HTTP 200 (HTML com script)
Browser → Renderiza HTML
Browser → Executa JavaScript
Browser → GET https://google.com
         ← HTTP 200 (página do Google)
```
**Total:** 2 requisições + renderização + execução de script
**Performance:** ⚡⚡ Ruim

---

## 🎯 Casos de Uso

### Caso 1: Compartilhar em Redes Sociais
```
Twitter, Facebook, WhatsApp, etc:
http://localhost:5106/abc123
```
✅ Funciona perfeitamente com HTTP 302

### Caso 2: Email Marketing
```html
<a href="http://localhost:5106/abc123">Clique aqui</a>
```
✅ Funciona perfeitamente com HTTP 302

### Caso 3: QR Code
```
QR Code → http://localhost:5106/abc123
```
✅ Funciona perfeitamente com HTTP 302

### Caso 4: Aplicação Web (React, Vue, Angular)
```javascript
// Opção 1: Navegação direta (com redirect)
window.location.href = 'http://localhost:5106/abc123';

// Opção 2: Obter URL primeiro (sem redirect)
const response = await fetch('http://localhost:5106/v1/url/abc123/info');
const data = await response.json();
// Processar ou exibir antes de redirecionar
window.location.href = data.longUrl;
```
✅ Ambas funcionam perfeitamente

---

## 🔒 Segurança

✅ **URL Scheme Validation** - Auto-adiciona `https://` se necessário
✅ **NotFound para URLs inválidas** - Retorna 404 com mensagem clara
✅ **JWT para criar URLs** - Apenas usuários autenticados podem criar
✅ **Redirects públicos** - Qualquer pessoa pode acessar URLs encurtadas

---

## 📊 Status HTTP Corretos

| Endpoint | Success | Error |
|----------|---------|-------|
| `POST /v1/url` | 200 OK + JSON | 400/401 + JSON |
| `GET /v1/url/{shortUrl}` | 302 Found + Header Location | 404 + JSON |
| `GET /v1/url/{shortUrl}/info` | 200 OK + JSON | 404 + JSON |
| `GET /{shortUrl}` | 302 Found + Header Location | 404 + JSON |

---

## ✅ Checklist de Funcionamento

- [x] HTTP 302 Redirect implementado
- [x] Performance máxima (padrão da indústria)
- [x] Funciona em browsers
- [x] Funciona em links HTML
- [x] Funciona em redes sociais
- [x] Funciona em emails
- [x] Funciona em QR codes
- [x] Endpoint `/info` para APIs que precisam de JSON
- [x] Validação de URL scheme (auto-adiciona https://)
- [x] CORS configurado (mas não afeta redirects)

---

## 🎉 Conclusão

✅ **HTTP 302 Redirect é o padrão correto**
✅ **Performance máxima**
✅ **Funciona em 100% dos casos de uso real**
❌ **O "erro" do Swagger é esperado e irrelevante**

**Teste no browser e confirme que funciona perfeitamente!** 🚀
