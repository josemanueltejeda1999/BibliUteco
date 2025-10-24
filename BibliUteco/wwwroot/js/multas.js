window.multas = {
    generarComprobante: function (multaJson) {
        try {
            var multa = JSON.parse(multaJson);

            // Crear contenido HTML para el comprobante
            var contenido = document.createElement('div');
            contenido.style.fontFamily = 'Arial, sans-serif';
            contenido.style.padding = '20px';
            contenido.innerHTML = `
                <h2 style="color:#2E7D32">Comprobante de Pago - BibliUTECO</h2>
                <hr/>
                <p><strong>Multa Id:</strong> ${multa.multaId}</p>
                <p><strong>Fecha Generada:</strong> ${multa.fechaGenerada}</p>
                <p><strong>Estudiante:</strong> ${multa.estudiante}</p>
                <p><strong>Libro:</strong> ${multa.libro}</p>
                <p><strong>Días de retraso:</strong> ${multa.diasRetraso}</p>
                <p><strong>Monto:</strong> RD$ ${multa.monto.toFixed(2)}</p>
                <p><strong>Método de Pago:</strong> ${multa.metodoPago}</p>
                <p><strong>Fecha Pago:</strong> ${multa.fechaPago}</p>
                <hr/>
                <p>Gracias por su pago. Este documento es su comprobante.</p>
            `;

            // Generar PDF usando html2pdf (asegúrate de cargar la librería)
            var opt = {
                margin:       10,
                filename:     'Comprobante_Multa_' + multa.multaId + '.pdf',
                image:        { type: 'jpeg', quality: 0.98 },
                html2canvas:  { scale: 2 },
                jsPDF:        { unit: 'mm', format: 'a4', orientation: 'portrait' }
            };

            // Añadimos temporalmente al body
            contenido.style.display = 'block';
            document.body.appendChild(contenido);

            html2pdf().set(opt).from(contenido).save().then(function () {
                // eliminar nodo temporal
                document.body.removeChild(contenido);
            });
        } catch (err) {
            console.error(err);
            alert('Error generando comprobante PDF');
        }
    },

    toast: function (mensaje, tipo) {
        // tipo: success | error | info
        alert(mensaje); // implementación simple; puedes cambiar a toasts de Bootstrap
    }
};