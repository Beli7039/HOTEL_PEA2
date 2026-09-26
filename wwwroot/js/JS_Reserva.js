document.addEventListener("DOMContentLoaded", function () {
    // Inputs del formulario
    const fechaEntrada = document.getElementById("fechaEntrada");
    const fechaSalida = document.getElementById("fechaSalida");
    const cantPersonas = document.getElementById("cantPersonas");
    const tipoHabitacionSelect = document.getElementById("tipoHabitacionSelect");
    const habitacionSelect = document.getElementById("habitacionSelect");
    const precioHabitacion = document.getElementById("precioHabitacion");

    // Elementos de la Card Resumen
    const lblHabitacion = document.getElementById("lblHabitacion");
    const lblTipo = document.getElementById("lblTipo");
    const lblEntrada = document.getElementById("lblEntrada");
    const lblSalida = document.getElementById("lblSalida");
    const lblNoches = document.getElementById("lblNoches");
    const lblPrecioNoches = document.getElementById("lblPrecioNoches");
    const lblPersonas = document.getElementById("lblPersonas");
    const lblTotal = document.getElementById("lblTotal");

    // Función para calcular noches evitando problemas de UTC/Zona Horaria
    function actualizarFechasYNoches() {
        const valEntrada = fechaEntrada ? fechaEntrada.value : "";
        const valSalida = fechaSalida ? fechaSalida.value : "";

        if (lblEntrada) lblEntrada.textContent = valEntrada || "--";
        if (lblSalida) lblSalida.textContent = valSalida || "--";

        if (valEntrada && valSalida) {
            // Dividir la fecha YYYY-MM-DD directamente para evitar errores de timezone
            const [y1, m1, d1] = valEntrada.split('-').map(Number);
            const [y2, m2, d2] = valSalida.split('-').map(Number);

            const f1 = new Date(y1, m1 - 1, d1);
            const f2 = new Date(y2, m2 - 1, d2);

            const diferenciaMs = f2 - f1;
            const dias = Math.round(diferenciaMs / (1000 * 60 * 60 * 24));

            if (lblNoches) {
                lblNoches.textContent = dias > 0 ? dias : "0";
            }
        } else {
            if (lblNoches) lblNoches.textContent = "0";
        }

        actualizarCostoTotal();
    }

    // Función para actualizar el tipo y la habitación seleccionada
    function actualizarDetalleHabitacion() {
        if (tipoHabitacionSelect && lblTipo) {
            const selectedOpt = tipoHabitacionSelect.options[tipoHabitacionSelect.selectedIndex];
            lblTipo.textContent = (tipoHabitacionSelect.value && selectedOpt) ? selectedOpt.text : "--";
        }

        if (habitacionSelect && lblHabitacion) {
            const selectedOpt = habitacionSelect.options[habitacionSelect.selectedIndex];
            lblHabitacion.textContent = (habitacionSelect.value && selectedOpt) ? selectedOpt.text : "--";
        }
    }

    // Función para calcular el costo total
    function actualizarCostoTotal() {
        const noches = parseInt(lblNoches ? lblNoches.textContent : 0) || 0;

        // Limpiar de texto cualquier símbolo para parsear sólo números (ej. 'S/. 140.00' -> 140.00)
        let rawPrecio = precioHabitacion ? precioHabitacion.value : "0";
        rawPrecio = rawPrecio.replace(/[^0-9.]/g, '');
        const precio = parseFloat(rawPrecio) || 0;

        if (lblPrecioNoches) {
            lblPrecioNoches.textContent = `S/. ${precio.toFixed(2)}`;
        }

        const total = noches * precio;

        if (lblTotal) {
            lblTotal.textContent = `S/. ${total.toFixed(2)}`;
        }
    }

    // Registrar Eventos con 'input' y 'change'
    if (fechaEntrada) {
        fechaEntrada.addEventListener("input", actualizarFechasYNoches);
        fechaEntrada.addEventListener("change", actualizarFechasYNoches);
    }
    if (fechaSalida) {
        fechaSalida.addEventListener("input", actualizarFechasYNoches);
        fechaSalida.addEventListener("change", actualizarFechasYNoches);
    }

    if (cantPersonas) {
        cantPersonas.addEventListener("input", function () {
            if (lblPersonas) lblPersonas.textContent = this.value || "--";
        });
    }

    if (tipoHabitacionSelect) {
        tipoHabitacionSelect.addEventListener("change", actualizarDetalleHabitacion);
    }

    if (habitacionSelect) {
        habitacionSelect.addEventListener("change", actualizarDetalleHabitacion);
    }

    if (precioHabitacion) {
        precioHabitacion.addEventListener("input", actualizarCostoTotal);
        precioHabitacion.addEventListener("change", actualizarCostoTotal);
    }

    // Observador para detectar si el precio o selects cambian dinámicamente por AJAX desde otros scripts
    if (precioHabitacion) {
        const observer = new MutationObserver(actualizarCostoTotal);
        observer.observe(precioHabitacion, { attributes: true, attributeFilter: ['value'] });
    }

    // Ejecución inicial para sincronizar al cargar la página
    actualizarFechasYNoches();
    actualizarDetalleHabitacion();
    if (cantPersonas && cantPersonas.value && lblPersonas) {
        lblPersonas.textContent = cantPersonas.value;
    }
});