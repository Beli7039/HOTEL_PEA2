/* ============================================================
   JS_Reserva.js
   Lógica interactiva de la página "Nueva Reserva":
     A) Toggle cliente existente / nuevo
     B) Autocompletar datos del cliente al seleccionarlo
     C) Autocompletar precio al elegir tipo de habitación
     D) Filtrar habitaciones disponibles por tipo + fechas
     E) Autocompletar piso al elegir habitación
     F) Actualizar el resumen en tiempo real
   ============================================================ */

document.addEventListener('DOMContentLoaded', function () {

    // ============================================================
    // REFERENCIAS A LOS ELEMENTOS DEL DOM
    // Se buscan una sola vez para no repetir getElementById
    // en cada evento (mejora rendimiento y legibilidad).
    // ============================================================
    const fechaEntrada = document.getElementById('fechaEntrada');
    const fechaSalida = document.getElementById('fechaSalida');
    const cantPersonas = document.getElementById('cantPersonas');

    const radiosTipo = document.querySelectorAll('input[name="tipoClienteRadio"]');
    const inputTipoCliente = document.getElementById('inputTipoCliente');
    const bloqueExistente = document.getElementById('bloqueClienteExistente');
    const clienteSelect = document.getElementById('clienteSelect');
    const clienteDni = document.getElementById('clienteDni');
    const clienteNombre = document.getElementById('clienteNombre');
    const clienteTelefono = document.getElementById('clienteTelefono');
    const clienteEmail = document.getElementById('clienteEmail');

    const tipoHabitacion = document.getElementById('tipoHabitacionSelect');
    const precioHabitacion = document.getElementById('precioHabitacion');
    const habitacionSelect = document.getElementById('habitacionSelect');
    const pisoHabitacion = document.getElementById('pisoHabitacion');

    // Elementos del resumen
    const lblHabitacion = document.getElementById('lblHabitacion');
    const lblTipo = document.getElementById('lblTipo');
    const lblEntrada = document.getElementById('lblEntrada');
    const lblSalida = document.getElementById('lblSalida');
    const lblNoches = document.getElementById('lblNoches');
    const lblPrecioNoches = document.getElementById('lblPrecioNoches');
    const lblPersonas = document.getElementById('lblPersonas');
    const lblTotal = document.getElementById('lblTotal');
    const inputCostoTotal = document.getElementById('inputCostoTotal');

    // Variable interna: precio actual en número (para cálculos)
    let precioActual = 0;

    // ============================================================
    // A) TOGGLE CLIENTE EXISTENTE / NUEVO
    // Al cambiar el radio:
    //   - existente → muestra selector, campos readonly
    //   - nuevo     → oculta selector, campos editables
    // ============================================================
    radiosTipo.forEach(function (radio) {
        radio.addEventListener('change', function () {
            const esExistente = this.value === 'existente';

            // Guardar en el hidden para que el controlador sepa qué hacer
            inputTipoCliente.value = esExistente ? 'existente' : 'nuevo';

            // Mostrar/ocultar el bloque del selector
            bloqueExistente.style.display = esExistente ? 'block' : 'none';

            // readonly en los campos según el modo
            [clienteDni, clienteNombre, clienteTelefono, clienteEmail].forEach(function (input) {
                input.readOnly = esExistente;
            });

            // Si es nuevo, limpiar los campos y el selector
            if (!esExistente) {
                clienteSelect.value = '';
                clienteDni.value = '';
                clienteNombre.value = '';
                clienteTelefono.value = '';
                clienteEmail.value = '';
            }
        });
    });

    // ============================================================
    // B) AUTOCOMPLETAR DATOS AL ELEGIR CLIENTE EXISTENTE
    // Los datos vienen como data-attributes en cada <option>.
    // ============================================================
    clienteSelect.addEventListener('change', function () {
        const opcion = this.options[this.selectedIndex];

        clienteDni.value = opcion.dataset.dni || '';
        clienteNombre.value = opcion.dataset.nombre || '';
        clienteTelefono.value = opcion.dataset.telefono || '';
        clienteEmail.value = opcion.dataset.email || '';
    });

    // ============================================================
    // C) AUTOCOMPLETAR PRECIO Y FILTRAR HABITACIONES
    // Al cambiar el tipo de habitación:
    //   - Se lee su precio del data-precio
    //   - Se cargan SOLO las habitaciones disponibles de ese tipo
    // ============================================================
    tipoHabitacion.addEventListener('change', function () {
        // Limpiar precio si no hay tipo seleccionado
        if (!this.value) {
            precioActual = 0;
            precioHabitacion.value = '';
            habitacionSelect.innerHTML = '<option value="">-- Primero elija un tipo --</option>';
            habitacionSelect.disabled = true;
            pisoHabitacion.value = '';
            actualizarResumen();
            return;
        }

        // Buscar el precio del tipo (por Id en el array de habitaciones)
        // Se toma el precio de la primera habitación de ese tipo.
        const habitacionDelTipo = HABITACIONES.find(
            h => String(h.TipoId) === String(this.value) || String(h.Tipo) === String(this.value)
        );

        // Si no encontramos precio, dejamos 0
        precioActual = habitacionDelTipo ? parseFloat(habitacionDelTipo.Precio) : 0;
        precioHabitacion.value = 'S/. ' + precioActual.toFixed(2);

        // Cargar las habitaciones disponibles de ese tipo
        cargarHabitacionesDisponibles(this.value);

        actualizarResumen();
    });

    // ============================================================
    // D) CARGAR HABITACIONES DISPONIBLES
    // Filtra por tipo Y por disponibilidad en el rango de fechas.
    // ============================================================
    function cargarHabitacionesDisponibles(tipoId) {
        // Reset
        habitacionSelect.innerHTML = '<option value="">-- Seleccione habitación --</option>';
        pisoHabitacion.value = '';

        if (!tipoId) {
            habitacionSelect.disabled = true;
            return;
        }

        // Obtener fechas actuales
        const entrada = fechaEntrada.value;
        const salida = fechaSalida.value;

        // Filtrar habitaciones por tipo
        const delTipo = HABITACIONES.filter(function (h) {
            return String(h.TipoId) === String(tipoId) || String(h.Tipo) === String(tipoId);
        });

        // Filtrar las que NO estén ocupadas en el rango
        const disponibles = delTipo.filter(function (h) {
            return !estaOcupada(h.Id, entrada, salida);
        });

        if (disponibles.length === 0) {
            habitacionSelect.disabled = true;
            habitacionSelect.innerHTML = '<option value="">Sin habitaciones disponibles</option>';
            return;
        }

        // Poblar el selector
        habitacionSelect.disabled = false;
        disponibles.forEach(function (h) {
            const opt = document.createElement('option');
            opt.value = h.Id;
            opt.textContent = 'Hab. ' + h.Numero;
            opt.dataset.piso = h.Piso;
            opt.dataset.precio = h.Precio;
            habitacionSelect.appendChild(opt);
        });
    }

    // ============================================================
    // FUNCIÓN AUXILIAR: ¿La habitación está ocupada en el rango?
    // Compara contra RESERVAS_EXISTENTES. Se consideran rangos
    // solapados si: entrada < resSalida && salida > resEntrada.
    // ============================================================
    function estaOcupada(habitacionId, entrada, salida) {
        if (!entrada || !salida) return false;

        const f1 = new Date(entrada);
        const f2 = new Date(salida);

        return RESERVAS_EXISTENTES.some(function (r) {
            // Ignorar la reserva actual (si estamos editando)
            if (r.IdReserva && r.IdReserva === ID_RESERVA_ACTUAL) return false;

            if (String(r.HabitacionId) !== String(habitacionId)) return false;

            const rEntrada = new Date(r.FechaEntrada);
            const rSalida = new Date(r.FechaSalida);

            return f1 < rSalida && f2 > rEntrada;
        });
    }

    // ============================================================
    // E) AUTOCOMPLETAR PISO AL ELEGIR HABITACIÓN
    // ============================================================
    habitacionSelect.addEventListener('change', function () {
        const opcion = this.options[this.selectedIndex];
        pisoHabitacion.value = opcion.dataset.piso || '';

        // Si la habitación tiene otro precio, actualizarlo
        if (opcion.dataset.precio) {
            precioActual = parseFloat(opcion.dataset.precio);
            precioHabitacion.value = 'S/. ' + precioActual.toFixed(2);
        }

        actualizarResumen();
    });

    // ============================================================
    // F) ACTUALIZAR EL RESUMEN
    // Se recalcula todo cada vez que cambia algo relevante.
    // ============================================================
    [fechaEntrada, fechaSalida, cantPersonas].forEach(function (el) {
        el.addEventListener('change', actualizarResumen);
        el.addEventListener('input', actualizarResumen);
    });

    // Cuando cambian las fechas, hay que recargar las habitaciones
    // porque la disponibilidad puede haber cambiado.
    fechaEntrada.addEventListener('change', recargarHabitaciones);
    fechaSalida.addEventListener('change', recargarHabitaciones);

    function recargarHabitaciones() {
        if (tipoHabitacion.value) {
            cargarHabitacionesDisponibles(tipoHabitacion.value);
        }
    }

    function actualizarResumen() {
        // --- Habitación ---
        const opHab = habitacionSelect.options[habitacionSelect.selectedIndex];
        lblHabitacion.textContent = habitacionSelect.value ? opHab.textContent : '--';

        // --- Tipo ---
        const opTipo = tipoHabitacion.options[tipoHabitacion.selectedIndex];
        lblTipo.textContent = tipoHabitacion.value ? opTipo.textContent : '--';

        // --- Entrada / Salida ---
        lblEntrada.textContent = fechaEntrada.value || '--';
        lblSalida.textContent = fechaSalida.value || '--';

        // --- Noches ---
        const noches = calcularNoches(fechaEntrada.value, fechaSalida.value);
        lblNoches.textContent = noches;

        // --- Precio por noche ---
        lblPrecioNoches.textContent = 'S/. ' + precioActual.toFixed(2);

        // --- Personas ---
        lblPersonas.textContent = cantPersonas.value || '1';

        // --- Total ---
        const total = noches * precioActual;
        lblTotal.textContent = 'S/. ' + total.toFixed(2);

        // Guardar el total en el campo oculto para que lo reciba el controlador
        if (inputCostoTotal) {
            inputCostoTotal.value = total.toFixed(2);
        }
    }

    // ============================================================
    // UTILIDAD: CALCULAR NÚMERO DE NOCHES
    // Devuelve 0 si las fechas no son válidas o si la salida
    // es anterior o igual a la entrada.
    // ============================================================
    function calcularNoches(entrada, salida) {
        if (!entrada || !salida) return 0;

        const f1 = new Date(entrada);
        const f2 = new Date(salida);
        const diff = Math.floor((f2 - f1) / (1000 * 60 * 60 * 24));

        return diff > 0 ? diff : 0;
    }

    // Inicializar el resumen al cargar
    actualizarResumen();
});

