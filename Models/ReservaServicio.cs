using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewDawn.Models;
[Table("Reserva_Servicio", Schema = "dbc")]
public partial class ReservaServicio
{

    [Key]
    [Column("IDReserva", Order = 1)]
    public int Idreserva { get; set; }

    [Key]
    [Column("IDServicio", Order = 2)]
    public int Idservicio { get; set; }

    public virtual Reserva IdreservaNavigation { get; set; } = null!;
    public virtual Servicio IdservicioNavigation { get; set; } = null!;
}
