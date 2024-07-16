document.addEventListener("DOMContentLoaded", function () {

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