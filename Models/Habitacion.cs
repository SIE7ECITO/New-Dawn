using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class Habitacion
{
    public int Idhabitacion { get; set; }

    public string TipoHabitacion { get; set; } = null!;

    public bool EstadoHabitacion { get; set; }

    public decimal PrecioNoche { get; set; }

    public bool EnPaquete { get; set; }

    public virtual ICollection<HabitacionReserva> HabitacionReservas { get; set; } = new List<HabitacionReserva>();

    public virtual ICollection<PaqueteHabitacion> PaqueteHabitacions { get; set; } = new List<PaqueteHabitacion>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
