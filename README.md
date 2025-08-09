# Sistema de Ejecución de Queue de Tareas

## Descripción
Sistema desarrollado para la empresa SaM (Software a Medida) que permite la ejecución automatizada de tareas en una cola de forma sincrónica. El sistema maneja el procesamiento de una tarea a la vez y prioriza la ejecución en función de las necesidades del usuario.

## Características Principales

### Gestión de Tareas
- ✅ Crear tareas con prioridad (Alta, Media, Baja)
- ✅ Definir fecha de ejecución programada
- ✅ Estados: Pendiente, En Proceso, Finalizada, Fallida
- ✅ Re-ejecución manual de tareas fallidas
- ✅ Priorización automática por nivel de prioridad

### Queue de Tareas
- ✅ Cola con prioridades implementada
- ✅ Procesamiento FIFO dentro de cada nivel de prioridad
- ✅ Espera de 30 segundos entre tareas
- ✅ Worker en segundo plano para gestión

### Ejecución de Tareas
- ✅ Procesamiento sincrónico (una tarea a la vez)
- ✅ Tiempos de ejecución variables simulados
- ✅ Prevención de bloqueo de interfaz
- ✅ Protección contra modificación/eliminación durante ejecución

### Monitoreo y Notificación
- ✅ Panel de monitoreo en tiempo real
- ✅ Notificaciones de estado de tareas
- ✅ Logs detallados de ejecución
- ✅ Historial completo de tareas

## Arquitectura del Sistema

### Patrón de Diseño
El sistema sigue los principios SOLID y utiliza una arquitectura en capas:

- **Data Layer**: Entidades y contexto de Entity Framework
- **Repository Layer**: Acceso a datos con patrón Repository
- **Business Layer**: Lógica de negocio y servicios
- **MVC Layer**: Controladores y vistas

### Componentes Principales

#### Entidades
- `Task`: Modelo principal de tareas
- `TaskQueue`: Gestión de cola de tareas
- `TaskLog`: Logs de ejecución
- `Notification`: Sistema de notificaciones

#### Servicios
- `TaskBusiness`: Lógica de negocio para tareas
- `QueueService`: Gestión de la cola de procesamiento
- `TaskExecutionService`: Ejecución de tareas
- `NotificationService`: Sistema de notificaciones

#### Controladores
- `TasksController`: CRUD de tareas
- `QueueController`: Gestión de cola y procesamiento
- `DashboardController`: Panel de monitoreo

## Instalación y Configuración

### Requisitos
- .NET Framework 4.8
- SQL Server
- Visual Studio 2019 o superior

### Pasos de Instalación
1. Clonar el repositorio
2. Restaurar paquetes NuGet
3. Configurar cadena de conexión en `Web.config`
4. Ejecutar migraciones de Entity Framework
5. Compilar y ejecutar el proyecto

### Configuración de Base de Datos
```xml
<connectionStrings>
  <add name="G4ProyectoFinalDBEntities" 
       connectionString="Data Source=.;Initial Catalog=G4ProyectoFinalDB;Integrated Security=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

## Uso del Sistema

### Dashboard
- Vista general del estado del sistema
- Resumen de tareas por estado
- Tareas pendientes y fallidas
- Acceso rápido a funcionalidades

### Gestión de Tareas
1. **Crear Tarea**: Definir título, descripción, prioridad y fecha
2. **Editar Tarea**: Modificar solo tareas no en proceso
3. **Eliminar Tarea**: Solo tareas no en proceso o finalizadas
4. **Encolar Tarea**: Agregar manualmente a la cola

### Cola de Procesamiento
1. **Iniciar Procesador**: Activar el worker en segundo plano
2. **Detener Procesador**: Pausar el procesamiento
3. **Procesar Siguiente**: Ejecutar tarea manualmente
4. **Monitorear Estado**: Ver tareas en cola y en proceso

### Historial y Logs
- Ver historial completo de ejecuciones
- Detalles de logs por tarea
- Reintentar tareas fallidas
- Análisis de rendimiento

## Funcionalidades Técnicas

### Priorización de Tareas
- **Alta**: Procesamiento inmediato
- **Media**: Procesamiento normal
- **Baja**: Procesamiento con menor prioridad

### Estados de Tareas
- **Pendiente**: Tarea creada, esperando procesamiento
- **En Proceso**: Tarea siendo ejecutada
- **Finalizada**: Tarea completada exitosamente
- **Fallida**: Tarea falló durante ejecución

### Sistema de Logs
- Registro de inicio y fin de ejecución
- Mensajes de error detallados
- Duración de ejecución
- Historial completo de intentos

## API Endpoints

### Tasks Controller
- `GET /Tasks` - Listar tareas
- `POST /Tasks/Create` - Crear tarea
- `POST /Tasks/Enqueue/{id}` - Encolar tarea
- `POST /Tasks/Retry/{id}` - Reintentar tarea

### Queue Controller
- `GET /Queue` - Estado de la cola
- `POST /Queue/StartProcessor` - Iniciar procesador
- `POST /Queue/StopProcessor` - Detener procesador
- `POST /Queue/ProcessNext` - Procesar siguiente tarea

### Dashboard Controller
- `GET /Dashboard` - Panel principal
- `GET /Dashboard/Summary` - Resumen estadístico

## Monitoreo y Mantenimiento

### Logs del Sistema
- Logs de ejecución en base de datos
- Logs de errores en Debug Output
- Notificaciones por email (configurable)

### Rendimiento
- Procesamiento asíncrono de cola
- Timeouts configurables
- Manejo de errores robusto

### Escalabilidad
- Arquitectura preparada para múltiples workers
- Separación de responsabilidades
- Interfaces extensibles

## Troubleshooting

### Problemas Comunes
1. **Tarea no se procesa**: Verificar estado del procesador
2. **Error de conexión**: Revisar cadena de conexión
3. **Tarea falla repetidamente**: Revisar logs de error
4. **Procesador no inicia**: Verificar permisos y configuración

### Logs de Debug
Los logs se pueden encontrar en:
- Output de Visual Studio
- Base de datos (tabla TaskLog)
- Event Viewer de Windows

## Contribución
Para contribuir al proyecto:
1. Fork del repositorio
2. Crear rama feature
3. Implementar cambios
4. Crear Pull Request

## Licencia
Este proyecto es propiedad de SaM (Software a Medida).

## Contacto
Para soporte técnico o consultas, contactar al equipo de desarrollo de SaM. 