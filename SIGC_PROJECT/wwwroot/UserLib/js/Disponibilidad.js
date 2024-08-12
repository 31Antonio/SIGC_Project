document.addEventListener("DOMContentLoaded", function () {

});

//Funcion para formatear la hora
function formatearHora(hora24) {
    const [hora, minutos] = hora24.split(':');
    const horaFormateada = parseInt(hora, 10);
    const ampm = horaFormateada >= 12 ? 'p.m' : 'a.m';
    const hora12 = horaFormateada % 12 || 12;
    return `${hora12}:${minutos} ${ampm}`;
}

//Funcion para filtrar los doctores por especialidad
async function filtrarDoctor() {
    const especialidad = document.getElementById("especialidad").value;
    const url = '/DisponibilidadDoctors/BuscarPorEspecialidad';

    try {
        const response = await fetch(url + "?especialidad=" + encodeURIComponent(especialidad));
        const data = await response.json();
        const container = document.getElementById("doctor-container");
        container.innerHTML = "";

        if (Array.isArray(data)) {
            data.forEach(doctor => {
                const disponibilidadesList = Array.isArray(doctor.disponibilidades) ? doctor.disponibilidades : [];

                const card = document.createElement("div");
                card.className = "col-md-6 doctor-card";
                card.innerHTML = `
                    <div class="card mb-4 d-flex flex-column">
                        <div class="card-body">
                            <h5 class="card-title text-center" style="font-weight: bold; color: #04294F;">${doctor.nombre} ${doctor.apellido}</h5>
                            <h6 class="card-title mb-2 text-center" style="font-weight: bold; color: #000000;">${doctor.especialidad} - ${doctor.consultorio} </h6>
                            <ul class="list-group list-group-flush">
                                ${disponibilidadesList.map(d => `
                                    <li class="list-group-item" style="font-size: 1.2em; color: #04294F;">
                                        <strong>${d.dia}:</strong> ${formatearHora(d.horaInicio)} - ${formatearHora(d.horaFin)}
                                    </li>`).join('')}
                            </ul>
                        </div>
                        <div class="card-footer text-center">
                            <form action="/Citas/Create" method="get">
                                <input type="hidden" name="idDoctor" value="${doctor.idDoctor}" />
                                <button type="submit" class="btn btn-estilo">Realizar Cita</button>
                            </form>
                        </div>
                    </div>
                `;
                container.appendChild(card);
            });
        } else {
            console.error("El formato de datos no es una matriz:", data);
        }
    } catch (error) {
        console.error("Error al obtener datos:", error);
    }
}

//Funcion para filtrar los doctores por consultorio
async function filtrarDoctorConsultorio() {
    const consultorio = document.getElementById("consultorio").value;
    const url = '/DisponibilidadDoctors/BuscarPorConsultorio';

    try {

        const response = await fetch(url + "?consultorio=" + encodeURIComponent(consultorio));
        const data = await response.json();
        const container = document.getElementById("doctor-container");
        container.innerHTML = "";

        if (Array.isArray(data)) {
            data.forEach(doctor => {
                const disponibilidadesList = Array.isArray(doctor.disponibilidades) ? doctor.disponibilidades : [];

                const card = document.createElement("div");
                card.className = "col-md-6 doctor-card";
                card.innerHTML = `
                    <div class="card mb-4 d-flex flex-column">
                        <div class="card-body">
                            <h5 class="card-title text-center" style="font-weight: bold; color: #04294F;">${doctor.nombre} ${doctor.apellido}</h5>
                            <h6 class="card-title mb-2 text-center" style="font-weight: bold; color: #000000;">${doctor.especialidad}</h6>
                            <ul class="list-group list-group-flush">
                                ${disponibilidadesList.map(d => `
                                    <li class="list-group-item" style="font-size: 1.2em; color: #04294F;">
                                        <strong>${d.dia}:</strong> ${formatearHora(d.horaInicio)} - ${formatearHora(d.horaFin)}
                                    </li>`).join('')}
                            </ul>
                        </div>
                        <div class="card-footer text-center">
                            <div class="card-footer text-center">
                                <h5 style="font-weight: bold; color: #000000;">${doctor.consultorio}</h5>
                            </div>
                        </div>
                    </div>
                `;
                container.appendChild(card);
            });
        } else {
            console.error("El formato de datos no es una matriz:", data);
        }

    } catch (error) {
        console.error("Error al obtener datos: ", error);
    }
}