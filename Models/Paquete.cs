using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class Paquete
{
    public int Idpaquete { get; set; }

    public string NombrePaquete { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal Precio { get; set; }

    public bool EstadoPaquete { get; set; }

    public virtual ICollection<PaqueteHabitacion> PaqueteHabitacions { get; set; } = new List<PaqueteHabitacion>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<ServicioPaquete> ServicioPaquetes { get; set; } = new List<ServicioPaquete>();
}
