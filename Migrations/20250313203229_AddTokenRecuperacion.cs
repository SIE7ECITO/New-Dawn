using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewDawn.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenRecuperacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Habitacion",
                columns: table => new
                {
                    IDHabitacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoHabitacion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    EstadoHabitacion = table.Column<bool>(type: "bit", nullable: false),
                    PrecioNoche = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnPaquete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Habitaci__6B4757DA6CE77C5C", x => x.IDHabitacion);
                });

            migrationBuilder.CreateTable(
                name: "Huesped",
                columns: table => new
                {
                    IDHuesped = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CCHuesped = table.Column<int>(type: "int", nullable: false),
                    NombreHuesped = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Huesped__854DF5E72CCBA18A", x => x.IDHuesped);
                });

            migrationBuilder.CreateTable(
                name: "Paquete",
                columns: table => new
                {
                    IDPaquete = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombrePaquete = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstadoPaquete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Paquete__4C29513B0196E011", x => x.IDPaquete);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    IDPermisos = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombrePermiso = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    DescripcionPermiso = table.Column<string>(type: "text", nullable: false),
                    EstadoPermisos = table.Column<bool>(type: "bit", nullable: false),
                    FechaCambio = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Permisos__BAB8D9DCF40A0541", x => x.IDPermisos);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    IDRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreRol = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    EstadoRol = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rol__A681ACB65620CA02", x => x.IDRol);
                });

            migrationBuilder.CreateTable(
                name: "Servicio",
                columns: table => new
                {
                    IDServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreServicio = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    DescripcionServicio = table.Column<string>(type: "text", nullable: false),
                    ValorServicio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateOnly>(type: "date", nullable: false),
                    EstadoServicio = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Servicio__3CCE74164DE09097", x => x.IDServicio);
                });

            migrationBuilder.CreateTable(
                name: "Paquete_Habitacion",
                columns: table => new
                {
                    IDPaqueteHabitacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDPaquete = table.Column<int>(type: "int", nullable: false),
                    IDHabitacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Paquete___E814B830F3CDEC87", x => x.IDPaqueteHabitacion);
                    table.ForeignKey(
                        name: "FK__Paquete_H__IDHab__619B8048",
                        column: x => x.IDHabitacion,
                        principalTable: "Habitacion",
                        principalColumn: "IDHabitacion");
                    table.ForeignKey(
                        name: "FK__Paquete_H__IDPaq__60A75C0F",
                        column: x => x.IDPaquete,
                        principalTable: "Paquete",
                        principalColumn: "IDPaquete");
                });

            migrationBuilder.CreateTable(
                name: "Rol_Permisos",
                columns: table => new
                {
                    IDRolPermisos = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDRol = table.Column<int>(type: "int", nullable: false),
                    IDPermisos = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rol_Perm__7FDE3C5D0AF557BF", x => x.IDRolPermisos);
                    table.ForeignKey(
                        name: "FK__Rol_Permi__IDPer__5535A963",
                        column: x => x.IDPermisos,
                        principalTable: "Permisos",
                        principalColumn: "IDPermisos");
                    table.ForeignKey(
                        name: "FK__Rol_Permi__IDRol__5441852A",
                        column: x => x.IDRol,
                        principalTable: "Rol",
                        principalColumn: "IDRol");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IDUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CCUsuario = table.Column<int>(type: "int", nullable: false),
                    NombreUsuario = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    ContraseñaUsuario = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    EstadoUsuario = table.Column<bool>(type: "bit", nullable: false),
                    IDRol = table.Column<int>(type: "int", nullable: false),
                    TokenRecuperacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenExpiracion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuarios__52311169888DEFB5", x => x.IDUsuario);
                    table.ForeignKey(
                        name: "FK__Usuarios__IDRol__4CA06362",
                        column: x => x.IDRol,
                        principalTable: "Rol",
                        principalColumn: "IDRol");
                });

            migrationBuilder.CreateTable(
                name: "Pago",
                columns: table => new
                {
                    IDPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDUsuario = table.Column<int>(type: "int", nullable: false),
                    CantidadPago = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantidadAbono = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaPago = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaAbono = table.Column<DateOnly>(type: "date", nullable: true),
                    EstadoPago = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pago__8A5C3DEE1A5B83B4", x => x.IDPago);
                    table.ForeignKey(
                        name: "FK__Pago__IDUsuario__59FA5E80",
                        column: x => x.IDUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IDUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    IDReserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDUsuario = table.Column<int>(type: "int", nullable: false),
                    IDHabitacion = table.Column<int>(type: "int", nullable: false),
                    IDPaquete = table.Column<int>(type: "int", nullable: true),
                    IDPago = table.Column<int>(type: "int", nullable: true),
                    FechaReserva = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaComienzo = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: false),
                    EstadoReserva = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reservas__D9F2FA67135A9EB1", x => x.IDReserva);
                    table.ForeignKey(
                        name: "FK__Reservas__IDHabi__656C112C",
                        column: x => x.IDHabitacion,
                        principalTable: "Habitacion",
                        principalColumn: "IDHabitacion");
                    table.ForeignKey(
                        name: "FK__Reservas__IDPago__6754599E",
                        column: x => x.IDPago,
                        principalTable: "Pago",
                        principalColumn: "IDPago");
                    table.ForeignKey(
                        name: "FK__Reservas__IDPaqu__66603565",
                        column: x => x.IDPaquete,
                        principalTable: "Paquete",
                        principalColumn: "IDPaquete");
                    table.ForeignKey(
                        name: "FK__Reservas__IDUsua__6477ECF3",
                        column: x => x.IDUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IDUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Habitacion_Reservas",
                columns: table => new
                {
                    IDHabitacionReserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDHabitacion = table.Column<int>(type: "int", nullable: false),
                    IDReserva = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Habitaci__D32A2F4405A364A8", x => x.IDHabitacionReserva);
                    table.ForeignKey(
                        name: "FK__Habitacio__IDHab__71D1E811",
                        column: x => x.IDHabitacion,
                        principalTable: "Habitacion",
                        principalColumn: "IDHabitacion");
                    table.ForeignKey(
                        name: "FK__Habitacio__IDRes__72C60C4A",
                        column: x => x.IDReserva,
                        principalTable: "Reservas",
                        principalColumn: "IDReserva");
                });

            migrationBuilder.CreateTable(
                name: "Huesped_Reservas",
                columns: table => new
                {
                    IDHuespedReserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDReserva = table.Column<int>(type: "int", nullable: false),
                    IDHuesped = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Huesped___80451727B8D842A2", x => x.IDHuespedReserva);
                    table.ForeignKey(
                        name: "FK__Huesped_R__IDHue__6EF57B66",
                        column: x => x.IDHuesped,
                        principalTable: "Huesped",
                        principalColumn: "IDHuesped");
                    table.ForeignKey(
                        name: "FK__Huesped_R__IDRes__6E01572D",
                        column: x => x.IDReserva,
                        principalTable: "Reservas",
                        principalColumn: "IDReserva");
                });

            migrationBuilder.CreateTable(
                name: "Reserva_Servicio",
                columns: table => new
                {
                    IDReserva = table.Column<int>(type: "int", nullable: false),
                    IDServicio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reserva___AA3E1D265ACEC5D8", x => new { x.IDReserva, x.IDServicio });
                    table.ForeignKey(
                        name: "FK__Reserva_S__IDRes__6A30C649",
                        column: x => x.IDReserva,
                        principalTable: "Reservas",
                        principalColumn: "IDReserva");
                    table.ForeignKey(
                        name: "FK__Reserva_S__IDSer__6B24EA82",
                        column: x => x.IDServicio,
                        principalTable: "Servicio",
                        principalColumn: "IDServicio");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Habitacion_Reservas_IDHabitacion",
                table: "Habitacion_Reservas",
                column: "IDHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_Habitacion_Reservas_IDReserva",
                table: "Habitacion_Reservas",
                column: "IDReserva");

            migrationBuilder.CreateIndex(
                name: "UQ__Huesped__C487C111AA632CD4",
                table: "Huesped",
                column: "CCHuesped",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Huesped_Reservas_IDHuesped",
                table: "Huesped_Reservas",
                column: "IDHuesped");

            migrationBuilder.CreateIndex(
                name: "IX_Huesped_Reservas_IDReserva",
                table: "Huesped_Reservas",
                column: "IDReserva");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_IDUsuario",
                table: "Pago",
                column: "IDUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Paquete_Habitacion_IDHabitacion",
                table: "Paquete_Habitacion",
                column: "IDHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_Paquete_Habitacion_IDPaquete",
                table: "Paquete_Habitacion",
                column: "IDPaquete");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_Servicio_IDServicio",
                table: "Reserva_Servicio",
                column: "IDServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IDHabitacion",
                table: "Reservas",
                column: "IDHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IDPago",
                table: "Reservas",
                column: "IDPago");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IDPaquete",
                table: "Reservas",
                column: "IDPaquete");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IDUsuario",
                table: "Reservas",
                column: "IDUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Rol_Permisos_IDPermisos",
                table: "Rol_Permisos",
                column: "IDPermisos");

            migrationBuilder.CreateIndex(
                name: "IX_Rol_Permisos_IDRol",
                table: "Rol_Permisos",
                column: "IDRol");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IDRol",
                table: "Usuarios",
                column: "IDRol");

            migrationBuilder.CreateIndex(
                name: "UQ__Usuarios__970A592B05EE342F",
                table: "Usuarios",
                column: "CCUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Habitacion_Reservas");

            migrationBuilder.DropTable(
                name: "Huesped_Reservas");

            migrationBuilder.DropTable(
                name: "Paquete_Habitacion");

            migrationBuilder.DropTable(
                name: "Reserva_Servicio");

            migrationBuilder.DropTable(
                name: "Rol_Permisos");

            migrationBuilder.DropTable(
                name: "Huesped");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Servicio");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Habitacion");

            migrationBuilder.DropTable(
                name: "Pago");

            migrationBuilder.DropTable(
                name: "Paquete");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
