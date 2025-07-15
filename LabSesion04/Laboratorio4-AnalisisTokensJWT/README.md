# 🧪 LABORATORIO 4: ANÁLISIS AVANZADO DE TOKENS JWT Y DEBUGGING

⏱️ **Duración**: 15 minutos  
🎯 **Objetivo**: Analizar tokens JWT en profundidad y implementar debugging avanzado para .NET 9  
🔧 **Tecnologías**: .NET 9, ASP.NET Core MVC, Microsoft.Identity.Web, Azure AD, JWT.io  

## 📋 Prerrequisitos

- **Laboratorio 0** completado exitosamente
- **Laboratorio 1** completado exitosamente 
- **Laboratorio 2** completado exitosamente
- **Laboratorio 3** completado exitosamente
- Información de Azure AD configurada y guardada
- Usuario autenticado con tokens JWT válidos

## 📝 Objetivos de Aprendizaje

- ✅ Analizar tokens JWT en profundidad (Access, ID, Refresh)
- ✅ Implementar debugging avanzado de autenticación
- ✅ Integrar con JWT.io para análisis externo
- ✅ Crear herramientas de copia y análisis de claims
- ✅ Validar estructura y contenido de tokens JWT
- ✅ Comprender el ciclo de vida de tokens OAuth 2.0

## 🚀 Pasos del Laboratorio

### Paso 1: Crear Vista de Análisis de Tokens (10 minutos)

La vista principal `TokenInfo.cshtml` incluye:

#### **Funcionalidades Principales:**
- **Carga Dinámica**: Información de tokens cargada via JavaScript
- **Análisis Completo**: Access Token, ID Token, Refresh Token
- **Claims Detallados**: Tabla interactiva con descripciones
- **Modal de Token**: Visualización completa del JWT
- **Integración JWT.io**: Análisis externo de tokens

#### **Características de la Interfaz:**
- **Spinner de Carga**: Mientras obtiene información de tokens
- **Cards Responsivos**: Información del usuario y estado de tokens
- **Tabla Interactiva**: Claims con funcionalidad de copia
- **Modal Completo**: Token JWT completo con advertencias de seguridad

### Paso 2: Testing de Análisis de Tokens (5 minutos)

#### **Secuencia de Testing:**

1. **🔑 Autenticarse en la aplicación** si no está autenticado
2. **🔍 Navegar a "Análisis de Tokens"** desde el menú
3. **📊 Verificar que carga correctamente:**
   - Información del usuario presente
   - Estado de tokens (Access, ID, Refresh)
   - Tabla de claims poblada con descripciones
4. **🔗 Probar funcionalidad de copia:**
   - Click en cualquier valor de claim
   - Verificar que se copia al portapapeles
   - Verificar feedback visual (cambio de color)
5. **🎯 Probar vista de token completo:**
   - Click en "Ver ID Token Completo"
   - Verificar que abre modal
   - Verificar que muestra token completo
6. **📋 Probar copia de token completo:**
   - Click en "Copiar Token"
   - Verificar que se copia al portapapeles
7. **🌐 Probar análisis en JWT.io:**
   - Click en "Analizar en JWT.io"
   - Verificar que abre jwt.io en nueva pestaña
   - Pegar token y analizar estructura

## 🔍 Análisis en JWT.io

### **Elementos a verificar en jwt.io:**

#### **✅ Header:**
- `alg: RS256` (algoritmo de firma)
- `typ: JWT` (tipo de token)
- `kid: key ID` para verificación

#### **✅ Payload (Claims importantes):**
- `iss`: https://login.microsoftonline.com/[tenant]/v2.0
- `aud`: [su-client-id]
- `sub`: [unique-user-identifier]
- `name`: [nombre-del-usuario]
- `preferred_username`: [email-del-usuario]
- `exp`: [timestamp-expiracion]
- `iat`: [timestamp-emision]

#### **✅ Signature:**
- Aparecerá como "Invalid" (normal, no tenemos la clave pública)
- Verificar que la estructura es correcta

## 🎨 Funcionalidades de la Interfaz

### **Vista Principal de Análisis**
- **Información del Usuario**: Nombre, autenticación, tipo de auth, total de claims
- **Estado de Tokens**: Presencia de Access, ID y Refresh tokens
- **Claims Detallados**: Tabla con numeración, tipo, valor y descripción
- **Descripciones de Claims**: Explicación de cada claim estándar

### **Modal de Token Completo**
- **Advertencia de Seguridad**: Información sobre tokens en desarrollo
- **Instrucciones**: Pasos para usar JWT.io
- **Textarea**: Token completo en formato monospace
- **Botones de Acción**: Copiar token y abrir JWT.io

### **Funcionalidad de Copia**
- **Copia Individual**: Click en cualquier claim para copiar
- **Feedback Visual**: Cambio de color al copiar
- **Copia de Token**: Botón para copiar token completo

## 📊 Controlador Extendido

