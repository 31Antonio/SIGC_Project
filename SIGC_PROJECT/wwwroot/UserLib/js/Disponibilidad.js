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

//Funcion para filtrar los doctores
async function filtrarDoctor() {
    const especialidad = document.getElementById("especialidad").value;
    const url = '/DisponibilidadDoctors/BuscarPorEspecialidad';

    fetch(url + "?especialidad=" + encodeURIComponent(especialidad))
        .then(response => response.json())
        .then(data => {
            console.log(data); // Verificar la estructura de datos recibida
            const container = document.getElementById("doctor-container");
            container.innerHTML = "";

            if (Array.isArray(data)) {
                data.forEach(doctor => {
                    // Verificar que Disponibilidades es un array
                    const disponibilidadesList = Array.isArray(doctor.disponibilidades) ? doctor.disponibilidades : [];

                    const card = document.createElement("div");
                    card.className = "col-md-4 doctor-card";
                    card.innerHTML = `
                                <div class="card mb-4 d-flex flex-column">
                                    <div class="card-body">
                                        <h5 class="card-title">${doctor.nombre} ${doctor.apellido}</h5>
                                        <h6 class="card-subtitle mb-2 text-muted">${doctor.especialidad}</h6>
                                        <ul class="list-group list-group-flush">
                                            ${disponibilidadesList.map(d => `
                                                <li class="list-group-item">
                                                    <strong>${d.dia}:</strong> ${formatearHora(d.horaInicio)}  - ${formatearHora(d.horaFin)}
                                                </li>`).join('')}
                                        </ul>
                                    </div>
                                    <div class="card-footer text-center">
                                        <button class="btn btn-primary">Button 1</button>
                                        <button class="btn btn-secondary">Button 2</button>
                                    </div>
                                </div>
                            `;
                    container.appendChild(card);
                });
            } else {
                console.error("El formato de datos no es una matriz:", data);
            }
        })
        .catch(error => {
            console.error("Error al obtener datos:", error);
        });
}