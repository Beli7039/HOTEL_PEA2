/* ============================================================
   JS_Reserva.js
   Lógica interactiva de la página "Nueva Reserva"
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

    const lblHabitacion = document.getElementById('lblHabitacion');
    const lblTipo = document.getElementById('lblTipo');
    const lblEntrada = document.getElementById('lblEntrada');
    const lblSalida = document.getElementById('lblSalida');
    const lblNoches = document.getElementById('lblNoches');
    const lblPrecioNoches = document.getElementById('lblPrecioNoches');
    const lblPersonas = document.getElementById('lblPersonas');
    const lblTotal = document.getElementById('lblTotal');
    const inputCostoTotal = document.getElementById('inputCostoTotal');

    const PLACEHOLDER_CLIENTE = '0';

    let precioActual = 0;

    // ============================================================
    // AUXILIARES
    // ============================================================
    function limpiarCamposCliente() {
        if (clienteDni) clienteDni.value = '';
        if (clienteNombre) clienteNombre.value = '';
        if (clienteTelefono) clienteTelefono.value = '';
        if (clienteEmail) clienteEmail.value = '';
    }

    function autocompletarCliente() {
        if (!clienteSelect || clienteSelect.value === PLACEHOLDER_CLIENTE) {
            limpiarCamposCliente();
            return;
        }

        const opcion = clienteSelect.options[clienteSelect.selectedIndex];
        if (!opcion) return;

        const dni = opcion.getAttribute('data-dni') || '';
        const nombre = opcion.getAttribute('data-nombre') || '';
        const telefono = opcion.getAttribute('data-telefono') || '';
        const email = opcion.getAttribute('data-email') || '';

        if (clienteDni) clienteDni.value = dni;
        if (clienteNombre) clienteNombre.value = nombre;
        if (clienteTelefono) clienteTelefono.value = telefono;
        if (clienteEmail) clienteEmail.value = email;
    }

    function calcularNoches(entrada, salida) {
        if (!entrada || !salida) return 0;
        const f1 = new Date(entrada);
        const f2 = new Date(salida);
        const diff = Math.floor((f2 - f1) / (1000 * 60 * 60 * 24));
        return diff > 0 ? diff : 0;
    }

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
    // A) TOGGLE CLIENTE EXISTENTE / NUEVO
    // ============================================================
    radiosTipo.forEach(function (radio) {
        radio.addEventListener('change', function () {
            const esExistente = this.value === 'existente';
            inputTipoCliente.value = esExistente ? 'existente' : 'nuevo';
            bloqueExistente.style.display = esExistente ? 'block' : 'none';

            [clienteDni, clienteNombre, clienteTelefono, clienteEmail].forEach(function (input) {
                if (input) input.readOnly = esExistente;
            });

            clienteSelect.value = PLACEHOLDER_CLIENTE;
            limpiarCamposCliente();
        });
    });

    // ============================================================
    // B) AUTOCOMPLETAR AL CAMBIAR CLIENTE
    // ============================================================
    if (clienteSelect) {
        clienteSelect.addEventListener('change', autocompletarCliente);
    }

    // ============================================================
    // C) AUTOCOMPLETAR PRECIO AL ELEGIR TIPO
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

    if (tipoHabitacion) {
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

            const habitacionDelTipo = HABITACIONES.find(
                h => String(h.TipoId) === String(this.value) || String(h.Tipo) === String(this.value)
            );

            precioActual = habitacionDelTipo ? parseFloat(habitacionDelTipo.Precio) : 0;
            precioHabitacion.value = 'S/. ' + precioActual.toFixed(2);

            cargarHabitacionesDisponibles(this.value);
            actualizarResumen();
        });
    }

    // ============================================================
    // E) AUTOCOMPLETAR PISO AL ELEGIR HABITACIÓN
    // ============================================================
    if (habitacionSelect) {
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
    }

    // ============================================================
    // F) ACTUALIZAR RESUMEN
    // ============================================================
    function actualizarResumen() {
        const opHab = habitacionSelect.options[habitacionSelect.selectedIndex];
        lblHabitacion.textContent = habitacionSelect.value ? opHab.textContent : '--';

        const opTipo = tipoHabitacion.options[tipoHabitacion.selectedIndex];
        lblTipo.textContent = tipoHabitacion.value ? opTipo.textContent : '--';

        lblEntrada.textContent = fechaEntrada.value || '--';
        lblSalida.textContent = fechaSalida.value || '--';

        const noches = calcularNoches(fechaEntrada.value, fechaSalida.value);
        lblNoches.textContent = noches;

        lblPrecioNoches.textContent = 'S/. ' + precioActual.toFixed(2);
        lblPersonas.textContent = cantPersonas.value || '1';

        const total = noches * precioActual;
        lblTotal.textContent = 'S/. ' + total.toFixed(2);

        if (inputCostoTotal) {
            inputCostoTotal.value = total.toFixed(2);
        }
    }

    [fechaEntrada, fechaSalida, cantPersonas].forEach(function (el) {
        if (el) {
            el.addEventListener('change', actualizarResumen);
            el.addEventListener('input', actualizarResumen);
        }
    });

    function recargarHabitaciones() {
        if (tipoHabitacion.value) {
            cargarHabitacionesDisponibles(tipoHabitacion.value);
        }
        actualizarResumen();
    }

    if (fechaEntrada) fechaEntrada.addEventListener('change', recargarHabitaciones);
    if (fechaSalida) fechaSalida.addEventListener('change', recargarHabitaciones);

    // ============================================================
    // G) INICIALIZACIÓN
    // ============================================================
    const esEdicion = ID_RESERVA_ACTUAL > 0;

    if (esEdicion) {
        if (ID_CLIENTE_ACTUAL > 0 && clienteSelect) {
            clienteSelect.value = ID_CLIENTE_ACTUAL;
            autocompletarCliente();
        }

        if (ID_HABITACION_ACTUAL > 0) {
            const habitacionActual = HABITACIONES.find(
                h => String(h.Id) === String(ID_HABITACION_ACTUAL)
            );

            if (habitacionActual && tipoHabitacion) {
                tipoHabitacion.value = habitacionActual.TipoId;

                precioActual = parseFloat(habitacionActual.Precio) || 0;
                precioHabitacion.value = 'S/. ' + precioActual.toFixed(2);

                cargarHabitacionesDisponibles(tipoHabitacion.value);

                if (habitacionSelect) {
                    habitacionSelect.value = ID_HABITACION_ACTUAL;
                    const opHab = habitacionSelect.options[habitacionSelect.selectedIndex];
                    if (opHab) pisoHabitacion.value = opHab.dataset.piso || '';
                }
            }
        }
    } else {
        if (clienteSelect) {
            clienteSelect.value = PLACEHOLDER_CLIENTE;
            limpiarCamposCliente();
        }

        setTimeout(function () {
            if (clienteSelect) {
                clienteSelect.value = PLACEHOLDER_CLIENTE;
                limpiarCamposCliente();
            }
        }, 100);
    }

    actualizarResumen();
});