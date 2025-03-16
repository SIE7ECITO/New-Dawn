using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class Huesped
{
    public int Idhuesped { get; set; }

    public int Cchuesped { get; set; }

    public string NombreHuesped { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public virtual ICollection<HuespedReserva> HuespedReservas { get; set; } = new List<HuespedReserva>();
}
