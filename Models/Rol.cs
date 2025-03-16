using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class Rol
{
    public int Idrol { get; set; }

    public string NombreRol { get; set; } = null!;

    public bool EstadoRol { get; set; }

    public virtual ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
