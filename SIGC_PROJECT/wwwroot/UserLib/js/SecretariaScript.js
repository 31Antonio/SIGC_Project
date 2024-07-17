document.addEventListener("DOMContentLoaded", function () {

    // ========== FUNCIONES PARA EL CAMPO DE HORARIO ========== //
    // #region 
    // Función para convertir un valor de 24 horas a formato de 12 horas con AM/PM
    function conversion(valor) {

        // Se divide el valor en horas y minutos
        var [horas, minutos] = valor.split(':').map(Number);

        // Determinar si es a.m. o p.m.
        var periodo = horas >= 12 ? 'p.m.' : 'a.m.';

        // Convierte la hora al formato de 12 horas
        horas = horas % 12;
        horas = horas ? horas : 12; // La hora 0 debe ser 12

        // Devuelve la cadena formateada
        return horas + ':' + (minutos < 10 ? '0' : '') + minutos + ' ' + periodo;
    }

    //Funcion para asignar el horario
    function validarHorario() {
        var desde = document.getElementById("Desde").value;
        var hasta = document.getElementById("Hasta").value;
        var formato = conversion(desde) + ' - ' + conversion(hasta);

        var horario = document.getElementById('Secretaria_HorarioTrabajo');
        horario.value = formato;
    }

    document.getElementById("Desde").addEventListener('change', validarHorario);
    document.getElementById("Hasta").addEventListener('change', validarHorario);

    // #endregion
    //==========================================================//

    //EVENTO CLICK PARA EL BOTON DE CREAR
    document.getElementById("btnCrear").addEventListener('submit', function (e) {

        //Datos del Usuario
        var nombreU = document.getElementById('nombre').value;
        var contrasena = document.getElementById('contrasena').value;
        var rol = document.getElementById('roles').value;

        //Datos de la Secretaria
        var cedula = document.getElementById('Secretaria_Cedula').value;
        var nombre = document.getElementById('Secretaria_Nombre').value;
        var apellido = document.getElementById('Secretaria_Apellido').value;
        var horario = document.getElementById('Secretaria_HorarioTrabajo').value;
        var telefono = document.getElementById('Secretaria_Telefono').value;
        var correo = document.getElementById('Secretaria_CorreoElectronico').value;

        var Usuario = {
            "NombreUsuario": nombreU,
            "Contrasena": contrasena
        }

        var Rol = rol;

        var Secretaria = {
            "Cedula": cedula,
            "Nombre": nombre,
            "Apellido": apellido,
            "HorarioTrabajo": horario,
            "Telefono": telefono,
            "CorreoElectronico": correo
        }

        var jsonData = {
            Usuario,
            Rol,
            Secretaria
        }

        $.ajax({
            url: '/Secretariums/Create',
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