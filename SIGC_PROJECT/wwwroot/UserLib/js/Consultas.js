//FUNCIONES JAVASCRIPT
document.addEventListener("DOMContentLoaded", function () {

    //Funcion para formatear el campo de cedula
    function formatearCedula(valor, cursorPos) {
        var valorNumerico = valor.replace(/\D/g, '');
        var valorFormateado = '';

        if (valorNumerico.length > 0) {
            valorFormateado += valorNumerico.substr(0, 3);
        }
        if (valorNumerico.length >= 4) {
            valorFormateado += '-' + valorNumerico.substr(3, 7);
        }
        if (valorNumerico.length >= 11) {
            valorFormateado += '-' + valorNumerico.substr(10, 1);
        }

        return valorFormateado;
    }

    //Evento para el input del campo de cedula
    var cedulaInput = document.getElementById('cedula');
    var nombrePacienteInput = document.getElementById('nombrePaciente');
    var edadPacienteInput = document.getElementById('edadPaciente');
    var idPacienteInput = document.getElementById('PacienteId');

    cedulaInput.addEventListener('input', function () {
        var valor = this.value.replace(/[^0-9]/g, '');
        var cursorPos = this.selectionStart;
        var valorFormateado = formatearCedula(valor, cursorPos);

        this.value = valorFormateado;

        // Restaurar la posición del cursor
        var newCursorPos = cursorPos;
        if (valor.length < valorFormateado.length) {
            newCursorPos++;
        }
        this.setSelectionRange(newCursorPos, newCursorPos);

        if (valor.length > 0) {
            fetch('/Consultas/buscarPacientePorCedula', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({ cedula: valorFormateado }) // Enviar el valor formateado
            })
                .then(response => response.json())
                .then(data => {
                    if (data) {
                        document.getElementById('pact_input1').style.display = 'block';
                        nombrePacienteInput.value = data.nombreCompleto;

                        document.getElementById('pact_input2').style.display = 'block';
                        edadPacienteInput.value = data.edad;

                        idPacienteInput.value = data.pacienteId;
                    } else {
                        document.getElementById('pact_input1').style.display = 'none';
                        nombrePacienteInput.value = '';

                        document.getElementById('pact_input2').style.display = 'none';
                        edadPacienteInput.value = '';

                        idPacienteInput.value = '';
                    }
                });
        }
    });

});