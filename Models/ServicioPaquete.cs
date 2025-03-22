using System;
using System.Collections.Generic;

namespace NewDawn.Models;

public partial class ServicioPaquete
{
    public int IdservicioPaquete { get; set; }

    public int Idservicio { get; set; }

    public int Idpaquete { get; set; }

    public virtual Paquete IdpaqueteNavigation { get; set; } = null!;

    public virtual Servicio IdservicioNavigation { get; set; } = null!;
}
