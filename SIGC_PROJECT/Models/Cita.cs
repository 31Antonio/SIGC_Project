﻿using System;
using System.Collections.Generic;

namespace SIGC_PROJECT.Models;

public partial class Cita
{
    public int CitaId { get; set; }

    public int PacienteId { get; set; }

    public int DoctorId { get; set; }

    public int? SecretariaId { get; set; }

    public string? Estado { get; set; }

    public string? Comentario { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Paciente Paciente { get; set; } = null!;

    public virtual Secretarium? Secretaria { get; set; }
}
