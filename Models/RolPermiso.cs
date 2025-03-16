using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class RolPermiso
{
    public int IdrolPermisos { get; set; }

    public int Idrol { get; set; }

    public int Idpermisos { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public virtual Permiso IdpermisosNavigation { get; set; } = null!;

    public virtual Rol IdrolNavigation { get; set; } = null!;
}
