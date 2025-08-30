# 🛡️ CONFIGURACIÓN AZURE API MANAGEMENT - OPCIÓN 1: SCOPES

## POLÍTICA APIM COMPLETA PARA BLENDING

```xml
<policies>
  <inbound>
    <!-- 🔐 VALIDACIÓN JWT REAL contra Azure AD -->
    <validate-jwt header-name="Authorization" failed-validation-httpcode="401" failed-validation-error-message="Token JWT inválido o expirado">
      <openid-config url="https://login.microsoftonline.com/f8d7cce6-0cf4-46cf-a13d-66f1099c05c8/v2.0/.well-known/openid_configuration" />
      <audiences>
        <audience>3bebf8d8-b4a1-4d2d-a31b-f44ad0d3b1cd</audience>
      </audiences>
      <!-- ✅ SCOPES REQUERIDOS POR RUTA -->
      <required-claims>
        <claim name="scp" match="any">
          <value>blending.read</value>
          <value>blending.write</value>
          <value>blending.admin</value>
        </claim>
      </required-claims>
    </validate-jwt>
    
    <!-- 🆔 HEADER: User ID para auditoría -->
    <set-header name="X-User-Id" exists-action="override">
      <value>@(context.Request.Headers.GetValueOrDefault("Authorization","").AsJwt()?.Claims.GetValueOrDefault("oid", ""))</value>
    </set-header>
    
    <!-- 👤 HEADER: Nombre del usuario -->
    <set-header name="X-User-Name" exists-action="override">
      <value>@(context.Request.Headers.GetValueOrDefault("Authorization","").AsJwt()?.Claims.GetValueOrDefault("name", ""))</value>
    </set-header>
    
    <!-- 📧 HEADER: Email del usuario -->
    <set-header name="X-User-Email" exists-action="override">
      <value>@(context.Request.Headers.GetValueOrDefault("Authorization","").AsJwt()?.Claims.GetValueOrDefault("preferred_username", ""))</value>
    </set-header>
    
    <!-- 👥 HEADER: Grupos del usuario -->
    <set-header name="X-User-Groups" exists-action="override">
      <value>@{
        var jwt = context.Request.Headers.GetValueOrDefault("Authorization","").AsJwt();
        var groups = jwt?.Claims.Where(c => c.Key == "groups").Select(c => c.Value);
        return groups != null ? string.Join(",", groups) : "";
      }</value>
    </set-header>
    
    <!-- 🔑 HEADER: Scopes del usuario -->
    <set-header name="X-User-Scopes" exists-action="override">
      <value>@(context.Request.Headers.GetValueOrDefault("Authorization","").AsJwt()?.Claims.GetValueOrDefault("scp", ""))</value>
    </set-header>

  </inbound>
  <backend>
    <forward-request />
  </backend>
  <outbound />
  <on-error />
</policies>
```

## 🛡️ PROTECCIÓN POR RUTAS CON SCOPES

### FUNCIÓN AUTH (Menús y UI)
**Scope Requerido:** `blending.read`
```xml
<!-- Solo en la operación GET /api/auth/menu -->
<required-claims>
  <claim name="scp" match="any">
    <value>blending.read</value>
    <value>blending.write</value>
    <value>blending.admin</value>
  </claim>
</required-claims>
```

### FUNCIÓN CORE (Business Logic)
**Scope Requerido:** `blending.write` o `blending.admin`
```xml
<!-- En operaciones POST/PUT/DELETE de /api/core/* -->
<required-claims>
  <claim name="scp" match="any">
    <value>blending.write</value>
    <value>blending.admin</value>
  </claim>
</required-claims>
```

### FUNCIÓN UPLOAD (Solo Admins)
**Scope Requerido:** `blending.admin`
```xml
<!-- En todas las operaciones de /api/upload/* -->
<required-claims>
  <claim name="scp" match="all">
    <value>blending.admin</value>
  </claim>
</required-claims>
```

## 📋 CONFIGURACIÓN AZURE AD APP REGISTRATION

### 1. SCOPES DEFINIDOS EN AZURE AD
```json
{
  "api://3bebf8d8-b4a1-4d2d-a31b-f44ad0d3b1cd": {
    "scopes": [
      {
        "name": "blending.read",
        "displayName": "Read Blending Data",
        "description": "Permite leer datos de blending y acceder a menús"
      },
      {
        "name": "blending.write", 
        "displayName": "Write Blending Data",
        "description": "Permite crear y modificar datos de blending"
      },
      {
        "name": "blending.admin",
        "displayName": "Admin Blending",
        "description": "Acceso completo a todas las funciones de blending"
      }
    ]
  }
}
```

### 2. ROLES EN AZURE AD (Mapeo a Scopes)
```json
{
  "appRoles": [
    {
      "allowedMemberTypes": ["User"],
      "displayName": "Blending Reader",
      "id": "...",
      "value": "BlendingReader"
    },
    {
      "allowedMemberTypes": ["User"],
      "displayName": "Blending Operator", 
      "id": "...",
      "value": "BlendingOperator"
    },
    {
      "allowedMemberTypes": ["User"],
      "displayName": "Blending Admin",
      "id": "...",
      "value": "BlendingAdmin"
    }
  ]
}
```

## ✅ VENTAJAS DE FUNCTION.AUTH CON APIM

### 🚀 Zero Auth Code
- Ya no maneja JWT, solo business logic
- Performance súper rápido, solo lee headers
- Focused: Solo se preocupa de generar menús

### 📊 Function.Auth se convierte en "Menu & UI Configuration Service"
- ✅ Generar menús dinámicos basados en roles
- ✅ Mapear grupos Azure AD a roles del negocio  
- ✅ Gestionar configuración de UI desde Azure App Config
- ✅ Proveer servicios específicos de autenticación del negocio

### 🔄 Maintainable & Scalable
- Lógica específica del dominio Auth/UI
- Puede escalar independiente de validación JWT
- Separación clara de responsabilidades

## 🏗️ ARQUITECTURA FINAL

```
Frontend (React/Angular)
    ↓ JWT Token
🛡️ Azure API Management 
    ├── validate-jwt (Azure AD)
    ├── set-headers (X-User-*)
    └── scope validation
    ↓ Headers Only
⚡ Azure Functions
    ├── Function.Auth (Menu Service)
    ├── Function.Core (Business Logic) 
    ├── Function.Upload (Admin Only)
    └── Function.Opt (Analytics)
```

## 🎯 SIGUIENTE PASO: CONFIGURAR APIM EN AZURE PORTAL
