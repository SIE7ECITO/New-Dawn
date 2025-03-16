﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NewDawn.Models;

#nullable disable

namespace NewDawn.Migrations
{
    [DbContext(typeof(NewDawnContext))]
    [Migration("20250313203229_AddTokenRecuperacion")]
    partial class AddTokenRecuperacion
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("NewDawn.Models.Habitacion", b =>
                {
                    b.Property<int>("Idhabitacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDHabitacion");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Idhabitacion"));

                    b.Property<bool>("EnPaquete")
                        .HasColumnType("bit");

                    b.Property<bool>("EstadoHabitacion")
                        .HasColumnType("bit");

                    b.Property<decimal>("PrecioNoche")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("TipoHabitacion")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Idhabitacion")
                        .HasName("PK__Habitaci__6B4757DA6CE77C5C");

                    b.ToTable("Habitacion", (string)null);
                });

            modelBuilder.Entity("NewDawn.Models.HabitacionReserva", b =>
                {
                    b.Property<int>("IdhabitacionReserva")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDHabitacionReserva");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdhabitacionReserva"));

                    b.Property<int>("Idhabitacion")
                        .HasColumnType("int")
                        .HasColumnName("IDHabitacion");

                    b.Property<int>("Idreserva")
                        .HasColumnType("int")
                        .HasColumnName("IDReserva");

                    b.HasKey("IdhabitacionReserva")
                        .HasName("PK__Habitaci__D32A2F4405A364A8");

                    b.HasIndex("Idhabitacion");

                    b.HasIndex("Idreserva");

