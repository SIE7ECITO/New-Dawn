using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class HabitacionReserva
{
    public int IdhabitacionReserva { get; set; }

    public int Idhabitacion { get; set; }

    public int Idreserva { get; set; }

    public virtual Habitacion IdhabitacionNavigation { get; set; } = null!;

    public virtual Reserva IdreservaNavigation { get; set; } = null!;
}
