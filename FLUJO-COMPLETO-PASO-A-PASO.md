# 🔄 FLUJO COMPLETO PASO A PASO - SIN DIAGRAMAS

## 1️⃣ USUARIO ABRE LA APLICACIÓN

**¿Qué pasa?**
- Usuario abre el navegador web
- Va a la URL de la aplicación (ej: https://blending-app.com)
- Ve la página de login

**¿Qué ve el usuario?**
- Formulario con campos: Email y Password
- Botón "Iniciar Sesión"
- Logo de la empresa

## 2️⃣ USUARIO HACE LOGIN

**¿Qué pasa?**
- Usuario ingresa email: juan.perez@company.com
- Usuario ingresa password: su_password_123
- Usuario hace click en "Iniciar Sesión"

**¿Qué hace el frontend?**
- Redirige al usuario a Azure Active Directory
- Azure AD valida las credenciales del usuario
- Azure AD genera un JWT Token con información del usuario
- Azure AD devuelve el usuario al frontend con el JWT Token

**¿Qué contiene el JWT Token?**
```
{
  "oid": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",  // ID único del usuario
  "name": "Juan Pérez",                            // Nombre completo
  "preferred_username": "juan.perez@company.com",  // Email
  "groups": ["Admin", "Calidad"],                  // Grupos de Azure AD
  "scp": "blending.read blending.write",           // Permisos/Scopes
  "tid": "f8d7cce6-0cf4-46cf-a13d-66f1099c05c8",  // Tenant ID
  "exp": 1693942800                                // Fecha de expiración
}
```

**¿Qué hace el frontend con el token?**
- Guarda el JWT Token en localStorage
- Redirige al usuario al dashboard principal

## 3️⃣ USUARIO NAVEGA EN LA APLICACIÓN

**¿Qué ve el usuario?**
- Dashboard principal con menú lateral
- El menú está vacío porque aún no se ha cargado

**¿Qué hace el frontend automáticamente?**
- Necesita cargar el menú personalizado para este usuario
- Hace un request al backend para obtener el menú

**Request que envía el frontend:**
```http
GET https://blending-apim.azure-api.net/api/auth/menu
Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIs...
Content-Type: application/json
```

## 4️⃣ AZURE API MANAGEMENT INTERCEPTA EL REQUEST

**¿Qué recibe APIM?**
- URL: /api/auth/menu
- Header: Authorization: Bearer eyJ0eXAi...
- Método: GET

**¿Qué hace APIM paso a paso?**

**Paso 4.1 - Validar JWT Token:**
- APIM toma el JWT del header Authorization
- Conecta con Azure Active Directory
- Verifica que la firma del token sea válida
- Verifica que el token no haya expirado
- Verifica que el audience sea correcto (3bebf8d8-b4a1-4d2d-a31b-f44ad0d3b1cd)

**Paso 4.2 - Verificar Permisos:**
- APIM lee los scopes del JWT: "blending.read blending.write"
- Para /api/auth/menu se requiere mínimo "blending.read"
- El usuario tiene "blending.read" ✅ AUTORIZADO

**Paso 4.3 - Extraer Información del Usuario:**
- APIM lee el claim "oid": a1b2c3d4-e5f6-7890-abcd-ef1234567890
- APIM lee el claim "name": Juan Pérez
- APIM lee el claim "preferred_username": juan.perez@company.com
- APIM lee el claim "groups": Admin,Calidad
- APIM lee el claim "scp": blending.read blending.write

**Paso 4.4 - Crear Headers Limpios:**
- APIM elimina el header Authorization (elimina el JWT)
- APIM agrega header X-User-Id: a1b2c3d4-e5f6-7890-abcd-ef1234567890
- APIM agrega header X-User-Name: Juan Pérez
- APIM agrega header X-User-Email: juan.perez@company.com
- APIM agrega header X-User-Groups: Admin,Calidad
- APIM agrega header X-User-Scopes: blending.read blending.write

**Paso 4.5 - Enviar Request a Function:**
- APIM identifica que /api/auth/* debe ir a Function.Blending.Auth
- APIM envía el request modificado a Function.Blending.Auth

## 5️⃣ FUNCTION.BLENDING.AUTH PROCESA EL REQUEST

**¿Qué recibe Function.Auth?**
```http
GET https://func-blending-auth.azurewebsites.net/api/menu
X-User-Id: a1b2c3d4-e5f6-7890-abcd-ef1234567890
X-User-Name: Juan Pérez
X-User-Email: juan.perez@company.com
X-User-Groups: Admin,Calidad
X-User-Scopes: blending.read blending.write
Content-Type: application/json

(NO HAY Authorization header - APIM lo eliminó)
```

**¿Qué hace Function.Auth paso a paso?**

**Paso 5.1 - Leer Headers:**
```csharp
var userId = req.Headers.GetValues("X-User-Id").FirstOrDefault();
// userId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890"

var userName = req.Headers.GetValues("X-User-Name").FirstOrDefault();
// userName = "Juan Pérez"

var userGroups = req.Headers.GetValues("X-User-Groups").FirstOrDefault();
// userGroups = "Admin,Calidad"
```

**Paso 5.2 - Mapear Grupos a Roles de Negocio:**
```csharp
var roles = new List<string>();
if (userGroups.Contains("Admin")) roles.Add("Administrador");
if (userGroups.Contains("Calidad")) roles.Add("Control_Calidad");
if (userGroups.Contains("Logistica")) roles.Add("Logistica");
// roles = ["Administrador", "Control_Calidad"]
```

**Paso 5.3 - Generar Menú Dinámico:**
```csharp
var menu = new List<MenuItem>();

// Para Admin: todos los módulos
if (roles.Contains("Administrador")) {
    menu.Add(new MenuItem { Name = "Dashboard", Url = "/dashboard", Icon = "📊" });
    menu.Add(new MenuItem { Name = "Plantas", Url = "/plantas", Icon = "🏭" });
    menu.Add(new MenuItem { Name = "Calidad", Url = "/calidad", Icon = "✅" });
    menu.Add(new MenuItem { Name = "Parámetros", Url = "/parametros", Icon = "⚙️" });
    menu.Add(new MenuItem { Name = "Upload", Url = "/upload", Icon = "📁" });
    menu.Add(new MenuItem { Name = "Reportes", Url = "/reportes", Icon = "📊" });
}

// Para Control_Calidad: módulos específicos
if (roles.Contains("Control_Calidad")) {
    menu.Add(new MenuItem { Name = "Calidad", Url = "/calidad", Icon = "✅" });
    menu.Add(new MenuItem { Name = "Reportes Calidad", Url = "/reportes/calidad", Icon = "📋" });
}
```

**Paso 5.4 - Retornar Response:**
```json
{
  "success": true,
  "data": {
    "user": {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "name": "Juan Pérez",
      "email": "juan.perez@company.com",
      "roles": ["Administrador", "Control_Calidad"]
    },
    "menu": [
      { "name": "Dashboard", "url": "/dashboard", "icon": "📊" },
      { "name": "Plantas", "url": "/plantas", "icon": "🏭" },
      { "name": "Calidad", "url": "/calidad", "icon": "✅" },
      { "name": "Parámetros", "url": "/parametros", "icon": "⚙️" },
      { "name": "Upload", "url": "/upload", "icon": "📁" },
      { "name": "Reportes", "url": "/reportes", "icon": "📊" }
    ]
  }
}
```

## 6️⃣ RESPONSE REGRESA AL FRONTEND

**¿Qué hace APIM?**
- Recibe la response de Function.Auth
- No modifica la response (solo modifica requests)
- Envía la response al frontend tal como está

**¿Qué recibe el frontend?**
- El JSON con la información del usuario y el menú
- Status 200 (exitoso)

**¿Qué hace el frontend?**
```javascript
fetch('/api/auth/menu')
.then(response => response.json())
.then(data => {
    // Actualizar información del usuario en la UI
    document.getElementById('user-name').textContent = data.user.name;
    document.getElementById('user-email').textContent = data.user.email;
    
    // Construir menú lateral dinámicamente
    const sidebar = document.getElementById('sidebar-menu');
    data.menu.forEach(item => {
        const menuItem = document.createElement('a');
        menuItem.href = item.url;
        menuItem.innerHTML = `${item.icon} ${item.name}`;
        sidebar.appendChild(menuItem);
    });
});
```

## 7️⃣ USUARIO VE EL RESULTADO FINAL

**¿Qué ve el usuario en pantalla?**
- Su nombre "Juan Pérez" en la esquina superior derecha
- Su email "juan.perez@company.com" en el perfil
- Menú lateral con opciones:
  - 📊 Dashboard
  - 🏭 Plantas  
  - ✅ Calidad
  - ⚙️ Parámetros
  - 📁 Upload
  - 📊 Reportes

**¿Qué pasa si el usuario hace click en "Plantas"?**
- Frontend hace otro request: GET /api/core/plantas
- Se repite todo el proceso (pasos 4-6)
- APIM valida JWT, agrega headers, envía a Function.Core
- Function.Core lee headers, consulta base de datos, retorna lista de plantas
- Frontend muestra tabla con las plantas

## 8️⃣ FLUJO PARA OTROS USUARIOS (Ejemplo: Operador)

Si otro usuario "María García" con grupo "Operador" hace login:

**Su JWT contendría:**
```
{
  "oid": "b2c3d4e5-f6g7-8901-bcde-f23456789012",
  "name": "María García",
  "preferred_username": "maria.garcia@company.com", 
  "groups": ["Operador"],
  "scp": "blending.read"  // Solo lectura
}
```

**Su menú sería diferente:**
```json
{
  "menu": [
    { "name": "Dashboard", "url": "/dashboard", "icon": "📊" },
    { "name": "Plantas", "url": "/plantas", "icon": "🏭" },
    { "name": "Calidad", "url": "/calidad", "icon": "✅" }
    // Sin Upload, sin Parámetros, sin Reportes
  ]
}
```

**Si María intenta acceder a /api/upload/files:**
- APIM valida su JWT: scope = "blending.read"
- /api/upload/* requiere "blending.admin" 
- APIM retorna 403 Forbidden (no autorizado)
- María nunca ve el módulo Upload en su menú

## 🎯 RESUMEN DEL FLUJO:

1. **Usuario hace login** → Azure AD genera JWT
2. **Frontend guarda JWT** → Lo envía en cada request
3. **APIM intercepta JWT** → Valida, extrae info, agrega headers
4. **Function procesa headers** → Genera response personalizada  
5. **Usuario ve resultado** → Menú y datos según sus permisos

**¡Cero código de autenticación en las Functions!** Solo leen headers limpios.
