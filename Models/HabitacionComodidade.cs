using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class HabitacionComodidade
{
    public int IdHabitacionComodidades { get; set; }

    public int IdHabitacion { get; set; }

    public int IdComodidades { get; set; }

    public virtual Comodidade IdComodidadesNavigation { get; set; } = null!;

    public virtual Habitacion IdHabitacionNavigation { get; set; } = null!;
}
