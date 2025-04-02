using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NewDawn.Models;

public partial class NewDawnContext : DbContext
{
    public NewDawnContext()
    {
    }

    public NewDawnContext(DbContextOptions<NewDawnContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Comodidades> Comodidades { get; set; }

    public virtual DbSet<Habitacion> Habitacions { get; set; }

    public virtual DbSet<HabitacionComodidade> HabitacionComodidades { get; set; }

    public virtual DbSet<HabitacionReserva> HabitacionReservas { get; set; }

    public virtual DbSet<Huesped> Huespeds { get; set; }

    public virtual DbSet<HuespedReserva> HuespedReservas { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Paquete> Paquetes { get; set; }

    public virtual DbSet<PaqueteHabitacion> PaqueteHabitacions { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<RolPermiso> RolPermisos { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<ServicioPaquete> ServicioPaquetes { get; set; }
    public virtual DbSet<ReservaServicio> ReservaServicios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=tcp:new-dawn.database.windows.net,1433;Initial Catalog=NewDawn;Persist Security Info=False;User ID=newdawnadmin;Password=New123dawnAdmin;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder); // Mantener otras configuraciones si existen

        // Configuración explícita para ReservaServicio
        modelBuilder.Entity<ReservaServicio>(entity =>
        {
            // Especificar esquema y nombre de tabla exacto
            entity.ToTable("Reserva_Servicio", "dbo");

            // Clave primaria compuesta
            entity.HasKey(rs => new { rs.Idreserva, rs.Idservicio })
                  .HasName("PK_Reserva_Servicio"); // Opcional: nombre de la PK

            // Mapeo de columnas
            entity.Property(rs => rs.Idreserva)
                  .HasColumnName("IDReserva");

            entity.Property(rs => rs.Idservicio)
                  .HasColumnName("IDServicio");

            // Relaciones
            entity.HasOne(rs => rs.IdreservaNavigation)
                  .WithMany(r => r.ReservaServicios)
                  .HasForeignKey(rs => rs.Idreserva)
                  .HasConstraintName("FK_ReservaServicio_Reserva") // Opcional
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rs => rs.IdservicioNavigation)
                  .WithMany(s => s.ReservaServicios)
                  .HasForeignKey(rs => rs.Idservicio)
                  .HasConstraintName("FK_ReservaServicio_Servicio") // Opcional
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Resto de tus configuraciones...
        modelBuilder.Entity<Comodidades>(entity =>
        {
            entity.ToTable("Comodidades", "dbo"); // Asegúrate de especificar esquema
            entity.HasKey(e => e.IdComodidades).HasName("PK__Comodida__A95B74EAEBFAC6CB");


            modelBuilder.Entity<Comodidades>(entity =>
        {
            entity.HasKey(e => e.IdComodidades).HasName("PK__Comodida__A95B74EAEBFAC6CB");

            entity.Property(e => e.IdComodidades).HasColumnName("idComodidades");
            entity.Property(e => e.DescripcionComodidad)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcionComodidad");
            entity.Property(e => e.EstadoComodidad).HasColumnName("estadoComodidad");
            entity.Property(e => e.NombreComodidades)
                .HasMaxLength(255)
                .IsUnicode(false);
        });



            modelBuilder.Entity<Habitacion>(entity =>
            {
                entity.HasKey(e => e.Idhabitacion).HasName("PK__Habitaci__6B4757DA6CE77C5C");

                entity.ToTable("Habitacion");

                entity.Property(e => e.Idhabitacion).HasColumnName("IDHabitacion");
                entity.Property(e => e.PrecioNoche).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.TipoHabitacion)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HabitacionComodidade>(entity =>
            {
                entity.HasKey(e => e.IdHabitacionComodidades).HasName("PK__Habitaci__7F71298BAFDC4AEA");

                entity.ToTable("Habitacion_Comodidades");

                entity.Property(e => e.IdHabitacionComodidades).HasColumnName("idHabitacion_comodidades");
                entity.Property(e => e.IdComodidades).HasColumnName("idComodidades");
                entity.Property(e => e.IdHabitacion).HasColumnName("idHabitacion");

                entity.HasOne(d => d.IdComodidadesNavigation).WithMany(p => p.HabitacionComodidades)
                    .HasForeignKey(d => d.IdComodidades)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Habitacion_Comodidades_Comodidades");

                entity.HasOne(d => d.IdHabitacionNavigation).WithMany(p => p.HabitacionComodidades)
                    .HasForeignKey(d => d.IdHabitacion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Habitacion_Comodidades_Habitacion");
            });

            modelBuilder.Entity<HabitacionReserva>(entity =>
            {
                entity.HasKey(e => e.IdhabitacionReserva).HasName("PK__Habitaci__D32A2F4405A364A8");

                entity.ToTable("Habitacion_Reservas");

                entity.Property(e => e.IdhabitacionReserva).HasColumnName("IDHabitacionReserva");
                entity.Property(e => e.Idhabitacion).HasColumnName("IDHabitacion");
                entity.Property(e => e.Idreserva).HasColumnName("IDReserva");

                entity.HasOne(d => d.IdhabitacionNavigation).WithMany(p => p.HabitacionReservas)
                    .HasForeignKey(d => d.Idhabitacion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Habitacio__IDHab__71D1E811");

                entity.HasOne(d => d.IdreservaNavigation).WithMany(p => p.HabitacionReservas)
                    .HasForeignKey(d => d.Idreserva)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Habitacio__IDRes__72C60C4A");
            });

            modelBuilder.Entity<Huesped>(entity =>
            {
                entity.HasKey(e => e.Idhuesped).HasName("PK__Huesped__854DF5E72CCBA18A");

                entity.ToTable("Huesped");

                entity.HasIndex(e => e.Cchuesped, "UQ__Huesped__C487C111AA632CD4").IsUnique();

                entity.Property(e => e.Idhuesped).HasColumnName("IDHuesped");
                entity.Property(e => e.Cchuesped).HasColumnName("CCHuesped");
                entity.Property(e => e.Correo)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.NombreHuesped)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HuespedReserva>(entity =>
            {
                entity.HasKey(e => e.IdhuespedReserva).HasName("PK__Huesped___80451727B8D842A2");

                entity.ToTable("Huesped_Reservas");

                entity.Property(e => e.IdhuespedReserva).HasColumnName("IDHuespedReserva");
                entity.Property(e => e.Idhuesped).HasColumnName("IDHuesped");
                entity.Property(e => e.Idreserva).HasColumnName("IDReserva");

                entity.HasOne(d => d.IdhuespedNavigation).WithMany(p => p.HuespedReservas)
                    .HasForeignKey(d => d.Idhuesped)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Huesped_R__IDHue__6EF57B66");

                entity.HasOne(d => d.IdreservaNavigation).WithMany(p => p.HuespedReservas)
                    .HasForeignKey(d => d.Idreserva)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Huesped_R__IDRes__6E01572D");
            });

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasKey(e => e.Idpago).HasName("PK__Pago__8A5C3DEE1A5B83B4");

                entity.ToTable("Pago");

                entity.Property(e => e.Idpago).HasColumnName("IDPago");
                entity.Property(e => e.CantidadAbono).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.CantidadPago).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");

                entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Pagos)
                    .HasForeignKey(d => d.Idusuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Pago__IDUsuario__59FA5E80");
            });

            modelBuilder.Entity<Paquete>(entity =>
            {
                entity.HasKey(e => e.Idpaquete).HasName("PK__Paquete__4C29513B0196E011");

                entity.ToTable("Paquete");

                entity.Property(e => e.Idpaquete).HasColumnName("IDPaquete");
                entity.Property(e => e.Descripcion).HasColumnType("text");
                entity.Property(e => e.NombrePaquete)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<PaqueteHabitacion>(entity =>
            {
                entity.HasKey(e => e.IdpaqueteHabitacion).HasName("PK__Paquete___E814B830F3CDEC87");

                entity.ToTable("Paquete_Habitacion");

                entity.Property(e => e.IdpaqueteHabitacion).HasColumnName("IDPaqueteHabitacion");
                entity.Property(e => e.Idhabitacion).HasColumnName("IDHabitacion");
                entity.Property(e => e.Idpaquete).HasColumnName("IDPaquete");

                entity.HasOne(d => d.IdhabitacionNavigation).WithMany(p => p.PaqueteHabitacions)
                    .HasForeignKey(d => d.Idhabitacion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Paquete_H__IDHab__619B8048");

                entity.HasOne(d => d.IdpaqueteNavigation).WithMany(p => p.PaqueteHabitacions)
                    .HasForeignKey(d => d.Idpaquete)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Paquete_H__IDPaq__60A75C0F");
            });

            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.HasKey(e => e.Idpermisos).HasName("PK__Permisos__BAB8D9DCF40A0541");

                entity.Property(e => e.Idpermisos).HasColumnName("IDPermisos");
                entity.Property(e => e.DescripcionPermiso).HasColumnType("text");
                entity.Property(e => e.NombrePermiso)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasKey(e => e.Idreserva).HasName("PK__Reservas__D9F2FA67135A9EB1");

                entity.Property(e => e.Idreserva).HasColumnName("IDReserva");
                entity.Property(e => e.Idhabitacion).HasColumnName("IDHabitacion");
                entity.Property(e => e.Idpago).HasColumnName("IDPago");
                entity.Property(e => e.Idpaquete).HasColumnName("IDPaquete");
                entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");

                entity.HasOne(d => d.IdhabitacionNavigation).WithMany(p => p.Reservas)
                    .HasForeignKey(d => d.Idhabitacion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Reservas__IDHabi__656C112C");

                entity.HasOne(d => d.IdpagoNavigation).WithMany(p => p.Reservas)
                    .HasForeignKey(d => d.Idpago)
                    .HasConstraintName("FK__Reservas__IDPago__6754599E");

                entity.HasOne(d => d.IdpaqueteNavigation).WithMany(p => p.Reservas)
                    .HasForeignKey(d => d.Idpaquete)
                    .HasConstraintName("FK__Reservas__IDPaqu__66603565");

                entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Reservas)
                    .HasForeignKey(d => d.Idusuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Reservas__IDUsua__6477ECF3");

                entity.HasMany(d => d.Idservicios).WithMany(p => p.Idreservas)
                    .UsingEntity<Dictionary<string, object>>(
                        "ReservaServicio",
                        r => r.HasOne<Servicio>().WithMany()
                            .HasForeignKey("Idservicio")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__Reserva_S__IDSer__6B24EA82"),
                        l => l.HasOne<Reserva>().WithMany()
                            .HasForeignKey("Idreserva")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__Reserva_S__IDRes__6A30C649"),
                        j =>
                        {
                            j.HasKey("Idreserva", "Idservicio").HasName("PK__Reserva___AA3E1D265ACEC5D8");
                            j.ToTable("Reserva_Servicio");
                            j.IndexerProperty<int>("Idreserva").HasColumnName("IDReserva");
                            j.IndexerProperty<int>("Idservicio").HasColumnName("IDServicio");
                        });
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.Idrol).HasName("PK__Rol__A681ACB65620CA02");

                entity.ToTable("Rol");

                entity.Property(e => e.Idrol).HasColumnName("IDRol");
                entity.Property(e => e.NombreRol)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RolPermiso>(entity =>
            {
                entity.HasKey(e => e.IdrolPermisos).HasName("PK__Rol_Perm__7FDE3C5D0AF557BF");

                entity.ToTable("Rol_Permisos");

                entity.Property(e => e.IdrolPermisos).HasColumnName("IDRolPermisos");
                entity.Property(e => e.Idpermisos).HasColumnName("IDPermisos");
                entity.Property(e => e.Idrol).HasColumnName("IDRol");

                entity.HasOne(d => d.IdpermisosNavigation).WithMany(p => p.RolPermisos)
                    .HasForeignKey(d => d.Idpermisos)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Rol_Permi__IDPer__5535A963");

                entity.HasOne(d => d.IdrolNavigation).WithMany(p => p.RolPermisos)
                    .HasForeignKey(d => d.Idrol)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Rol_Permi__IDRol__5441852A");
            });

            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.HasKey(e => e.Idservicio).HasName("PK__Servicio__3CCE74164DE09097");

                entity.ToTable("Servicio");

                entity.Property(e => e.Idservicio).HasColumnName("IDServicio");
                entity.Property(e => e.DescripcionServicio).HasColumnType("text");
                entity.Property(e => e.NombreServicio)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.ValorServicio).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<ServicioPaquete>(entity =>
            {
                entity.HasKey(e => e.IdservicioPaquete).HasName("PK__Servicio__C0383A8F3CAD24C0");

                entity.ToTable("Servicio_Paquete");

                entity.Property(e => e.IdservicioPaquete).HasColumnName("IDServicio_Paquete");
                entity.Property(e => e.Idpaquete).HasColumnName("IDPaquete");
                entity.Property(e => e.Idservicio).HasColumnName("IDServicio");

                entity.HasOne(d => d.IdpaqueteNavigation).WithMany(p => p.ServicioPaquetes)
                    .HasForeignKey(d => d.Idpaquete)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServicioPaquete_Paquete");

                entity.HasOne(d => d.IdservicioNavigation).WithMany(p => p.ServicioPaquetes)
                    .HasForeignKey(d => d.Idservicio)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServicioPaquete_Servicio");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Idusuario).HasName("PK__Usuarios__52311169888DEFB5");

                entity.HasIndex(e => e.Ccusuario, "UQ__Usuarios__970A592B05EE342F").IsUnique();

                entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");
                entity.Property(e => e.Apellido)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Ccusuario).HasColumnName("CCUsuario");
                entity.Property(e => e.ContraseñaUsuario)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.Correo)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.Idrol).HasColumnName("IDRol");
                entity.Property(e => e.NombreUsuario)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.NumeroTelUsuario)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdrolNavigation).WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.Idrol)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Usuarios__IDRol__4CA06362");
            });

            OnModelCreatingPartial(modelBuilder);
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
