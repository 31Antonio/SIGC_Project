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

    public virtual DbSet<Modulo> Modulos { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Receta> Recetas { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Secretarium> Secretaria { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

   //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
     //   => optionsBuilder.UseSqlServer("Server=JD;Database=SIGC_PROJECT;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Analisi>(entity =>
        {
            entity.HasKey(e => e.AnalisisId).HasName("PK__Analisis__9F72C9153ACBE0D6");

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
                .HasConstraintName("FK__Analisis__doctor__628FA481");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Analisis)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Analisis__pacien__619B8048");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.CitaId).HasName("PK__Citas__5AC1B05B045773FB");

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
            entity.Property(e => e.PacienteId).HasColumnName("paciente_id");
            entity.Property(e => e.SecretariaId).HasColumnName("secretaria_id");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Cita)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Citas__doctor_id__5535A963");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Cita)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Citas__comentari__5441852A");

            entity.HasOne(d => d.Secretaria).WithMany(p => p.Cita)
                .HasForeignKey(d => d.SecretariaId)
                .HasConstraintName("FK__Citas__secretari__5629CD9C");
        });

        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.HasKey(e => e.ConsultaId).HasName("PK__Consulta__BBB59BD862A99069");

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
                .HasConstraintName("FK__Consultas__docto__5DCAEF64");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Consultas__pacie__5CD6CB2B");

            entity.HasOne(d => d.Receta).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.RecetaId)
                .HasConstraintName("FK__Consultas__recet__5EBF139D");
        });

        modelBuilder.Entity<DisponibilidadDoctor>(entity =>
        {
            entity.HasKey(e => e.DisponibilidadDoctorId).HasName("PK__Disponib__646F6E6A3C083770");

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
                .HasConstraintName("FK__Disponibi__hora___5165187F");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctor__F39935643703E20C");

            entity.ToTable("Doctor");

            entity.HasIndex(e => e.Cedula, "UQ__Doctor__415B7BE517D4D4C8").IsUnique();

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
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NumeroExequatur)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("numero_exequatur");
            entity.Property(e => e.SecretariaId).HasColumnName("secretaria_id");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.Secretaria).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SecretariaId)
                .HasConstraintName("FK__Doctor__secretar__4E88ABD4");
        });

        modelBuilder.Entity<Modulo>(entity =>
        {
            entity.HasKey(e => e.IdModulo).HasName("PK__Modulo__B2584DFC36EEEDAB");

            entity.ToTable("Modulo");

            entity.Property(e => e.IdModulo).HasColumnName("id_modulo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasMany(d => d.IdPermisos).WithMany(p => p.IdModulos)
                .UsingEntity<Dictionary<string, object>>(
                    "ModuloPermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("IdPermiso")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Modulo_Pe__id_pe__3E52440B"),
                    l => l.HasOne<Modulo>().WithMany()
                        .HasForeignKey("IdModulo")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Modulo_Pe__id_mo__3D5E1FD2"),
                    j =>
                    {
                        j.HasKey("IdModulo", "IdPermiso").HasName("PK__Modulo_P__5070BFD8A1BBEAE4");
                        j.ToTable("Modulo_Permiso");
                        j.IndexerProperty<int>("IdModulo").HasColumnName("id_modulo");
                        j.IndexerProperty<int>("IdPermiso").HasColumnName("id_permiso");
                    });
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.PacienteId).HasName("PK__Paciente__46FEF656510F9A33");

            entity.ToTable("Paciente");

            entity.HasIndex(e => e.Cedula, "UQ__Paciente__415B7BE5DBD8CD11").IsUnique();

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
                .HasColumnType("datetime")
                .HasColumnName("fecha_nacimiento");
            entity.Property(e => e.Genero)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("genero");
            entity.Property(e => e.HistorialMedico)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("historial_medico");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PK__Permiso__228F224F19D3A099");

            entity.ToTable("Permiso");

            entity.Property(e => e.IdPermiso).HasColumnName("id_permiso");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasMany(d => d.IdRols).WithMany(p => p.IdPermisos)
                .UsingEntity<Dictionary<string, object>>(
                    "PermisoRol",
                    r => r.HasOne<Rol>().WithMany()
                        .HasForeignKey("IdRol")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Permiso_R__id_ro__4222D4EF"),
                    l => l.HasOne<Permiso>().WithMany()
                        .HasForeignKey("IdPermiso")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Permiso_R__id_pe__412EB0B6"),
                    j =>
                    {
                        j.HasKey("IdPermiso", "IdRol").HasName("PK__Permiso___3424E9114FA0A54B");
                        j.ToTable("Permiso_Rol");
                        j.IndexerProperty<int>("IdPermiso").HasColumnName("id_permiso");
                        j.IndexerProperty<int>("IdRol").HasColumnName("id_rol");
                    });
        });

        modelBuilder.Entity<Receta>(entity =>
        {
            entity.HasKey(e => e.RecetaId).HasName("PK__Recetas__4BCAA6B5B6196397");

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
                .HasConstraintName("FK__Recetas__doctor___59FA5E80");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Receta)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recetas__pacient__59063A47");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__6ABCB5E0746522E2");

            entity.ToTable("Rol");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Secretarium>(entity =>
        {
            entity.HasKey(e => e.SecretariaId).HasName("PK__Secretar__B9ECB3B0F2586651");

            entity.HasIndex(e => e.Cedula, "UQ__Secretar__415B7BE52D7A0506").IsUnique();

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
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__4E3E04AD35671CA8");

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contrasena");
            entity.Property(e => e.FechaUltimoAcceso)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("fecha_ultimo_acceso");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre_usuario");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuario__id_rol__44FF419A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
