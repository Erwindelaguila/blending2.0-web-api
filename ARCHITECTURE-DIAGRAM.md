# 🏗️ ARQUITECTURA COMPLETA - APIM + AZURE FUNCTIONS

## 📋 DIAGRAMA DE ARQUITECTURA ACTUAL

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           🌐 FRONTEND (React/Vue/Angular)                      │
│                                                                                 │
│  ┌─────────────────┐    JWT Token     ┌─────────────────────────────────────┐  │
│  │  Login Page     │──────────────────▶│  Azure AD Authentication            │  │
│  │  - Username     │                   │  - Validates credentials            │  │
│  │  - Password     │                   │  - Issues JWT with claims           │  │
│  │  - Submit       │                   │  - Returns token to frontend        │  │
│  └─────────────────┘                   └─────────────────────────────────────┘  │
│                                                         │                        │
│                                          JWT Token      │                        │
│                                          with claims    ▼                        │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                    APP FUNCTIONALITY                                   │   │
│  │  - Dashboard                                                            │   │
│  │  - Menu Navigation                                                      │   │
│  │  - Business Forms                                                       │   │
│  │  - Data Tables                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────────┘
                                              │
                                              │ HTTPS Request
                                              │ Authorization: Bearer eyJ0eXAi...
                                              ▼
