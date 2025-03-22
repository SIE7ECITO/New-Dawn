using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class Comodidade
{
    public int IdComodidades { get; set; }

    public string DescripcionComodidad { get; set; } = null!;

    public bool EstadoComodidad { get; set; }

    public string NombreComodidades { get; set; } = null!;

    public virtual ICollection<HabitacionComodidade> HabitacionComodidades { get; set; } = new List<HabitacionComodidade>();
}
