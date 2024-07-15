document.addEventListener("DOMContentLoaded", function () {

//Enviar informacion al metodo de Registrar Usuario
//document.getElementById("Registrar").addEventListener("submit", function (e) {

//    //Hacer una llamada Ajax al controlador
//    $.ajax({
//        beforeSend: function () {
//            $("#Registrar button[type=submit]").prop("disabled", true);
//        },
//        type: 'GET',
//        url: "/RegistrosUsuario/Registrar",
//        success: function (data) {
//            alert("Usuario Registrado con Exito");
//        },
//        error: function (xhr, status, error) {
//            console.error(xhr);
//        },
//        complete: function () {
//            $("#Registrar button[type=submit]").prop("disabled", false);
//        }
//    });
//});

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