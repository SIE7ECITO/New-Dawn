using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class Usuario
{
    public int Idusuario { get; set; }

    public int Ccusuario { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string ContraseñaUsuario { get; set; } = null!;

    public bool EstadoUsuario { get; set; }

    public int Idrol { get; set; }

    public virtual Rol? IdrolNavigation  { get; set; } 

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
