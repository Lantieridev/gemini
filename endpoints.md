¡Claro! Aquí tienes una lista detallada de todos los endpoints de tu API, agrupados por funcionalidad (controlador) y explicando qué hace cada uno, quién puede usarlo y qué datos espera.

---

## 🔐 Módulo de Autenticación y Cuentas
Controladores: `AuthController` y `AccountController`

Estos endpoints gestionan el inicio de sesión y el registro de nuevos clientes.

### `POST /api/auth/login`
* **Permisos:** 🟢 **Público**.
* **Descripción:** Es el endpoint principal para iniciar sesión. El usuario envía su email y contraseña. Si las credenciales son correctas, la API devuelve un **Token JWT (JSON Web Token)**, su email y su rol ("Cliente" o el cargo del empleado, ej: "Admin"). Este token deberá ser enviado en las cabeceras (Header `Authorization`) del resto de peticiones que requieran permisos.
* **Cuerpo (Body):** `LoginRequestDTO` (Email, Password).

### `POST /api/account/register`
* **Permisos:** 🟢 **Público**.
* **Descripción:** Permite a un **Cliente** nuevo registrarse. Este endpoint crea *dos* entidades: primero crea el `Cliente` (con nombre, apellido, etc.) y luego crea el `Usuario` asociado (con el email y la contraseña encriptada). Antes de crear, valida que el email, el documento y el teléfono no existan previamente.
* **Cuerpo (Body):** `RegisterClienteDTO` (Email, Password, Nombre, Apellido, Telefono, Documento).

---

## 🏀 Módulo de Reservas (Principal)
Controlador: `ReservaController`

Endpoints centrales para la lógica de negocio: ver disponibilidad, crear y gestionar reservas.

### `GET /api/reserva/complejos`
* **Permisos:** 🟢 **Público**.
* **Descripción:** Devuelve una lista de todos los complejos deportivos disponibles (ID y Nombre). Es el primer paso para que el usuario filtre canchas.
* **Parámetros:** Ninguno.

### `GET /api/reserva/canchas/{complejoId}`
* **Permisos:** 🟢 **Público**.
* **Descripción:** Devuelve una lista de todas las canchas (ID y Nombre) que pertenecen a un complejo específico.
* **Parámetros (URL):** `complejoId` (int).

### `GET /api/reserva/disponibilidad`
* **Permisos:** 🟢 **Público**.
* **Descripción:** Devuelve los horarios *libres* para una cancha en una fecha específica. Comprueba tanto las reservas existentes como los bloqueos administrativos.
* **Parámetros (Query):** `?canchaId= (int)` y `&fecha= (DateOnly, ej: "2025-11-05")`.

### `POST /api/reserva`
* **Permisos:** 🔵 **Cliente** / 🟡 **Empleado** / 🔴 **Admin**.
* **Descripción:** **(Endpoint modificado)** Crea una nueva reserva. Ahora permite reservar **múltiples canchas** en la misma transacción. El servicio valida la disponibilidad hora por hora para *cada* cancha. También calcula el precio total hora por hora, aplicando tarifas diferenciadas (ej. con luz después de las 19:00). Si un Cliente lo usa, solo puede reservar a su propio nombre.
* **Cuerpo (Body):** `CrearReservaDTO` (ClienteId, `List<int> CanchaIds`, Fecha, HoraInicio, HoraFin).

### `GET /api/reserva/cliente/{clienteId}`
* **Permisos:** 🔵 **Cliente** / 🟡 **Empleado** / 🔴 **Admin**.
* **Descripción:** **(Endpoint modificado)** Devuelve el historial de reservas de un cliente, incluyendo los detalles de qué canchas se reservaron. Por seguridad, si el rol es "Cliente", solo podrá ver sus propias reservas (validado contra su token).
* **Parámetros (URL):** `clienteId` (int).

### `PUT /api/reserva/cancelar`
* **Permisos:** 🔵 **Cliente** / 🟡 **Empleado** / 🔴 **Admin**.
* **Descripción:** Cancela una reserva existente (cambia su estado a "Cancelada"). La lógica de negocio impide cancelar si faltan menos de 24 horas para la reserva. Si un Cliente lo usa, solo puede cancelar sus propias reservas.
* **Cuerpo (Body):** `CancelarReservaDTO` (ReservaId, ClienteId, Motivo).

---

## 📊 Módulo de Dashboard (Empleados)
Controlador: `DashboardController`

