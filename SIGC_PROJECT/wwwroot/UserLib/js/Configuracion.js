﻿
document.addEventListener("DOMContentLoaded", function () {

    //Funcion para cargar el contenido en el div del mainContent
    function cargarContenido(url, contenedorId) {
        //Solicitud GET
        fetch(url)
            //Si recibe una respuesta, pasarla a texto
            .then(response => response.text())
            //Insertar el texto en el contenedor
            .then(html => {
                document.getElementById(contenedorId).innerHTML = html;

            })
            //Si hay algun error, se captura y se muestra en la consola
            .catch(error => console.log('Ha ocurrido un error', error))
    }

    // ========== CONFIGURACION PACIENTE ========== //
    // #region ----- Mostrar el contenido para la configuracion del paciente -----//

    //Cargar el formulario por defecto al cargar la pagina
    cargarContenido("/Pacientes/FormEdit", "content_configPaciente");

    //Al dar click en Informacion General
    document.getElementById("infoPaciente").addEventListener('click', function () {
        cargarContenido("/Pacientes/FormEdit", "content_configPaciente");
    });

    // ------------------- Al dar click en Cuenta --------------------//
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

                                var modalMensaje = document.getElementById('alertModalMensaje');

                                if (response.success) {

                                    $('#ModalAlert').modal({ backdrop: false });
                                    $('#ModalAlert').modal('show');

                                    modalMensaje.classList.add('alert-success');
                                    modalMensaje.innerText = response.mensaje;

                                    document.getElementById("currentPassword").value = '';
                                    document.getElementById("newPassword").value = '';
                                    document.getElementById("confirmPass").value = '';

                                    setTimeout(function () {
                                        $('#ModalAlert').modal('hide');
                                        modalMensaje.classList.remove('alert-success');
                                        modalMensaje.innerText = '';
                                    }, 3000);
                                    
                                }
                                else {

                                    $('#ModalAlert').modal({ backdrop: false });
                                    $('#ModalAlert').modal('show');

                                    modalMensaje.classList.add('alert-danger');
                                    modalMensaje.innerText = response.mensaje;

                                    document.getElementById("currentPassword").value = '';
                                    document.getElementById("newPassword").value = '';
                                    document.getElementById("confirmPass").value = '';

                                    setTimeout(function () {
                                        modalMensaje.classList.remove('alert-danger');
                                        modalMensaje.innerText = '';
                                        $('#ModalAlert').modal('hide');
                                    }, 3000);
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
    //----------------------------------------------------------------//

    // #endregion
    //=============================================//

});
