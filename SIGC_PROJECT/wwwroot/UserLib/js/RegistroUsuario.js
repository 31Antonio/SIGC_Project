//Enviar informacion al metodo de Registrar Usuario
document.getElementById("Registrar").addEventListener("submit", function (e) {

    //Asignar el valor de los campos a variables
    var nombre = document.getElementById("nombre").value;
    var contrasena = document.getElementById("contrasena").value;
    var roles = $("#roles").val();

    //Evitar el evento
    e.preventDefault();

    //Hacer una llamada Ajax al controlador
    $.ajax({
        beforeSend: function () {
            $("#Registrar button[type=submit]").prop("disabled", true);
        },
        type: this.method,
        url: "/RegistrosUsuario/Registrar",
        data: { nombre: nombre, contrasena: contrasena, rol: roles },
        success: function (data) {
            alert("Usuario Registrado con Exito");
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseJSON.Message);
        },
        complete: function () {
            $("#Registrar button[type=submit]").prop("disabled", false);
        }
    });
});
