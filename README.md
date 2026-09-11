# Sistema de Trámites Ciudadanos — .NET + SQLite

Sistema generado a partir del documento **"SIS III — DER, Arquitectura y Casos de Uso"**,
adaptado de la propuesta original (Laravel + MySQL) a **ASP.NET Core MVC + Entity
Framework Core + SQLite**, para trabajar con **VSCode**.

## Arquitectura (3 capas, igual que el documento original)

```
SistemaTramites.sln
└── src/
    ├── SistemaTramites.Domain          → Entidades del DER (POCOs)
    ├── SistemaTramites.Infrastructure  → Capa de datos: DbContext (EF Core + SQLite)
    ├── SistemaTramites.Application     → Capa de aplicación: lógica de negocio
    │                                      (registrar solicitud, registrar pago)
    └── SistemaTramites.Web             → Capa de presentación: ASP.NET Core MVC
```

Esto reemplaza la arquitectura original (Cliente → Presentación → Aplicación Laravel →
MySQL) manteniendo las mismas 3 capas, pero 100% en .NET y con SQLite como base de datos
(archivo único `tramites.db`, sin necesidad de instalar un motor de base de datos aparte).

## Entidades (fiel al DER del documento)

Ciudadano, TipoTramite, Usuario, **Trámite** (entidad central), Cita, DocumentoAdjunto,
Notificación, Protocolo y Pago — con las mismas relaciones y cardinalidades del diagrama
(1..N solicita, 1..N clasifica, 1..N atiende, 1..N agenda, 1..N incluye, 1..N envía,
0..1 archiva, 1..N genera, 1..N registra).

## Casos de uso implementados como flujos de extremo a extremo

Los dos diagramas de secuencia del documento están implementados como pantallas dedicadas
(menú superior "Registrar solicitud" / "Registrar pago"), además del CRUD administrativo
completo de las 9 entidades (menú "Catálogos" y resto de opciones):

1. **Registrar solicitud de trámite** (RF-01, RF-03): Ciudadano → Ventanilla → Sistema.
   Registra el trámite, calcula el arancel según el tipo de trámite y entrega el código
   de seguimiento y el monto a pagar.
2. **Registrar pago y emitir recibo** (RF-05, RF-09): Ciudadano → Caja → Sistema.
   Registra el pago, cambia el estado del trámite a "Pagado", emite el número de recibo
   y genera automáticamente una notificación al ciudadano.

## Autenticación y roles (ASP.NET Core Identity)

El sistema ahora exige iniciar sesión para entrar a cualquier pantalla. Se agregó
**ASP.NET Core Identity** sobre la misma base SQLite, con 6 roles y una cuenta de
prueba por cada uno (contraseña para todas: `Notaria2026`):

| Usuario      | Rol            |
|--------------|----------------|
| `admin`      | Administrador  |
| `amontes`    | Notario        |
| `mrojas`     | Ventanilla     |
| `lparedes`   | Caja           |
| `cfernandez` | Oficial        |
| `jsalazar`   | Archivo        |

La sección **"Usuarios (roles)"** del menú "Catálogos" (RF-06: gestionar roles y
permisos) está restringida solo a los roles `Administrador` y `Notario`; si otro
usuario intenta entrar ahí, se le redirige a "Acceso denegado".

> Nota de diseño: la cuenta de acceso (login) y el catálogo de negocio **Usuario**
> (que aparece como oficial/cajero dentro de un trámite o pago) son dos cosas
> separadas a propósito — la primera es "quién puede entrar al sistema y con qué
> rol", la segunda es "qué funcionario aparece registrado en un trámite". Es un punto
> natural para seguir evolucionando el sistema (por ejemplo, vincular ambas tablas).

Las contraseñas de las cuentas de prueba son intencionalmente simples para
desarrollo; cámbialas (o elimina el `IdentityDataInitializer` del `Program.cs`)
antes de usar el sistema en un entorno real.

## Requisitos previos

1. **.NET 8 SDK** — descargar de https://dotnet.microsoft.com/download/dotnet/8.0
   (verificar con `dotnet --version`, debe mostrar `8.x.x`)
2. **VSCode** con la extensión **C# Dev Kit** (o al menos "C#" de Microsoft)
3. No se necesita instalar SQLite aparte: EF Core la maneja como un simple archivo.

## Cómo ejecutarlo

### Opción A — desde la terminal (recomendado la primera vez)

```bash
cd SistemaTramites

# 1) Restaurar paquetes NuGet de todos los proyectos
dotnet restore

# 2) Compilar la solución completa
dotnet build

# 3) Ejecutar el proyecto web
cd src/SistemaTramites.Web
dotnet run
```

La consola mostrará algo como `Now listening on: http://localhost:5080`. Abre esa URL
en el navegador. La base de datos SQLite (`tramites.db`) se crea automáticamente en esa
carpeta la primera vez que se ejecuta, con datos de ejemplo (ciudadanos, tipos de trámite
y usuarios ya cargados).

### Opción B — desde VSCode

1. Abre la carpeta `SistemaTramites` en VSCode (`code .`)
2. Espera a que el C# Dev Kit restaure los proyectos automáticamente
3. Presiona **F5** (o "Run and Debug" → ".NET Core Launch (web)") — ya está configurado
   en `.vscode/launch.json` y `.vscode/tasks.json` para compilar y levantar el sitio,
   abriendo el navegador automáticamente.

## Migraciones de EF Core (opcional)

El proyecto usa `Database.EnsureCreated()` al iniciar, así que **no es obligatorio** usar
migraciones para empezar a trabajar. Si más adelante quieres usar migraciones formales
(recomendado si vas a modificar el modelo de datos con el tiempo):

```bash
dotnet tool install --global dotnet-ef   # solo una vez

cd src/SistemaTramites.Web
dotnet ef migrations add InicialSQLite --project ../SistemaTramites.Infrastructure
dotnet ef database update
```

Si haces esto, recuerda quitar o ajustar la llamada a `DbInitializer.Seed()` en
`Program.cs` para no mezclar `EnsureCreated()` con migraciones (son dos estrategias
que no deben combinarse sobre el mismo `DbContext`).

## Estructura de datos de ejemplo (seed)

Al iniciar por primera vez se cargan:
- 3 tipos de trámite (Poder Notarial, Testimonio de Escritura, Certificación de Firma)
- 5 usuarios con distintos roles (Ventanilla, Oficial, Caja, Notario, Archivo)
- 2 ciudadanos de prueba

## Próximos pasos sugeridos

- Adaptar los roles de `Usuario` a un login real (ASP.NET Core Identity) para cubrir
  RF-06 (gestionar roles y permisos) con autenticación real.
- Implementar la exportación a PDF/Excel (RF-12) en un controlador de reportes.
- Conectar el envío real de notificaciones (correo/WhatsApp) en `PagoService`, hoy
  simulado marcando la notificación como "Enviado" directamente.
