document.addEventListener("DOMContentLoaded", function () {

    //Evento para calcular la edad cuando se elija la fecha de namicimiento
    document.getElementById("FechaNacimiento").addEventListener("change", function () {
        var fechaNacimiento = document.getElementById("FechaNacimiento").value;

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
    document.getElementById('Cedula').addEventListener('input', function () {
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
    });

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

    //Evento para el input del campo de cedula
    document.getElementById('Telefono').addEventListener('input', function () {
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
});