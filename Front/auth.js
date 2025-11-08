/**
 * auth.js
 * * Gestiona la autenticación (login, logout, registro)
 * y la comunicación con la API para todo el sitio.
 */

// --- 1. CONFIGURACIÓN CENTRAL ---

// Define la URL de tu API en un solo lugar.
const API_BASE_URL = "http://localhost:5026/api";

/**
 * Objeto AuthService
 * * Abstrae toda la lógica de manejo de localStorage para
 * guardar y recuperar los datos del usuario.
 */
const AuthService = {
    
    /**
     * Guarda los datos de la sesión de login en localStorage.
     * @param {object} loginData - El objeto { token, email, rol, clienteId }
     */
    saveSession: (loginData) => {
        localStorage.setItem('jwtToken', loginData.token);
        localStorage.setItem('userEmail', loginData.email);
        localStorage.setItem('userRol', loginData.rol);
        if (loginData.clienteId) {
            localStorage.setItem('clienteId', loginData.clienteId);
        }
    },

    /**
     * Cierra la sesión, limpia localStorage y redirige al Home.
     */
    logout: () => {
        localStorage.removeItem('jwtToken');
        localStorage.removeItem('userEmail');
        localStorage.removeItem('userRol');
        localStorage.removeItem('clienteId');
        // Redirige al home
        window.location.href = 'index.html'; 
    },

    /**
     * Verifica si el usuario está actualmente logueado.
     * @returns {boolean} True si hay un token, false si no.
     */
    isLoggedIn: () => {
        return !!localStorage.getItem('jwtToken');
    },

    /**
     * Devuelve el token JWT.
     * @returns {string|null} El token o null.
     */
    getToken: () => {
        return localStorage.getItem('jwtToken');
    },

    /**
     * Devuelve todos los datos del usuario guardados.
     * @returns {object} Un objeto con { email, rol, clienteId }
     */
    getUserData: () => {
        return {
            email: localStorage.getItem('userEmail'),
            rol: localStorage.getItem('userRol'),
            clienteId: localStorage.getItem('clienteId')
        };
    },

    /**
     * Llama a la API para registrar un nuevo cliente.
     * @param {object} dto - El DTO de RegisterClienteDTO
     * @returns {Promise<object>} El resultado de la API.
     */
    register: async (dto) => {
        return await apiFetch('/account/register', {
            method: 'POST',
            body: JSON.stringify(dto)
        }, false); // No enviar token para registrarse
    },

    /**
     * Llama a la API para iniciar sesión.
     * Si tiene éxito, guarda la sesión y redirige.
     * @param {string} email 
     * @param {string} password
     */
    login: async (email, password) => {
        // Llama a apiFetch sin token (es la única ruta que lo permite)
        const data = await apiFetch('/auth/login', {
            method: 'POST',
            body: JSON.stringify({ email, password })
        }, false); // 'false' indica que NO envíe token de auth

        // Asumiendo que hiciste el cambio de añadir 'clienteId' a LoginResponseDTO
        const loginData = {
            token: data.token,
            email: data.email,
            rol: data.rol,
            clienteId: data.clienteId 
        };

        // Guarda la sesión
        AuthService.saveSession(loginData);

        // Redirige según el ROL
        if (loginData.rol === 'Admin' || loginData.rol === 'Empleado') {
            window.location.href = 'empleado.html'; // Al panel de admin
        } else {
            window.location.href = 'index.html'; // Al home
        }
    }
};

// --- 2. COMUNICACIÓN GLOBAL CON API ---

/**
 * Función global para hacer peticiones a la API.
 * Automáticamente añade el Header de Autorización.
 * * @param {string} endpoint - El endpoint de la API (ej. "/reserva/complejos")
 * @param {object} options - Opciones de Fetch (method, body, etc.)
 * @param {boolean} [sendAuth=true] - (Opcional) Si es false, no envía el token (para Login/Register)
 * @returns {Promise<any>} El JSON de la respuesta.
 * @throws {Error} Si la respuesta de la API no es "ok".
 */
async function apiFetch(endpoint, options = {}, sendAuth = true) {
    
    // Configura los headers por defecto
    const headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        ...options.headers // Permite sobreescribir headers si es necesario
    };

    // Añade el token JWT si estamos logueados y 'sendAuth' es true
    if (sendAuth && AuthService.isLoggedIn()) {
        headers['Authorization'] = `Bearer ${AuthService.getToken()}`;
    }

    // Construye la petición
    const requestOptions = {
        ...options,
        headers: headers
    };

    const response = await fetch(`${API_BASE_URL}${endpoint}`, requestOptions);

    // --- Manejo de Errores ---

    // Si la respuesta es 401 (No autorizado) o 403 (Prohibido),
    // probablemente el token expiró. Cerramos sesión.
    if ((response.status === 401 || response.status === 403) && sendAuth) {
        AuthService.logout();
        throw new Error('Sesión expirada o no autorizada.');
    }

    if (!response.ok) {
        let errorMessage;
        try {
            const errorData = await response.json();
            errorMessage = errorData.message || 'Error desconocido en la API.';
        } catch (e) {
            errorMessage = `Error ${response.status}: ${response.statusText}`;
        }
        throw new Error(errorMessage);
    }
    
    if (response.status === 204) {
        return null; 
    }

    return await response.json();
}

// --- 3. PROTECCIÓN DE PÁGINAS ---

/**
 * Protege una página, verificando el rol del usuario.
 * @param {Array<string>} rolesPermitidos - Lista de roles que pueden ver la página (ej. ['Admin', 'Empleado'])
 */
function protegerPagina(rolesPermitidos = []) {
    const usuario = AuthService.getUserData();

    if (!AuthService.isLoggedIn()) {
        // 1. No está logueado
        window.location.href = 'login.html';
        throw new Error('Acceso denegado. No autenticado.'); // Detiene ejecución
    }

    if (rolesPermitidos.length > 0 && !rolesPermitidos.includes(usuario.rol)) {
        // 2. Está logueado, pero no tiene el rol correcto
        window.location.href = 'index.html'; // Lo saca al home
        throw new Error('Acceso denegado. Rol no autorizado.'); // Detiene ejecución
    }

    // 3. Está logueado y tiene el rol correcto
    console.log(`Acceso permitido. Rol: ${usuario.rol}`);
}