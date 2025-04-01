using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class Servicio
{
    public int Idservicio { get; set; }

    public string NombreServicio { get; set; } = null!;

    public string DescripcionServicio { get; set; } = null!;

    public decimal ValorServicio { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public bool EstadoServicio { get; set; }

    public virtual ICollection<ServicioPaquete> ServicioPaquetes { get; set; } = new List<ServicioPaquete>();

    public virtual ICollection<Reserva> Idreservas { get; set; } = new List<Reserva>();
    public virtual ICollection<ReservaServicio> ReservaServicios { get; set; } = new List<ReservaServicio>();

}
