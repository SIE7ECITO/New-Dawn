using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class Reserva
{
    public int Idreserva { get; set; }

    public int Idusuario { get; set; }

    public int Idhabitacion { get; set; }

    public int? Idpaquete { get; set; }

    public int Idpago { get; set; }

    public DateOnly FechaReserva { get; set; }

    public DateOnly? FechaComienzo { get; set; }

    public DateOnly? FechaFin { get; set; }

    public double ValorTotal { get; set; }

    public bool EstadoReserva { get; set; }

    public virtual ICollection<HabitacionReserva> HabitacionReservas { get; set; } = new List<HabitacionReserva>();

    public virtual ICollection<HuespedReserva>? HuespedReservas { get; set; } = new List<HuespedReserva>();

    public virtual Habitacion? IdhabitacionNavigation { get; set; } = null!;

    public virtual Pago? IdpagoNavigation { get; set; }

    public virtual Paquete? IdpaqueteNavigation { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; } = null!;

    public virtual ICollection<Servicio> Idservicios { get; set; } = new List<Servicio>();
    public virtual ICollection<ReservaServicio> ReservaServicios { get; set; } = new List<ReservaServicio>();

}
