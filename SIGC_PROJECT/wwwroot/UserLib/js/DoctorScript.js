document.addEventListener("DOMContentLoaded", function () {

    // ========== FUNCION PARA FORMATEAR EL CAMPO DE CEDULA ========== //
    //#region
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

    var cedulaInput = document.getElementById('Doctor_Cedula');

    cedulaInput.addEventListener('input', function () {
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

    //#endregion
    //=================================================================//

    // ========== FUNCION PARA FORMATEAR EL CAMPO DE TELEFONO ========== //
    //#region

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

    var telefonoInput = document.getElementById('Doctor_Telefono');

    telefonoInput.addEventListener('input', function () {
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

    //#endregion
    //==================================================================//

    //EVENTO CLICK PARA EL BOTON DE CREAR
    document.getElementById("btnCrear").addEventListener('submit', function (e) {

        //Datos del Usuario
        var nombreU = document.getElementById('nombre').value;
        var contrasena = document.getElementById('contrasena').value;
        var rol = document.getElementById('roles').value;

        //Datos del Doctor
        var cedula = document.getElementById('Doctor_Cedula').value;
        var nombre = document.getElementById('Doctor_Nombre').value;
        var apellido = document.getElementById('Doctor_Apellido').value;
        var especialidad = document.getElementById('Doctor_Especialidad').value;
        var numeroExe = document.getElementById('Doctor_NumeroExequatur').value;
        var telefono = document.getElementById('Doctor_Telefono').value;
        var correo = document.getElementById('Doctor_CorreoElectronico').value;

        var Usuario = {
            "NombreUsuario": nombreU,
            "Contrasena": contrasena
        }

        var Rol = rol;

        var Doctor = {
            "Cedula": cedula,
            "Nombre": nombre,
            "Apellido": apellido,
            "Especialidad": especialidad,
            "NumeroExequatur": numeroExe,
            "Telefono": telefono,
            "CorreoElectronico": correo
        }

        var jsonData = {
            Usuario,
            Rol,
            Doctor
        }

        $.ajax({
            url: '/Doctors/Create',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(jsonData),
            success: function (response) {

            },
            error: function (error) {
                console.log(error);
            }
        });

    });

});