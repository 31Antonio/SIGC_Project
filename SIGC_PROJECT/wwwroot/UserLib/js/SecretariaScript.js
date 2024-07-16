document.addEventListener("DOMContentLoaded", function () {

    //Funcion para el campo de Horario de trabajo

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

    function validarHorario() {
        var desde = document.getElementById("Desde").value;
        var hasta = document.getElementById("Hasta").value;
        var formato = conversion(desde) + ' - ' + conversion(hasta);

        var horario = document.getElementById('Secretaria_HorarioTrabajo');
        horario.value = formato;
    }

    document.getElementById("Desde").addEventListener('change', validarHorario);
    document.getElementById("Hasta").addEventListener('change', validarHorario);


});