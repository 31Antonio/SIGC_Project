
document.addEventListener("DOMContentLoaded", function () {

    // #region ----- Mostrar el contenido para la configuracion del paciente -----//

    //Funcion para cargar el contenido en el div del mainContent
    function cargarContenido(url, contenedorId) {
        //Solicitud GET
        fetch(url)
            //Si recibe una respuesta, pasarla a texto
            .then(response => response.text())
            //Insertar el texto en el contenedor
            .then(html => {
                document.getElementById(contenedorId).innerHTML = html;

                //Al dar click en Cuenta
                document.getElementById("passwordPaciente").addEventListener('click', function () {
                    cargarContenido("/Pacientes/FormPassword", "content_configPaciente");

                    setTimeout(function () {
                        //EVENTO PARA EL BOTON DE ACTUALIZAR CONTRASEÑA
                        const btnCambiarPass = document.getElementById("btnCambiar");
                        console.log("Cambiar: ", btnCambiarPass)
                        if (btnCambiarPass) {
                            btnCambiarPass.addEventListener("click", function () {
                                var currentPass = document.getElementById("currentPassword").value;
                                var nuevaPass = document.getElementById("newPassword").value;
                                var confirmPass = document.getElementById("confirmPass").value;
                                var nombre = document.getElementById("nombreUsu").value;

                                if (nuevaPass != confirmPass) {

                                    document.getElementById("alertPass").textContent = "Las contraseñas no coinciden";
                                    document.getElementById("alertPass").style.display = "block";

                                    setTimeout(function () {
                                        document.getElementById("alertPass").style.display = "none";
                                    }, 2000);
                                }
                                else if (nuevaPass == confirmPass) {
                                    $.ajax({
                                        url: "/Usuarios/ActualizarPassword",
                                        type: "POST",
                                        data: { currentPass: currentPass, nuevaPass: nuevaPass, userName: nombre },
                                        success: function (response) {
                                            if (response.success) {
                                                alert(response.mensaje)
                                            } else {
                                                alert(response.mensaje)
                                            }
                                        },
                                        error: function (error) {
                                            console.error("Ha ocurrido un error: ", error);
                                        }
                                    });
                                }
                            });
                        }
                    }, 1000);
                });
                // #endregion ----------------------------------------------------------------//
            })
            //Si hay algun error, se captura y se muestra en la consola
            .catch(error => console.log('Ha ocurrido un error', error))
    }

    //Cargar el formulario por defecto al cargar la pagina
    cargarContenido("/Pacientes/FormEdit", "content_configPaciente");

    //Al dar click en Informacion General
    document.getElementById("infoPaciente").addEventListener('click', function () {
        cargarContenido("/Pacientes/FormEdit", "content_configPaciente");
    });

});
