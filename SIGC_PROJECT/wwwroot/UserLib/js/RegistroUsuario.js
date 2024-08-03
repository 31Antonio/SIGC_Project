document.addEventListener("DOMContentLoaded", function () {

    //Evento de cambio para el select de rol para
    document.getElementById("roles").addEventListener("change", function () {
        var rol = this.value;

        if (rol == "2") {
            document.getElementById("doctor-tab").style.display = "block";
            document.getElementById("paciente-tab").style.display = "none";
        }
        else if (rol == "4") {
            document.getElementById("paciente-tab").style.display = "block";
            document.getElementById("doctor-tab").style.display = "none";
        }
    });

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

    //Funcion para el evento input
    function agregarEventListenerCedula(input) {
        document.getElementById(input).addEventListener('input', function () {
            var valor = this.value;
            var cursorPos = this.selectionStart; // Obtener la posición del cursor
            var valorFormateado = formatearCedula(valor, cursorPos);

            // Actualizar el valor del campo
            this.value = valorFormateado;

            // Restaurar la posición del cursor
            var newCursorPos = cursorPos;
            if (valor.length < valorFormateado.length) {
                newCursorPos++;
            }
            this.setSelectionRange(newCursorPos, newCursorPos);
        })
    }

    agregarEventListenerCedula('Paciente_Cedula');
    agregarEventListenerCedula('Doctor_Cedula');

    //Funcion para formatear el campo de telefono
    function formatearTelefono(valor, cursorPos) {
        var valorNumerico = valor.replace(/\D/g, '');
        var valorFormateado = '';

        if (valorNumerico.length > 0) {
            valorFormateado += valorNumerico.substr(0, 3);
        }
        if (valorNumerico.length >= 4) {
            valorFormateado += '-' + valorNumerico.substr(3, 3);
        }
        if (valorNumerico.length >= 7) {
            valorFormateado += '-' + valorNumerico.substr(6, 4);
        }

        return valorFormateado;
    }

    //Funcion para el evento input
    function agregarEventListenerTelefono(input) {
        document.getElementById(input).addEventListener('input', function () {
            var valor = this.value;
            var cursorPos = this.selectionStart; // Obtener la posición del cursor
            var valorFormateado = formatearTelefono(valor, cursorPos);

            // Actualizar el valor del campo
            this.value = valorFormateado;

            // Restaurar la posición del cursor
            var newCursorPos = cursorPos;
            if (valor.length < valorFormateado.length) {
                newCursorPos++;
            }
            this.setSelectionRange(newCursorPos, newCursorPos);
        });
    }

    agregarEventListenerTelefono('Paciente_Telefono');
    agregarEventListenerTelefono('Doctor_Telefono');


    //Evento para calcular la edad cuando se elija la fecha de namicimiento del paciente
    document.getElementById("Paciente_FechaNacimiento").addEventListener("change", function () {
        var fechaNacimiento = document.getElementById("Paciente_FechaNacimiento").value;

        //Llamar a la función y asignarla a una variable
        var edad = CalcularEdad(fechaNacimiento);

        if (edad != null) {
            document.getElementById("pacienteEdad").value = edad;
        }
        else {
            console.log("Ha ocurrido un error");
        }
    });

    //Funcion para calcular la edad
    function CalcularEdad(fechaNacimiento) {
        //Validar y Parsear la fecha de nacimiento
        var partes = fechaNacimiento.split("-");

        var anho = parseInt(partes[0], 10);
        var mes = parseInt(partes[1], 10) - 1; //Los meses son del 0-11 en JS
        var dia = parseInt(partes[2], 10);

        var fechaNac = new Date(anho, mes, dia);

        var hoy = new Date();
        var edad = hoy.getFullYear() - fechaNac.getFullYear();
        var diferenciaMeses = hoy.getMonth() - fechaNac.getMonth();
        var diferenciaDias = hoy.getDate() - fechaNac.getDate();

        //Ajustar si el cumpleños aún no ha ocurrido en el año actual
        if (diferenciaMeses < 0 || (diferenciaMeses === 0 && diferenciaDias < 0)) {
            edad--;
        }

        //Regresar la edad calculada
        return edad;
    }

});