┌─────────────────────────────────────────────────────────────────────────────────┐
│                    🛡️ AZURE API MANAGEMENT (APIM)                              │
│                                                                                 │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                         🔐 INBOUND POLICIES                             │   │
│  │                                                                         │   │
│  │  1️⃣ validate-jwt:                                                       │   │
│  │     ├── Validates token signature against Azure AD                     │   │
│  │     ├── Checks expiration date                                          │   │
│  │     ├── Verifies audience (3bebf8d8-b4a1-4d2d-a31b-f44ad0d3b1cd)      │   │
│  │     └── Validates issuer (Azure AD tenant)                             │   │
│  │                                                                         │   │
│  │  2️⃣ set-header X-User-Id:                                               │   │
│  │     └── Extracts 'oid' claim → X-User-Id header                        │   │
│  │                                                                         │   │
│  │  3️⃣ set-header X-User-Name:                                             │   │
│  │     └── Extracts 'name' claim → X-User-Name header                     │   │
│  │                                                                         │   │
│  │  4️⃣ set-header X-User-Email:                                            │   │
│  │     └── Extracts 'preferred_username' → X-User-Email header            │   │
│  │                                                                         │   │
│  │  5️⃣ set-header X-User-Groups:                                           │   │
│  │     └── Extracts 'groups' claims → X-User-Groups header (comma-sep)    │   │
│  │                                                                         │   │
│  │  6️⃣ set-header X-User-Scopes:                                           │   │
│  │     └── Extracts 'scp' claim → X-User-Scopes header                    │   │
│  │                                                                         │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                              │                                 │
│                                              │ Clean Request                   │
│                                              │ (No JWT, only headers)         │
│                                              ▼                                 │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                      🔄 ROUTING LOGIC                                   │   │
│  │                                                                         │   │
│  │  /api/auth/*   → Function.Blending.Auth                                │   │
│  │  /api/core/*   → Function.Blending.Core                                │   │
│  │  /api/upload/* → Function.Blending.Upload                              │   │
│  │  /api/opt/*    → Function.Blending.Opt                                 │   │
│  │                                                                         │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────────┘
                                              │
                                              │ Headers Only
                                              │ X-User-Id: a1b2c3d4-e5f6...
                                              │ X-User-Name: Juan Pérez
                                              │ X-User-Email: juan@company.com
                                              │ X-User-Groups: Admin,Calidad
                                              │ X-User-Scopes: blending.read
                                              ▼
┌─────────────────────────────────────────────────────────────────────────────────┐
│                        ☁️ AZURE FUNCTIONS                                      │
│                                                                                 │
│  ┌───────────────────┐  ┌───────────────────┐  ┌───────────────────┐          │
│  │ 📋 Auth Function  │  │ 🔧 Core Function  │  │ 📁 Upload Function│          │
│  │                   │  │                   │  │                   │          │
│  │ 🎯 SPECIALIZATION │  │ 🎯 SPECIALIZATION │  │ 🎯 SPECIALIZATION │          │
│  │ Menu & UI Service │  │ Business Logic    │  │ File Management   │          │
│  │                   │  │                   │  │                   │          │
│  │ ✅ Responsibilities│  │ ✅ Responsibilities│  │ ✅ Responsibilities│          │
│  │ ├─ Generate menus │  │ ├─ CRUD operations│  │ ├─ File upload    │          │
│  │ ├─ User roles     │  │ ├─ Data validation│  │ ├─ File processing│          │
│  │ ├─ UI config      │  │ ├─ Business rules │  │ ├─ File storage   │          │
│  │ ├─ Permissions    │  │ ├─ Database ops   │  │ └─ File metadata  │          │
│  │ └─ Navigation     │  │ └─ Auditoría      │  │                   │          │
│  │                   │  │                   │  │                   │          │
│  │ 🚫 NO Auth Code   │  │ 🚫 NO Auth Code   │  │ 🚫 NO Auth Code   │          │
│  │ ├─ No JWT validation│  │ ├─ No JWT validation│  │ ├─ No JWT validation│          │
│  │ ├─ No token parsing│  │ ├─ No token parsing│  │ ├─ No token parsing│          │
│  │ └─ Only reads headers│  │ └─ Only reads headers│  │ └─ Only reads headers│          │
│  │                   │  │                   │  │                   │          │
│  │ 📤 CODE EXAMPLE:  │  │ 📤 CODE EXAMPLE:  │  │ 📤 CODE EXAMPLE:  │          │
│  │ var userId =      │  │ var userId =      │  │ var userId =      │          │
│  │   GetHeader(      │  │   _currentUser    │  │   GetHeader(      │          │
│  │   "X-User-Id");   │  │   .GetUserId(req);│  │   "X-User-Id");   │          │
│  │                   │  │                   │  │                   │          │
│  └───────────────────┘  └───────────────────┘  └───────────────────┘          │
└─────────────────────────────────────────────────────────────────────────────────┘
```

## 🛡️ PROTECCIÓN DE RUTAS POR SCOPES

### 📋 FUNCTION.AUTH - Menu & UI Service
```xml
<!-- APIM Policy para /api/auth/* -->
<required-claims>
  <claim name="scp" match="any">
    <value>blending.read</value>     <!-- Mínimo para ver menús -->
    <value>blending.write</value>    <!-- También puede ver menús -->
    <value>blending.admin</value>    <!-- Admin ve todo -->
  </claim>
</required-claims>

USUARIOS PERMITIDOS:
✅ Lectores    → Ver menús básicos
✅ Operadores  → Ver menús de operación  
✅ Admins      → Ver menús completos
```

### 🔧 FUNCTION.CORE - Business Logic
```xml
<!-- APIM Policy para /api/core/* -->
<choose>
  <when condition="@(context.Request.Method == "GET")">
    <!-- Lectura: scope read o superior -->
    <required-claims>
      <claim name="scp" match="any">
        <value>blending.read</value>
        <value>blending.write</value>
        <value>blending.admin</value>
      </claim>
    </required-claims>
  </when>
  <when condition="@(context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "DELETE")">
    <!-- Escritura: scope write o admin -->
    <required-claims>
      <claim name="scp" match="any">
        <value>blending.write</value>
        <value>blending.admin</value>
      </claim>
    </required-claims>
  </when>
</choose>

USUARIOS PERMITIDOS:
❌ Lectores    → Solo GET (lectura)
✅ Operadores  → GET, POST, PUT (operaciones)
✅ Admins      → GET, POST, PUT, DELETE (todo)
```

### 📁 FUNCTION.UPLOAD - File Management (Admin Only)
```xml
<!-- APIM Policy para /api/upload/* -->
<required-claims>
  <claim name="scp" match="all">
    <value>blending.admin</value>    <!-- Solo administradores -->
  </claim>
</required-claims>

USUARIOS PERMITIDOS:
❌ Lectores    → Sin acceso
❌ Operadores  → Sin acceso  
✅ Admins      → Acceso completo
```

## 🎯 FUNCTION.AUTH - SPECIALIZATION DETAIL

```
┌─────────────────────────────────────────────────────────────────┐
│                   📋 FUNCTION.AUTH ARCHITECTURE                 │
│                                                                 │
│  🎯 CORE PURPOSE: Menu & UI Configuration Service              │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                    ENDPOINTS                            │   │
│  │                                                         │   │
│  │  GET /api/auth/menu                                     │   │
│  │  ├── Reads X-User-Groups header                        │   │
│  │  ├── Maps groups to business roles                     │   │
│  │  ├── Generates dynamic menu structure                  │   │
│  │  └── Returns personalized navigation                   │   │
│  │                                                         │   │
│  │  GET /api/auth/permissions                              │   │
│  │  ├── Reads X-User-Scopes header                        │   │
│  │  ├── Maps scopes to UI permissions                     │   │
│  │  └── Returns what user can see/do                      │   │
│  │                                                         │   │
│  │  GET /api/auth/profile                                  │   │
│  │  ├── Reads X-User-* headers                            │   │
│  │  ├── Builds user profile object                        │   │
│  │  └── Returns user display information                  │   │
│  │                                                         │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                  BUSINESS LOGIC                         │   │
│  │                                                         │   │
│  │  🔄 Group → Role Mapping:                               │   │
│  │  ├── "Admin" group       → Admin role                  │   │
│  │  ├── "Calidad" group     → Quality Control role        │   │
│  │  ├── "Logistica" group   → Logistics role              │   │
│  │  └── "Operador" group    → Operator role               │   │
│  │                                                         │   │
│  │  📋 Menu Generation Logic:                              │   │
│  │  ├── Admin → All modules visible                       │   │
│  │  ├── Calidad → Quality + Reports modules              │   │
│  │  ├── Logistica → Logistics + Planning modules         │   │
│  │  └── Operador → Basic operations only                 │   │
│  │                                                         │   │
│  │  ⚙️ UI Configuration:                                   │   │
│  │  ├── Feature flags per role                            │   │
│  │  ├── Button visibility rules                           │   │
│  │  ├── Field access permissions                          │   │
│  │  └── Action availability                               │   │
│  │                                                         │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## ✅ VENTAJAS DE ESTA ARQUITECTURA

### 🎯 SEPARATION OF CONCERNS
- **APIM**: Solo autenticación y autorización
- **Function.Auth**: Solo menús y configuración UI  
- **Function.Core**: Solo lógica de negocio
- **Function.Upload**: Solo gestión de archivos

### 🚀 PERFORMANCE
- **Zero JWT processing** en Functions
- **Headers ultra-rápidos** para leer
- **APIM optimizado** por Microsoft
- **Cacheable** responses

### 🔒 SECURITY
- **Centralized auth** en APIM
- **No JWT leaks** a Functions
- **Scope-based** protection
- **Enterprise-grade** security

### 🛠️ MAINTAINABILITY  
- **Single responsibility** per Function
- **Clean interfaces** entre componentes
- **Easy testing** (mock headers)
- **Independent scaling** por Function

¿Te queda claro cómo Function.Auth se especializa solo en menús/UI y cómo APIM protege todo?
