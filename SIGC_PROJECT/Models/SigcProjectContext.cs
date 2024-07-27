using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SIGC_PROJECT.Models;

public partial class SigcProjectContext : DbContext
{
    public SigcProjectContext()
    {
    }

    public SigcProjectContext(DbContextOptions<SigcProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Analisi> Analises { get; set; }

    public virtual DbSet<Cita> Citas { get; set; }

    public virtual DbSet<Consulta> Consultas { get; set; }

    public virtual DbSet<DisponibilidadDoctor> DisponibilidadDoctors { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<Receta> Recetas { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Secretarium> Secretaria { get; set; }

    public virtual DbSet<SolicitudesRegistro> SolicitudesRegistros { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioRol> UsuarioRols { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=conexion");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Analisi>(entity =>
        {
            entity.HasKey(e => e.AnalisisId).HasName("PK__Analisis__9F72C915FE8C8E97");

            entity.ToTable("Analisis");

            entity.Property(e => e.AnalisisId).HasColumnName("analisis_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.FechaAnalisis)
                .HasColumnType("datetime")
                .HasColumnName("fecha_analisis");
            entity.Property(e => e.PacienteId).HasColumnName("paciente_id");
            entity.Property(e => e.Resultados)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("resultados");
            entity.Property(e => e.TipoAnalisis)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tipo_analisis");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Analisis)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Analisis__doctor__74AE54BC");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Analisis)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Analisis__pacien__73BA3083");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.CitaId).HasName("PK__Citas__5AC1B05B7A805E3F");

            entity.Property(e => e.CitaId).HasColumnName("cita_id");
            entity.Property(e => e.Comentario)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("comentario");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCita)
                .HasColumnType("date");
            entity.Property(e => e.HoraCita)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NombrePaciente)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombrePaciente");
            entity.Property(e => e.NombreDoctor)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreDoctor");
            entity.Property(e => e.EspecialidadDoctor)
                  .HasMaxLength(150)
                  .IsUnicode(false)
                  .HasColumnName("especialidadDoctor");
            entity.Property(e => e.PacienteId).HasColumnName("paciente_id");
            entity.Property(e => e.SecretariaId).HasColumnName("secretaria_id");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Cita)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Citas__doctor_id__6754599E");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Cita)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Citas__comentari__66603565");

            entity.HasOne(d => d.Secretaria).WithMany(p => p.Cita)
                .HasForeignKey(d => d.SecretariaId)
                .HasConstraintName("FK__Citas__secretari__68487DD7");
        });

        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.HasKey(e => e.ConsultaId).HasName("PK__Consulta__BBB59BD85F967115");

            entity.Property(e => e.ConsultaId).HasColumnName("consulta_id");
            entity.Property(e => e.Diagnostico)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("diagnostico");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.FechaConsulta)
                .HasColumnType("datetime")
                .HasColumnName("fecha_consulta");
            entity.Property(e => e.MotivoConsulta)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("motivo_consulta");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("observaciones");
            entity.Property(e => e.PacienteId).HasColumnName("paciente_id");
            entity.Property(e => e.RecetaId).HasColumnName("receta_id");
            entity.Property(e => e.Tratamiento)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("tratamiento");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Consultas__docto__6FE99F9F");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Consultas__pacie__6EF57B66");

            entity.HasOne(d => d.Receta).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.RecetaId)
                .HasConstraintName("FK__Consultas__recet__70DDC3D8");
        });

        modelBuilder.Entity<DisponibilidadDoctor>(entity =>
        {
            entity.HasKey(e => e.DisponibilidadDoctorId).HasName("PK__Disponib__646F6E6AF14FEF6C");

            entity.ToTable("Disponibilidad_Doctor");

            entity.Property(e => e.DisponibilidadDoctorId).HasColumnName("disponibilidad_doctor_id");
            entity.Property(e => e.Dia)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("dia");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.HoraFin)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("hora_inicio");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DisponibilidadDoctors)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Disponibi__hora___6383C8BA");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctor__F3993564715AC8F8");

            entity.ToTable("Doctor");

            entity.HasIndex(e => e.Cedula, "UQ__Doctor__415B7BE55CBFCEFF").IsUnique();

            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Cedula)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("cedula");
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo_electronico");
            entity.Property(e => e.Especialidad)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("especialidad");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NumeroExequatur)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("numero_exequatur");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Doctor__id_usuar__09A971A2");

            //entity.HasOne(d => d.Secretaria).WithMany(p => p.Doctors)
            //    .HasForeignKey(d => d.SecretariaId)
            //    .HasConstraintName("FK__Doctor__secretar__60A75C0F");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.PacienteId).HasName("PK__Paciente__46FEF65671A43804");

            entity.ToTable("Paciente");

            entity.HasIndex(e => e.Cedula, "UQ__Paciente__415B7BE54A9AA003").IsUnique();

            entity.Property(e => e.PacienteId).HasColumnName("paciente_id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Cedula)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("cedula");
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo_electronico");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Edad).HasColumnName("edad");
            entity.Property(e => e.FechaNacimiento)
                .HasColumnType("date")
                .HasColumnName("fecha_nacimiento");
            entity.Property(e => e.Genero)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("genero");
            entity.Property(e => e.HistorialMedico)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("historial_medico");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Pacientes)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Paciente__id_usu__07C12930");
        });

        modelBuilder.Entity<Receta>(entity =>
        {
            entity.HasKey(e => e.RecetaId).HasName("PK__Recetas__4BCAA6B51F4A7F89");

            entity.Property(e => e.RecetaId).HasColumnName("receta_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.FechaEmision)
                .HasColumnType("datetime")
                .HasColumnName("fecha_emision");
            entity.Property(e => e.Indicaciones)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("indicaciones");
            entity.Property(e => e.Medicamentos)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("medicamentos");
            entity.Property(e => e.PacienteId).HasColumnName("paciente_id");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Receta)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recetas__doctor___6C190EBB");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Receta)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recetas__pacient__6B24EA82");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__6ABCB5E0BD6B6EF0");

            entity.ToTable("Rol");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Secretarium>(entity =>
        {
            entity.HasKey(e => e.SecretariaId).HasName("PK__Secretar__B9ECB3B0BCBBDD01");

            entity.HasIndex(e => e.Cedula, "UQ__Secretar__415B7BE52AA88F3A").IsUnique();

            entity.Property(e => e.SecretariaId).HasColumnName("secretaria_id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Cedula)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("cedula");
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo_electronico");
            entity.Property(e => e.HorarioTrabajo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("horario_trabajo");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdDoctor).HasColumnName("id_doctor");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Secretaria)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Secretari__id_us__08B54D69");

            entity.HasOne(d => d.IdDoctorNavegation).WithMany(p => p.Secretaria)
                  .HasForeignKey(d => d.IdDoctor)
                  .HasConstraintName("FK__Secretari__id_do__3B40CD36");
        });

        modelBuilder.Entity<SolicitudesRegistro>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__Solicitu__36899CEFC1FD6095");

            entity.ToTable("SolicitudesRegistro");

            entity.Property(e => e.Clave)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("clave");
            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_solicitud");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__4E3E04AD811E63CE");

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("contrasena");
            entity.Property(e => e.FechaUltimoAcceso)
                .HasColumnType("datetime")
                .HasColumnName("fecha_ultimo_acceso");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre_usuario");

            //entity.HasMany(d => d.IdRols).WithMany(p => p.IdUsuarios)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "UsuarioRol",
            //        r => r.HasOne<Rol>().WithMany()
            //            .HasForeignKey("IdRol")
            //            .HasConstraintName("FK__UsuarioRo__IdRol__03F0984C"),
            //        l => l.HasOne<Usuario>().WithMany()
            //            .HasForeignKey("IdUsuario")
            //            .HasConstraintName("FK__UsuarioRo__IdUsu__02FC7413"),
            //        j =>
            //        {
            //            j.HasKey("IdUsuario", "IdRol").HasName("PK__UsuarioR__89C12A13BA18EBDD");
            //            j.ToTable("UsuarioRol");
            //        });
        });

        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuario, e.IdRol }).HasName("PK__UsuarioR__89C12A13BA18EBDD");

            entity.ToTable("UsuarioRol");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.UsuarioRoles)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsuarioRo__IdUsu__02FC7413");

            entity.HasOne(d => d.Rol)
                .WithMany(p => p.UsuarioRoles)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsuarioRo__IdRol__03F0984C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