                    b.ToTable("Habitacion_Reservas", (string)null);
                });

            modelBuilder.Entity("NewDawn.Models.Huesped", b =>
                {
                    b.Property<int>("Idhuesped")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDHuesped");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Idhuesped"));

                    b.Property<int>("Cchuesped")
                        .HasColumnType("int")
                        .HasColumnName("CCHuesped");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("NombreHuesped")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Idhuesped")
                        .HasName("PK__Huesped__854DF5E72CCBA18A");

                    b.HasIndex(new[] { "Cchuesped" }, "UQ__Huesped__C487C111AA632CD4")
                        .IsUnique();

                    b.ToTable("Huesped", (string)null);
                });

            modelBuilder.Entity("NewDawn.Models.HuespedReserva", b =>
                {
                    b.Property<int>("IdhuespedReserva")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDHuespedReserva");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdhuespedReserva"));

                    b.Property<int>("Idhuesped")
                        .HasColumnType("int")
                        .HasColumnName("IDHuesped");

                    b.Property<int>("Idreserva")
                        .HasColumnType("int")
                        .HasColumnName("IDReserva");

                    b.HasKey("IdhuespedReserva")
                        .HasName("PK__Huesped___80451727B8D842A2");

                    b.HasIndex("Idhuesped");

                    b.HasIndex("Idreserva");

                    b.ToTable("Huesped_Reservas", (string)null);
                });

            modelBuilder.Entity("NewDawn.Models.Pago", b =>
                {
                    b.Property<int>("Idpago")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDPago");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Idpago"));

                    b.Property<decimal>("CantidadAbono")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("CantidadPago")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<bool>("EstadoPago")
                        .HasColumnType("bit");

                    b.Property<DateOnly?>("FechaAbono")
                        .HasColumnType("date");

                    b.Property<DateOnly>("FechaPago")
                        .HasColumnType("date");

                    b.Property<int>("Idusuario")
                        .HasColumnType("int")
                        .HasColumnName("IDUsuario");

                    b.HasKey("Idpago")
                        .HasName("PK__Pago__8A5C3DEE1A5B83B4");

                    b.HasIndex("Idusuario");

                    b.ToTable("Pago", (string)null);
                });

            modelBuilder.Entity("NewDawn.Models.Paquete", b =>
                {
                    b.Property<int>("Idpaquete")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDPaquete");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Idpaquete"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("EstadoPaquete")
                        .HasColumnType("bit");

                    b.Property<string>("NombrePaquete")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("Precio")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("Idpaquete")
                        .HasName("PK__Paquete__4C29513B0196E011");

                    b.ToTable("Paquete", (string)null);
                });

            modelBuilder.Entity("NewDawn.Models.PaqueteHabitacion", b =>
                {
                    b.Property<int>("IdpaqueteHabitacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDPaqueteHabitacion");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdpaqueteHabitacion"));

                    b.Property<int>("Idhabitacion")
                        .HasColumnType("int")
                        .HasColumnName("IDHabitacion");

                    b.Property<int>("Idpaquete")
                        .HasColumnType("int")
                        .HasColumnName("IDPaquete");

                    b.HasKey("IdpaqueteHabitacion")
                        .HasName("PK__Paquete___E814B830F3CDEC87");

                    b.HasIndex("Idhabitacion");

                    b.HasIndex("Idpaquete");

                    b.ToTable("Paquete_Habitacion", (string)null);
                });

            modelBuilder.Entity("NewDawn.Models.Permiso", b =>
                {
                    b.Property<int>("Idpermisos")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDPermisos");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Idpermisos"));

                    b.Property<string>("DescripcionPermiso")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("EstadoPermisos")
                        .HasColumnType("bit");

                    b.Property<DateOnly>("FechaCambio")
                        .HasColumnType("date");

                    b.Property<string>("NombrePermiso")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Idpermisos")
                        .HasName("PK__Permisos__BAB8D9DCF40A0541");

                    b.ToTable("Permisos");
                });

            modelBuilder.Entity("NewDawn.Models.Reserva", b =>
                {
                    b.Property<int>("Idreserva")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDReserva");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Idreserva"));

                    b.Property<bool>("EstadoReserva")
                        .HasColumnType("bit");

                    b.Property<DateOnly>("FechaComienzo")
                        .HasColumnType("date");

                    b.Property<DateOnly>("FechaFin")
                        .HasColumnType("date");

                    b.Property<DateOnly>("FechaReserva")
                        .HasColumnType("date");

                    b.Property<int>("Idhabitacion")
                        .HasColumnType("int")
                        .HasColumnName("IDHabitacion");

                    b.Property<int?>("Idpago")
                        .HasColumnType("int")
                        .HasColumnName("IDPago");

                    b.Property<int?>("Idpaquete")
                        .HasColumnType("int")
                        .HasColumnName("IDPaquete");

                    b.Property<int>("Idusuario")
                        .HasColumnType("int")
                        .HasColumnName("IDUsuario");

                    b.HasKey("Idreserva")
                        .HasName("PK__Reservas__D9F2FA67135A9EB1");

                    b.HasIndex("Idhabitacion");

                    b.HasIndex("Idpago");

                    b.HasIndex("Idpaquete");

                    b.HasIndex("Idusuario");

                    b.ToTable("Reservas");
                });

            modelBuilder.Entity("NewDawn.Models.Rol", b =>
                {
                    b.Property<int>("Idrol")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDRol");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Idrol"));

                    b.Property<bool>("EstadoRol")
                        .HasColumnType("bit");

                    b.Property<string>("NombreRol")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Idrol")
                        .HasName("PK__Rol__A681ACB65620CA02");

                    b.ToTable("Rol", (string)null);
                });

            modelBuilder.Entity("NewDawn.Models.RolPermiso", b =>
                {
                    b.Property<int>("IdrolPermisos")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDRolPermisos");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdrolPermisos"));

                    b.Property<DateOnly>("FechaCreacion")
                        .HasColumnType("date");

                    b.Property<int>("Idpermisos")
                        .HasColumnType("int")
                        .HasColumnName("IDPermisos");

                    b.Property<int>("Idrol")
                        .HasColumnType("int")
                        .HasColumnName("IDRol");

                    b.HasKey("IdrolPermisos")
                        .HasName("PK__Rol_Perm__7FDE3C5D0AF557BF");

                    b.HasIndex("Idpermisos");

                    b.HasIndex("Idrol");

                    b.ToTable("Rol_Permisos", (string)null);
                });

            modelBuilder.Entity("NewDawn.Models.Servicio", b =>
                {
                    b.Property<int>("Idservicio")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDServicio");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Idservicio"));

                    b.Property<string>("DescripcionServicio")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("EstadoServicio")
                        .HasColumnType("bit");

                    b.Property<DateOnly>("FechaCreacion")
                        .HasColumnType("date");

                    b.Property<string>("NombreServicio")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("ValorServicio")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("Idservicio")
                        .HasName("PK__Servicio__3CCE74164DE09097");

                    b.ToTable("Servicio", (string)null);
                });

            modelBuilder.Entity("NewDawn.Models.Usuario", b =>
                {
                    b.Property<int>("Idusuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IDUsuario");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Idusuario"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("Ccusuario")
                        .HasColumnType("int")
                        .HasColumnName("CCUsuario");

                    b.Property<string>("ContraseñaUsuario")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("EstadoUsuario")
                        .HasColumnType("bit");

                    b.Property<int>("Idrol")
                        .HasColumnType("int")
                        .HasColumnName("IDRol");

                    b.Property<string>("NombreUsuario")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime?>("TokenExpiracion")
                        .HasColumnType("datetime2");

                    b.Property<string>("TokenRecuperacion")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Idusuario")
                        .HasName("PK__Usuarios__52311169888DEFB5");

                    b.HasIndex("Idrol");

                    b.HasIndex(new[] { "Ccusuario" }, "UQ__Usuarios__970A592B05EE342F")
                        .IsUnique();

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("ReservaServicio", b =>
                {
                    b.Property<int>("Idreserva")
                        .HasColumnType("int")
                        .HasColumnName("IDReserva");

                    b.Property<int>("Idservicio")
                        .HasColumnType("int")
                        .HasColumnName("IDServicio");

                    b.HasKey("Idreserva", "Idservicio")
                        .HasName("PK__Reserva___AA3E1D265ACEC5D8");

                    b.HasIndex("Idservicio");

                    b.ToTable("Reserva_Servicio", (string)null);
                });

            modelBuilder.Entity("NewDawn.Models.HabitacionReserva", b =>
                {
                    b.HasOne("NewDawn.Models.Habitacion", "IdhabitacionNavigation")
                        .WithMany("HabitacionReservas")
                        .HasForeignKey("Idhabitacion")
                        .IsRequired()
                        .HasConstraintName("FK__Habitacio__IDHab__71D1E811");

                    b.HasOne("NewDawn.Models.Reserva", "IdreservaNavigation")
                        .WithMany("HabitacionReservas")
                        .HasForeignKey("Idreserva")
                        .IsRequired()
                        .HasConstraintName("FK__Habitacio__IDRes__72C60C4A");

                    b.Navigation("IdhabitacionNavigation");

                    b.Navigation("IdreservaNavigation");
                });

            modelBuilder.Entity("NewDawn.Models.HuespedReserva", b =>
                {
                    b.HasOne("NewDawn.Models.Huesped", "IdhuespedNavigation")
                        .WithMany("HuespedReservas")
                        .HasForeignKey("Idhuesped")
                        .IsRequired()
                        .HasConstraintName("FK__Huesped_R__IDHue__6EF57B66");

                    b.HasOne("NewDawn.Models.Reserva", "IdreservaNavigation")
                        .WithMany("HuespedReservas")
                        .HasForeignKey("Idreserva")
                        .IsRequired()
                        .HasConstraintName("FK__Huesped_R__IDRes__6E01572D");

                    b.Navigation("IdhuespedNavigation");

                    b.Navigation("IdreservaNavigation");
                });

            modelBuilder.Entity("NewDawn.Models.Pago", b =>
                {
                    b.HasOne("NewDawn.Models.Usuario", "IdusuarioNavigation")
                        .WithMany("Pagos")
                        .HasForeignKey("Idusuario")
                        .IsRequired()
                        .HasConstraintName("FK__Pago__IDUsuario__59FA5E80");

                    b.Navigation("IdusuarioNavigation");
                });

            modelBuilder.Entity("NewDawn.Models.PaqueteHabitacion", b =>
                {
                    b.HasOne("NewDawn.Models.Habitacion", "IdhabitacionNavigation")
                        .WithMany("PaqueteHabitacions")
                        .HasForeignKey("Idhabitacion")
                        .IsRequired()
                        .HasConstraintName("FK__Paquete_H__IDHab__619B8048");

                    b.HasOne("NewDawn.Models.Paquete", "IdpaqueteNavigation")
                        .WithMany("PaqueteHabitacions")
                        .HasForeignKey("Idpaquete")
                        .IsRequired()
                        .HasConstraintName("FK__Paquete_H__IDPaq__60A75C0F");

                    b.Navigation("IdhabitacionNavigation");

                    b.Navigation("IdpaqueteNavigation");
                });

            modelBuilder.Entity("NewDawn.Models.Reserva", b =>
                {
                    b.HasOne("NewDawn.Models.Habitacion", "IdhabitacionNavigation")
                        .WithMany("Reservas")
                        .HasForeignKey("Idhabitacion")
                        .IsRequired()
                        .HasConstraintName("FK__Reservas__IDHabi__656C112C");

                    b.HasOne("NewDawn.Models.Pago", "IdpagoNavigation")
                        .WithMany("Reservas")
                        .HasForeignKey("Idpago")
                        .HasConstraintName("FK__Reservas__IDPago__6754599E");

                    b.HasOne("NewDawn.Models.Paquete", "IdpaqueteNavigation")
                        .WithMany("Reservas")
                        .HasForeignKey("Idpaquete")
                        .HasConstraintName("FK__Reservas__IDPaqu__66603565");

                    b.HasOne("NewDawn.Models.Usuario", "IdusuarioNavigation")
                        .WithMany("Reservas")
                        .HasForeignKey("Idusuario")
                        .IsRequired()
                        .HasConstraintName("FK__Reservas__IDUsua__6477ECF3");

                    b.Navigation("IdhabitacionNavigation");

                    b.Navigation("IdpagoNavigation");

                    b.Navigation("IdpaqueteNavigation");

                    b.Navigation("IdusuarioNavigation");
                });

            modelBuilder.Entity("NewDawn.Models.RolPermiso", b =>
                {
                    b.HasOne("NewDawn.Models.Permiso", "IdpermisosNavigation")
                        .WithMany("RolPermisos")
                        .HasForeignKey("Idpermisos")
                        .IsRequired()
                        .HasConstraintName("FK__Rol_Permi__IDPer__5535A963");

                    b.HasOne("NewDawn.Models.Rol", "IdrolNavigation")
                        .WithMany("RolPermisos")
                        .HasForeignKey("Idrol")
                        .IsRequired()
                        .HasConstraintName("FK__Rol_Permi__IDRol__5441852A");

                    b.Navigation("IdpermisosNavigation");

                    b.Navigation("IdrolNavigation");
                });

            modelBuilder.Entity("NewDawn.Models.Usuario", b =>
                {
                    b.HasOne("NewDawn.Models.Rol", "IdrolNavigation")
                        .WithMany("Usuarios")
                        .HasForeignKey("Idrol")
                        .IsRequired()
                        .HasConstraintName("FK__Usuarios__IDRol__4CA06362");

                    b.Navigation("IdrolNavigation");
                });

            modelBuilder.Entity("ReservaServicio", b =>
                {
                    b.HasOne("NewDawn.Models.Reserva", null)
                        .WithMany()
                        .HasForeignKey("Idreserva")
                        .IsRequired()
                        .HasConstraintName("FK__Reserva_S__IDRes__6A30C649");

                    b.HasOne("NewDawn.Models.Servicio", null)
                        .WithMany()
                        .HasForeignKey("Idservicio")
                        .IsRequired()
                        .HasConstraintName("FK__Reserva_S__IDSer__6B24EA82");
                });

            modelBuilder.Entity("NewDawn.Models.Habitacion", b =>
                {
                    b.Navigation("HabitacionReservas");

                    b.Navigation("PaqueteHabitacions");

                    b.Navigation("Reservas");
                });

            modelBuilder.Entity("NewDawn.Models.Huesped", b =>
                {
                    b.Navigation("HuespedReservas");
                });

            modelBuilder.Entity("NewDawn.Models.Pago", b =>
                {
                    b.Navigation("Reservas");
                });

            modelBuilder.Entity("NewDawn.Models.Paquete", b =>
                {
                    b.Navigation("PaqueteHabitacions");

                    b.Navigation("Reservas");
                });

            modelBuilder.Entity("NewDawn.Models.Permiso", b =>
                {
                    b.Navigation("RolPermisos");
                });

            modelBuilder.Entity("NewDawn.Models.Reserva", b =>
                {
                    b.Navigation("HabitacionReservas");

                    b.Navigation("HuespedReservas");
                });

            modelBuilder.Entity("NewDawn.Models.Rol", b =>
                {
                    b.Navigation("RolPermisos");

                    b.Navigation("Usuarios");
                });

            modelBuilder.Entity("NewDawn.Models.Usuario", b =>
                {
                    b.Navigation("Pagos");

                    b.Navigation("Reservas");
                });
#pragma warning restore 612, 618
        }
    }
}
