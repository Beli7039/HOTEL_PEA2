/* ============================================================
   JS_ModificarHabitacion.js
   Lógica para la vista de Modificar Habitación.
   ============================================================ */

document.addEventListener('DOMContentLoaded', function () {

    const tipoHabitacion = document.getElementById('tipoHabitacionSelect');
    const precioHabitacion = document.getElementById('precioHabitacion');
    const habitacionSelect = document.getElementById('habitacionSelect');
    const pisoHabitacion = document.getElementById('pisoHabitacion');

    const lblHabitacion = document.getElementById('lblHabitacion');
    const lblTipo = document.getElementById('lblTipo');
    const lblPiso = document.getElementById('lblPiso');
    const lblPrecio = document.getElementById('lblPrecio');
    const lblBase = document.getElementById('lblBase');
    const lblCargo = document.getElementById('lblCargo');
    const lblTotal = document.getElementById('lblTotal');

    let precioActual = 0;

    // ---- Cargo administrativo ----
    function calcularCargo(base) {
        const c = base * PORCENTAJE_CARGO;
        return c < CARGO_MINIMO ? CARGO_MINIMO : c;
    }

    // ---- Cambio de tipo ----
    tipoHabitacion.addEventListener('change', function () {
        const opcion = this.options[this.selectedIndex];
        const tipoId = opcion.dataset.id;

        if (!tipoId) {
            precioActual = 0;
            precioHabitacion.value = 'S/. 0.00';
            habitacionSelect.innerHTML = '<option value="">-- Primero elija un tipo --</option>';
            habitacionSelect.disabled = true;
            pisoHabitacion.value = '';
            actualizarResumen();
            return;
        }

        const habTipo = HABITACIONES.find(h => String(h.TipoId) === String(tipoId));
        precioActual = habTipo ? parseFloat(habTipo.Precio) : 0;
        precioHabitacion.value = 'S/. ' + precioActual.toFixed(2);

        cargarHabitaciones(tipoId);
        actualizarResumen();
    });

    // ---- Cargar habitaciones ----
    function cargarHabitaciones(tipoId) {
        habitacionSelect.innerHTML = '<option value="">-- Seleccione habitación --</option>';
        pisoHabitacion.value = '';

        const delTipo = HABITACIONES.filter(h => String(h.TipoId) === String(tipoId));
        const disponibles = delTipo.filter(h => !estaOcupada(h.Id));

        if (disponibles.length === 0) {
            habitacionSelect.disabled = true;
            habitacionSelect.innerHTML = '<option value="">Sin habitaciones disponibles</option>';
            return;
        }

        habitacionSelect.disabled = false;
        disponibles.forEach(h => {
            const opt = document.createElement('option');
            opt.value = h.Id;
            opt.textContent = 'Hab. ' + h.Numero;
            opt.dataset.piso = h.Piso;
            opt.dataset.precio = h.Precio;
            habitacionSelect.appendChild(opt);
        });
    }

    // ---- ¿Está ocupada? ----
    function estaOcupada(habitacionId) {
        return RESERVAS_EXISTENTES.some(function (r) {
            if (r.IdReserva === ID_RESERVA_ACTUAL) return false;
            if (String(r.HabitacionId) !== String(habitacionId)) return false;

            const rEntrada = new Date(r.FechaEntrada);
            const rSalida = new Date(r.FechaSalida);
            const f1 = new Date(FECHA_ENTRADA);
            const f2 = new Date(FECHA_SALIDA);

            return f1 < rSalida && f2 > rEntrada;
        });
    }

    // ---- Cambio de habitación ----
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

    // ---- Actualizar resumen ----
    function actualizarResumen() {
        const opHab = habitacionSelect.options[habitacionSelect.selectedIndex];
        const opTipo = tipoHabitacion.options[tipoHabitacion.selectedIndex];

        const base = NOCHES * precioActual;
        const cargo = calcularCargo(base);
        const total = base + cargo + CARGO_PREVIO;

        lblHabitacion.textContent = habitacionSelect.value ? opHab.textContent : '--';
        lblTipo.textContent = tipoHabitacion.value ? opTipo.textContent : '--';
        lblPiso.textContent = pisoHabitacion.value || '--';
        lblPrecio.textContent = 'S/. ' + precioActual.toFixed(2);
        lblBase.textContent = 'S/. ' + base.toFixed(2);
        lblCargo.textContent = 'S/. ' + cargo.toFixed(2);
        lblTotal.textContent = 'S/. ' + total.toFixed(2);
    }

    // ---- Inicialización: preseleccionar la habitación actual ----
    if (ID_HABITACION_ACTUAL > 0) {
        const habActual = HABITACIONES.find(h => String(h.Id) === String(ID_HABITACION_ACTUAL));

        if (habActual) {
            const opTipo = Array.from(tipoHabitacion.options)
                .find(o => o.dataset.id === String(habActual.TipoId));

            if (opTipo) tipoHabitacion.value = opTipo.value;

            precioActual = parseFloat(habActual.Precio) || 0;
            precioHabitacion.value = 'S/. ' + precioActual.toFixed(2);

            cargarHabitaciones(habActual.TipoId);

            habitacionSelect.value = ID_HABITACION_ACTUAL;
            pisoHabitacion.value = habActual.Piso;
        }
    }

    actualizarResumen();
});