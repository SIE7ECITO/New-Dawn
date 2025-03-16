using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class Pago
{
    public int Idpago { get; set; }

    public int Idusuario { get; set; }

    public decimal CantidadPago { get; set; }

    public decimal CantidadAbono { get; set; }

    public DateOnly FechaPago { get; set; }

    public DateOnly? FechaAbono { get; set; }

    public bool EstadoPago { get; set; }

    public virtual Usuario IdusuarioNavigation { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
