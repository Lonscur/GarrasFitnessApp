// wwwroot/js/cobros.js
// Lógica del mostrador de cobros (Views/Pagos/Create.cshtml)
// Responsabilidad principal: BLOQUEAR el precio total para que nunca se pueda
// editar a mano — siempre se recalcula desde el Plan + la Promoción elegidos.
// También valida que el desglose de métodos de pago (Efectivo/QR/Tarjeta)
// sume exactamente el total, para no mandarle a Jhoel un dato descuadrado.

(function () {
    document.addEventListener('DOMContentLoaded', function () {

        const planSelect = document.getElementById('planSelect');
        const promoSelect = document.getElementById('promoSelect');

        const subtotalDisplay = document.getElementById('subtotalDisplay');
        const descuentoDisplay = document.getElementById('descuentoDisplay');
        const totalDisplay = document.getElementById('totalDisplay');
        const totalOculto = document.getElementById('MontoTotal'); // input hidden que sí se envía al servidor

        const inputEfectivo = document.getElementById('montoEfectivo');
        const inputQr = document.getElementById('montoQr');
        const inputTarjeta = document.getElementById('montoTarjeta');

        const sumaDisplay = document.getElementById('sumaMetodosDisplay');
        const alertaDescuadre = document.getElementById('alertaDescuadre');
        const btnCobrar = document.getElementById('btnCobrar');

        if (!planSelect || !totalDisplay) {
            // Esta página no está cargada (o todavía no existe el formulario), no hacer nada.
            return;
        }

        function formatoMoneda(valor) {
            return 'Bs. ' + valor.toFixed(2);
        }

        // === 1. Calcular Subtotal + Descuento + Total (el precio queda BLOQUEADO) ===
        function recalcularTotal() {
            const opcionPlan = planSelect.options[planSelect.selectedIndex];
            const precioPlan = opcionPlan ? parseFloat(opcionPlan.dataset.precio || '0') : 0;

            let descuentoAplicado = 0;
            if (promoSelect && promoSelect.value) {
                const opcionPromo = promoSelect.options[promoSelect.selectedIndex];
                const porcentaje = parseFloat(opcionPromo.dataset.descuento || '0');
                descuentoAplicado = precioPlan * (porcentaje / 100);
            }

            const total = Math.max(precioPlan - descuentoAplicado, 0);

            subtotalDisplay.textContent = formatoMoneda(precioPlan);
            descuentoDisplay.textContent = '- ' + formatoMoneda(descuentoAplicado);
            totalDisplay.textContent = formatoMoneda(total);

            // El input real que se manda al backend SIEMPRE se llena por código,
            // nunca por el usuario. El campo visible es readonly (ver el .cshtml).
            if (totalOculto) {
                totalOculto.value = total.toFixed(2);
            }

            validarDesglose();
            return total;
        }

        // === 2. Validar que Efectivo + QR + Tarjeta sumen exactamente el total ===
        function validarDesglose() {
            const total = parseFloat(totalOculto ? totalOculto.value : '0') || 0;

            const efectivo = parseFloat(inputEfectivo ? inputEfectivo.value : '0') || 0;
            const qr = parseFloat(inputQr ? inputQr.value : '0') || 0;
            const tarjeta = parseFloat(inputTarjeta ? inputTarjeta.value : '0') || 0;

            const suma = efectivo + qr + tarjeta;

            if (sumaDisplay) {
                sumaDisplay.textContent = formatoMoneda(suma);
            }

            // Se permite un margen mínimo por redondeo de centavos
            const cuadra = Math.abs(suma - total) < 0.01;

            if (alertaDescuadre) {
                if (!cuadra && suma > 0) {
                    alertaDescuadre.classList.remove('d-none');
                    alertaDescuadre.classList.add('d-flex');
                } else {
                    alertaDescuadre.classList.add('d-none');
                    alertaDescuadre.classList.remove('d-flex');
                }
            }

            if (btnCobrar) {
                btnCobrar.disabled = !cuadra || total <= 0;
            }

            return cuadra;
        }

        planSelect.addEventListener('change', recalcularTotal);
        if (promoSelect) {
            promoSelect.addEventListener('change', recalcularTotal);
        }
        [inputEfectivo, inputQr, inputTarjeta].forEach(function (input) {
            if (input) {
                input.addEventListener('input', validarDesglose);
            }
        });

        // Defensa extra: si alguien intenta editar el total "oculto" desde la
        // consola del navegador, lo recalculamos apenas se envíe el formulario.
        const formularioCobro = document.getElementById('formCobro');
        if (formularioCobro) {
            formularioCobro.addEventListener('submit', function (e) {
                const totalReal = recalcularTotal();
                if (!validarDesglose() || totalReal <= 0) {
                    e.preventDefault();
                }
            });
        }

        recalcularTotal();
    });
})();