Endpoints para la visualización de datos y KPIs por parte de los empleados.

### `GET /api/dashboard`
* **Permisos:** 🟡 **Empleado** / 🔴 **Admin**.
* **Descripción:** Es el endpoint principal del dashboard. Devuelve un objeto (`DashboardCompletoDto`) que contiene múltiples métricas, incluyendo:
    * Resumen de KPIs (Ingresos hoy, reservas hoy).
    * Gráfico de Ingresos por período.
    * Gráfico de torta de Estados de Reservas.
    * **Top 10 Canchas Populares** (Ranking por N° de reservas).
    * **Top 10 Clientes Frecuentes** (Ranking por N° de reservas y gasto).
    * Alertas de Stock (si se implementa) y Ocupación por horario.
* **Parámetros (Query):** `FiltrosDashboardDto` (ej: `?ComplejoId=1&PeriodoPredefinido=ultimos_30_dias`). Si no se envía ComplejoId, usa `1` por defecto.

---

## 🛠️ Módulos de Administración (Admin/Empleado)
Controladores: `ClienteController`, `EmpleadoController`, `CanchaController`, `TiposCanchaController`, `ComplejoController`, `AdminUsuarioController`

Estos endpoints se usan para el ABMC (CRUD) de las entidades principales.

### Gestión de Clientes (`/api/admin/clientes`)
* **Permisos:** 🟡 **Empleado** / 🔴 **Admin**.
* `GET /`: Obtiene la lista completa de clientes.
* `GET /{id}`: Obtiene un cliente por ID.
* `POST /`: Crea un nuevo cliente (sin crear usuario).
* `PUT /{id}`: Actualiza los datos de un cliente.
* `DELETE /{id}`: Elimina un cliente (si no tiene reservas asociadas).

### Gestión de Canchas (`/api/cancha`)
* **Permisos:** La mayoría 🟡 **Empleado** / 🔴 **Admin**.
* `GET /`: Obtiene la lista de todas las canchas (Público).
* `GET /{id}`: Obtiene una cancha por ID (Público).
* `POST /`: Crea una nueva cancha (Admin/Empleado).
* `PUT /{id}`: Actualiza los datos de una cancha (Admin/Empleado).
* `DELETE /{id}`: Elimina una cancha (Admin/Empleado).
* `PUT /{id}/activar`: Marca una cancha como "Activa" (Admin/Empleado).
* `PUT /{id}/desactivar`: Marca una cancha como "Inactiva" (Admin/Empleado).

### Gestión de Tipos de Cancha (`/api/tiposcancha`)
* **Permisos:** 🟡 **Empleado** / 🔴 **Admin**.
* `GET /`: Obtiene la lista de tipos de cancha (Ej: "Fútbol 5", "Fútbol 7").
* `GET /{id}`: Obtiene un tipo de cancha por ID.
* `POST /`: Crea un nuevo tipo de cancha.

### Gestión de Complejos (`/api/admin/complejos`)
* **Permisos:** La mayoría 🔴 **Admin**.
* `GET /`: Obtiene la lista de todos los complejos (Público).
* `GET /{id}`: Obtiene un complejo por ID (Público).
* `POST /`: Crea un nuevo complejo (Admin).
* `PUT /{id}`: Actualiza los datos de un complejo (Admin).
* `DELETE /{id}`: Elimina un complejo (Admin).

### Gestión de Empleados (`/api/admin/empleados`)
* **Permisos:** 🔴 **Admin**.
* `GET /`: Obtiene la lista completa de empleados.
* `GET /{id}`: Obtiene un empleado por ID.
* `POST /`: Crea un nuevo empleado.
* `PUT /{id}`: Actualiza los datos de un empleado.
* `DELETE /{id}`: Elimina un empleado.

### Gestión de Usuarios (`/api/admin/usuarios`)
* **Permisos:** 🔴 **Admin**.
* **Descripción:** Permite al Admin gestionar las cuentas de usuario (logins). Se usa principalmente para crear usuarios de tipo "Empleado" o "Admin" y asociarlos a un EmpleadoID, o para gestionar usuarios de Clientes existentes.
* `GET /`: Obtiene la lista de todos los usuarios (logins).
* `GET /{id}`: Obtiene un usuario por ID.
* `POST /`: Crea un nuevo usuario (ej. un login para un empleado).
* `PUT /{id}`: Actualiza un usuario (ej. cambiar email o reasignar roles).
* `DELETE /{id}`: Elimina un usuario (impide que inicie sesión).