using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class Permiso
{
    public int Idpermisos { get; set; }

    public string NombrePermiso { get; set; } = null!;

    public string DescripcionPermiso { get; set; } = null!;

    public bool EstadoPermisos { get; set; }

    public DateOnly FechaCambio { get; set; }

    public virtual ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
}
