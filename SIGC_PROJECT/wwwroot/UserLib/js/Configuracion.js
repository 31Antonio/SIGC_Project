
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
                agregarEventListenerEdad();
                agregarEventListenerCedula();
                agregarEventListenerTelefono();
                agregarEventListenerBoton();
            })
            //Si hay algun error, se captura y se muestra en la consola
            .catch(error => console.log('Ha ocurrido un error', error))
    }

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

    //Funcion para el evento listener
    function agregarEventListenerEdad() {
        var fechaNacimientoInput = document.getElementById("FechaNacimiento");
        if (fechaNacimientoInput) {
            fechaNacimientoInput.addEventListener("change", function () {
                var fechaNacimiento = fechaNacimientoInput.value;

                // Llamar a la función y asignarla a una variable
                var edad = CalcularEdad(fechaNacimiento);

                if (edad != null) {
                    document.getElementById("Edad").value = edad;
                } else {
                    console.log("Ha ocurrido un error");
                }
            });
        }
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

    //Funcion para el evento listener
    function agregarEventListenerCedula() {
        var cedulaInput = document.getElementById('Cedula');
        if (cedulaInput) {
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
        }
    }

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

    //Funcion para el evento listener
    function agregarEventListenerTelefono() {
        var telefonoInput = document.getElementById('Telefono');
        if (telefonoInput) {
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
        }
    }

    function agregarEventListenerBoton() {

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
    });
    //----------------------------------------------------------------//

    // #endregion
    //=============================================//

});
