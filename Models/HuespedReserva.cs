using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class HuespedReserva
{
    public int IdhuespedReserva { get; set; }

    public int Idreserva { get; set; }

    public int Idhuesped { get; set; }

    public virtual Huesped IdhuespedNavigation { get; set; } = null!;

    public virtual Reserva IdreservaNavigation { get; set; } = null!;
}
