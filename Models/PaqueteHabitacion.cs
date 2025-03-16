using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class PaqueteHabitacion
{
    public int IdpaqueteHabitacion { get; set; }

    public int Idpaquete { get; set; }

    public int Idhabitacion { get; set; }

    public virtual Habitacion IdhabitacionNavigation { get; set; } = null!;

    public virtual Paquete IdpaqueteNavigation { get; set; } = null!;
}
