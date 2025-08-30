# ⚡ CORRECCIÓN IMPORTANTE - FLUJO DE TOKENS

## 🔥 LO QUE REALMENTE PASA CON EL JWT:

```
┌─────────────────┐                ┌─────────────────┐                ┌─────────────────┐
│   📱 FRONTEND    │                │   🛡️ APIM       │                │ ☁️ AZURE FUNCTION│
│                 │                │                 │                │                 │
│  fetch(url, {   │   JWT TOKEN    │                 │  HEADERS ONLY  │                 │
│    headers: {   │───────────────▶│                 │───────────────▶│                 │
│    'Authorization│                │                 │                │                 │
│    : Bearer eyJ'│                │  🔍 INTERCEPTA  │                │ 📋 RECIBE       │
│    }            │                │  🛡️ VALIDA     │                │ ✅ Xçl │
└─────────────────┘                └─────────────────┘                └─────────────────┘
        │                                   │                                   │
        │ ✅ SÍ ENVÍA JWT                   │ ✅ SÍ PROCESA JWT                 │ ❌ NO VE JWT
        │    AL BACKEND                     │    Y LO QUITA                      │    SOLO HEADERS
        │                                   │                                   │
```

## 📋 EJEMPLOS REALES DE REQUESTS:

### 🚀 REQUEST DEL FRONTEND:
```javascript
// ✅ Frontend SÍ envía JWT Token:
fetch('https://blending-apim.azure-api.net/api/auth/menu', {
    method: 'GET',
    headers: {
        'Authorization': 'Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIs...',
        'Content-Type': 'application/json'
    }
});
```

### 🛡️ APIM INTERCEPTA Y PROCESA:
```xml
<!-- APIM Policy hace esto: -->
<policies>
  <inbound>
    <!-- 1️⃣ RECIBE JWT del frontend -->
    <validate-jwt header-name="Authorization">
      <!-- Valida token contra Azure AD -->
    </validate-jwt>
    
    <!-- 2️⃣ EXTRAE claims del JWT -->
    <set-header name="X-User-Id" exists-action="override">
      <value>@(context.Request.Headers.GetValueOrDefault("Authorization","").AsJwt()?.Claims.GetValueOrDefault("oid", ""))</value>
    </set-header>
    
    <!-- 3️⃣ ELIMINA JWT del request -->
    <set-header name="Authorization" exists-action="delete" />
  </inbound>
</policies>
```

### ☁️ FUNCTION RECIBE REQUEST LIMPIO:
```http
# ✅ Lo que llega a Function.Auth:
GET /api/menu HTTP/1.1
X-User-Id: a1b2c3d4-e5f6-7890-abcd-ef1234567890
X-User-Name: Juan Pérez  
X-User-Email: juan.perez@company.com
X-User-Groups: Admin,Calidad
X-User-Scopes: blending.read blending.write

# ❌ NO HAY Authorization header
# ❌ NO HAY JWT Token
# ✅ SOLO headers limpios
```

## 🎯 VENTAJAS DE ESTE FLUJO:

### ✅ FRONTEND SIMPLE:
```javascript
// Frontend no cambia nada:
const token = localStorage.getItem('access_token');
fetch('/api/auth/menu', {
    headers: { 'Authorization': `Bearer ${token}` }
});
```

### ✅ APIM MANEJA TODO:
- Recibe JWT del frontend
- Valida contra Azure AD  
- Extrae claims necesarios
- Elimina JWT del request
- Agrega headers clean
- Envía a Function

### ✅ FUNCTION SÚPER SIMPLE:
```csharp
// Function no ve JWT nunca:
public async Task<IActionResult> GetMenu(HttpRequestData req)
{
    var userId = req.Headers.GetValues("X-User-Id").FirstOrDefault();
    var userName = req.Headers.GetValues("X-User-Name").FirstOrDefault();
    
    // Genera menú basado en headers
    var menu = await GenerateMenu(userId, userName);
    return menu;
}
```

## 🔥 CORRECCIÓN AL DIAGRAMA ANTERIOR:

**SÍ, el frontend ENVÍA JWT al backend**, pero:

1. **APIM lo intercepta** antes de llegar a Functions
2. **APIM lo valida** contra Azure AD
3. **APIM lo elimina** del request  
4. **APIM agrega headers** con user info
5. **Function recibe** request sin JWT, solo headers

Es como un **"JWT → Headers Translator"** transparente.

¿Ahora está más claro el flujo real?
