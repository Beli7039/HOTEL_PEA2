/* ============================================================
   JS_Reserva.js
   Lógica interactiva de la página "Nueva Reserva":
     A) Toggle cliente existente / nuevo
     B) Autocompletar datos del cliente al seleccionarlo
     C) Autocompletar precio al elegir tipo de habitación
     D) Filtrar habitaciones disponibles por tipo + fechas
     E) Autocompletar piso al elegir habitación
     F) Actualizar el resumen en tiempo real
     G) Inicialización al cargar la página
   ============================================================ */

document.addEventListener('DOMContentLoaded', function () {

    // ============================================================
    // REFERENCIAS A LOS ELEMENTOS DEL DOM
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

    // Resumen
    const lblHabitacion = document.getElementById('lblHabitacion');
    const lblTipo = document.getElementById('lblTipo');
    const lblEntrada = document.getElementById('lblEntrada');
    const lblSalida = document.getElementById('lblSalida');
    const lblNoches = document.getElementById('lblNoches');
    const lblPrecioNoches = document.getElementById('lblPrecioNoches');
    const lblPersonas = document.getElementById('lblPersonas');
    const lblTotal = document.getElementById('lblTotal');
    const inputCostoTotal = document.getElementById('inputCostoTotal');

    // Placeholder del select de cliente
    const PLACEHOLDER_CLIENTE = '0';

    // Precio actual en número
    let precioActual = 0;

    // ============================================================
    // FUNCIONES AUXILIARES
    // ============================================================

    function limpiarCamposCliente() {
        if (clienteDni) clienteDni.value = '';
        if (clienteNombre) clienteNombre.value = '';
        if (clienteTelefono) clienteTelefono.value = '';
        if (clienteEmail) clienteEmail.value = '';
    }

    function autocompletarCliente(opcion) {
        if (!opcion) return;
        if (clienteDni) clienteDni.value = opcion.dataset.dni || '';
        if (clienteNombre) clienteNombre.value = opcion.dataset.nombre || '';
        if (clienteTelefono) clienteTelefono.value = opcion.dataset.telefono || '';
        if (clienteEmail) clienteEmail.value = opcion.dataset.email || '';
    }

    // ============================================================
    // A) TOGGLE CLIENTE EXISTENTE / NUEVO
    // ============================================================
    radiosTipo.forEach(function (radio) {
        radio.addEventListener('change', function () {
            const esExistente = this.value === 'existente';

            inputTipoCliente.value = esExistente ? 'existente' : 'nuevo';
            bloqueExistente.style.display = esExistente ? 'block' : 'none';

            // readonly según modo
            [clienteDni, clienteNombre, clienteTelefono, clienteEmail].forEach(function (input) {
                if (input) input.readOnly = esExistente;
            });

            // Resetear select y limpiar campos en ambos casos
            clienteSelect.value = PLACEHOLDER_CLIENTE;
            limpiarCamposCliente();
        });
    });

    // ============================================================
    // B) AUTOCOMPLETAR DATOS AL ELEGIR CLIENTE
    // ============================================================
    clienteSelect.addEventListener('change', function () {
        if (this.value === PLACEHOLDER_CLIENTE) {
            limpiarCamposCliente();
            return;
        }
        autocompletarCliente(this.options[this.selectedIndex]);
    });

    // ============================================================
    // C) AUTOCOMPLETAR PRECIO AL ELEGIR TIPO DE HABITACIÓN
    // ============================================================
    tipoHabitacion.addEventListener('change', function () {
        if (!this.value) {
            precioActual = 0;
            precioHabitacion.value = 'S/. 0.00';
            habitacionSelect.innerHTML = '<option value="">-- Primero elija un tipo --</option>';
            habitacionSelect.disabled = true;
            pisoHabitacion.value = '';
            actualizarResumen();
            return;
        }

        // Buscar precio en HABITACIONES
        const habitacionDelTipo = HABITACIONES.find(
            h => String(h.TipoId) === String(this.value) || String(h.Tipo) === String(this.value)
        );

        precioActual = habitacionDelTipo ? parseFloat(habitacionDelTipo.Precio) : 0;
        precioHabitacion.value = 'S/. ' + precioActual.toFixed(2);

        cargarHabitacionesDisponibles(this.value);
        actualizarResumen();
    });

    // ============================================================
    // D) CARGAR HABITACIONES DISPONIBLES
    // ============================================================
    function cargarHabitacionesDisponibles(tipoId) {
        habitacionSelect.innerHTML = '<option value="">-- Seleccione habitación --</option>';
        pisoHabitacion.value = '';

        if (!tipoId) {
            habitacionSelect.disabled = true;
            return;
        }

        const entrada = fechaEntrada.value;
        const salida = fechaSalida.value;

        const delTipo = HABITACIONES.filter(function (h) {
            return String(h.TipoId) === String(tipoId) || String(h.Tipo) === String(tipoId);
        });

        const disponibles = delTipo.filter(function (h) {
            return !estaOcupada(h.Id, entrada, salida);
        });

        if (disponibles.length === 0) {
            habitacionSelect.disabled = true;
            habitacionSelect.innerHTML = '<option value="">Sin habitaciones disponibles</option>';
            return;
        }

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
    // FUNCIÓN AUXILIAR: ¿La habitación está ocupada?
    // ============================================================
    function estaOcupada(habitacionId, entrada, salida) {
        if (!entrada || !salida) return false;

        const f1 = new Date(entrada);
        const f2 = new Date(salida);

        return RESERVAS_EXISTENTES.some(function (r) {
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

        if (!this.value) {
            pisoHabitacion.value = '';
            actualizarResumen();
            return;
        }

        pisoHabitacion.value = opcion.dataset.piso || '';

        if (opcion.dataset.precio) {
            precioActual = parseFloat(opcion.dataset.precio);
            precioHabitacion.value = 'S/. ' + precioActual.toFixed(2);
        }

        actualizarResumen();
    });

    // ============================================================
    // F) ACTUALIZAR RESUMEN EN TIEMPO REAL
    // ============================================================
    [fechaEntrada, fechaSalida, cantPersonas].forEach(function (el) {
        if (el) {
            el.addEventListener('change', actualizarResumen);
            el.addEventListener('input', actualizarResumen);
        }
    });

    fechaEntrada.addEventListener('change', recargarHabitaciones);
    fechaSalida.addEventListener('change', recargarHabitaciones);

    function recargarHabitaciones() {
        if (tipoHabitacion.value) {
            cargarHabitacionesDisponibles(tipoHabitacion.value);
        }
        actualizarResumen();
    }

    function actualizarResumen() {
        // Habitación
        const opHab = habitacionSelect.options[habitacionSelect.selectedIndex];
        lblHabitacion.textContent = habitacionSelect.value ? opHab.textContent : '--';

        // Tipo
        const opTipo = tipoHabitacion.options[tipoHabitacion.selectedIndex];
        lblTipo.textContent = tipoHabitacion.value ? opTipo.textContent : '--';

        // Entrada / Salida
        lblEntrada.textContent = fechaEntrada.value || '--';
        lblSalida.textContent = fechaSalida.value || '--';

        // Noches
        const noches = calcularNoches(fechaEntrada.value, fechaSalida.value);
        lblNoches.textContent = noches;

        // Precio por noche
        lblPrecioNoches.textContent = 'S/. ' + precioActual.toFixed(2);

        // Personas
        lblPersonas.textContent = cantPersonas.value || '1';

        // Total
        const total = noches * precioActual;
        lblTotal.textContent = 'S/. ' + total.toFixed(2);

        if (inputCostoTotal) {
            inputCostoTotal.value = total.toFixed(2);
        }
    }

    // ============================================================
    // UTILIDAD: CALCULAR NOCHES
    // ============================================================
    function calcularNoches(entrada, salida) {
        if (!entrada || !salida) return 0;

        const f1 = new Date(entrada);
        const f2 = new Date(salida);
        const diff = Math.floor((f2 - f1) / (1000 * 60 * 60 * 24));

        return diff > 0 ? diff : 0;
    }

    // ============================================================
    // G) INICIALIZACIÓN AL CARGAR LA PÁGINA
    // Maneja tanto "Nueva Reserva" como "Editar Reserva".
    // ============================================================

    // G.1: ¿Estamos editando?
    const esEdicion = ID_RESERVA_ACTUAL > 0;

    if (esEdicion) {
        // ------------------------------------------------
        // MODO EDICIÓN
        // ------------------------------------------------

        // 1) Cliente: si hay cliente preseleccionado, autocompletar sus campos
        if (ID_CLIENTE_ACTUAL > 0 && clienteSelect) {
            clienteSelect.value = ID_CLIENTE_ACTUAL;
            const op = clienteSelect.options[clienteSelect.selectedIndex];
            if (op) autocompletarCliente(op);
        }

        // 2) Tipo + Habitación: si hay habitación seleccionada, buscar
        //    su tipo en HABITACIONES, preseleccionar el tipo, cargar
        //    las habitaciones disponibles y preseleccionar la actual.
        if (ID_HABITACION_ACTUAL > 0) {
            const habitacionActual = HABITACIONES.find(
                h => String(h.Id) === String(ID_HABITACION_ACTUAL)
            );

            if (habitacionActual && tipoHabitacion) {
                // Preseleccionar el tipo
                tipoHabitacion.value = habitacionActual.TipoId;

                // Cargar precio del tipo
                precioActual = parseFloat(habitacionActual.Precio) || 0;
                precioHabitacion.value = 'S/. ' + precioActual.toFixed(2);

                // Cargar habitaciones del tipo (sin filtrar por disponibilidad
                // porque la habitación actual puede estar "ocupada" por esta misma reserva)
                cargarHabitacionesDisponibles(tipoHabitacion.value);

                // Preseleccionar la habitación actual
                if (habitacionSelect) {
                    habitacionSelect.value = ID_HABITACION_ACTUAL;

                    // Autocompletar el piso
                    const opHab = habitacionSelect.options[habitacionSelect.selectedIndex];
                    if (opHab) {
                        pisoHabitacion.value = opHab.dataset.piso || '';
                    }
                }
            }
        }
    } else {
        // ------------------------------------------------
        // MODO NUEVA RESERVA
        // ------------------------------------------------
        if (clienteSelect) {
            clienteSelect.value = PLACEHOLDER_CLIENTE;
            limpiarCamposCliente();
        }
    }

    // G.3: Actualizar el resumen final
    actualizarResumen();
});
});