### **Métodos Implementados:**
- `TokenInfo()`: Vista principal de análisis
- `GetTokenInfo()`: API para obtener información de tokens
- `GetFullIdToken()`: API para obtener token completo
- `TestAuthentication()`: Herramientas de testing
- `ClaimsAnalysis()`: Análisis avanzado de claims

### **Manejo de Errores:**
- Try-catch para obtención de tokens
- Mensajes de error descriptivos
- Fallback para tokens no disponibles

## 🔧 Configuración y Ejecución

### **Compilar y Ejecutar:**
```bash
# Navegar al directorio del proyecto
cd LabSesion04/Laboratorio4-AnalisisTokensJWT/DevSeguroWebApp

# Compilar proyecto
dotnet build

# Ejecutar aplicación
dotnet run
```

### **Verificar la Aplicación:**
- ✅ Aplicación ejecutándose en: `https://localhost:7001`
- ✅ Aceptar advertencia de certificado (desarrollo local)
- ✅ Verificar página de inicio del laboratorio 4

## 📁 Estructura del Proyecto

```
DevSeguroWebApp/
├── Controllers/
│   └── AccountController.cs       # Controlador con análisis de tokens
├── Views/
│   ├── Account/
│   │   ├── TokenInfo.cshtml        # Vista principal de análisis
│   │   ├── Profile.cshtml          # Vista de perfil
│   │   ├── TestAuthentication.cshtml # Testing
│   │   └── ClaimsAnalysis.cshtml   # Análisis de claims
│   ├── Home/
│   │   └── Index.cshtml            # Página principal Lab 4
│   └── Shared/
│       └── _Layout.cshtml          # Layout con navegación
├── wwwroot/                        # Recursos estáticos
├── appsettings.json               # Configuración Azure AD
├── Program.cs                     # Configuración de la aplicación
└── DevSeguroWebApp.csproj         # Archivo de proyecto
```

## 🔒 Consideraciones de Seguridad

### **Advertencias Importantes:**
- **Tokens en Desarrollo**: Solo mostrar tokens completos en desarrollo
- **No Exposición en Producción**: Nunca exponer tokens en producción
- **Clave Pública**: Signature aparecerá como "Invalid" sin la clave pública
- **Manejo Seguro**: Tokens se obtienen del contexto de autenticación

### **Mejores Prácticas:**
- Validación de tokens automática
- Manejo de errores robusto
- Feedback visual para acciones del usuario
- Integración segura con herramientas externas

## 🛠️ Herramientas de Debugging

### **Funcionalidades Implementadas:**
- **Análisis de Tokens**: Información detallada de JWT
- **Testing de Autenticación**: Herramientas de debugging
- **Análisis de Claims**: Categorización de claims
- **Copia de Claims**: Funcionalidad de portapapeles
- **Integración JWT.io**: Análisis externo

### **Descripciones de Claims:**
- `sub`: Subject - Identificador único del usuario
- `name`: Nombre completo del usuario
- `preferred_username`: Nombre de usuario preferido (email)
- `iss`: Issuer - Quien emitió el token
- `aud`: Audience - Para quién está destinado el token
- `exp`: Expiration - Cuándo expira el token
- `iat`: Issued At - Cuándo fue emitido el token

## ✅ Resultado Esperado

Al completar este laboratorio, tendrás:

- **Vista de análisis de tokens** funcionando completamente
- **Información detallada** de usuario y tokens visible
- **Funcionalidad de copia** al portapapeles operativa
- **Modal de token completo** mostrando JWT completo
- **Integración con JWT.io** funcionando
- **Claims con descripciones** explicativas
- **Comprensión completa** de la estructura JWT
- **Herramientas de debugging** funcionales

## 🐛 Troubleshooting

### **Problema: No se cargan los tokens**
**Solución**: Verificar que el usuario esté autenticado y que los tokens estén disponibles

### **Problema: Error al copiar al portapapeles**
**Solución**: Verificar que el navegador soporte la API de clipboard y que sea HTTPS

### **Problema: Modal no se abre**
**Solución**: Verificar que Bootstrap esté cargado correctamente

### **Problema: JWT.io no funciona**
**Solución**: Verificar que se permite abrir nuevas pestañas en el navegador

## 📈 Funcionalidades Adicionales

### **Más Allá del Documento:**
- **Análisis de Claims**: Categorización avanzada
- **Testing de Autenticación**: Herramientas de debugging
- **Interfaz Moderna**: Bootstrap 5 y Font Awesome
- **Navegación Mejorada**: Menús contextuales
- **Feedback Visual**: Indicadores de estado

## 🔄 Próximos Pasos

- **Implementar autorización** basada en roles
- **Análisis de Refresh Tokens** más detallado
- **Validación de firma** con clave pública
- **Monitoreo de expiración** de tokens
- **Logging de eventos** de autenticación

---

**Resultado esperado**: Herramientas completas de análisis de tokens JWT funcionando perfectamente con interfaz moderna y debugging avanzado. 