﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MCol.DAL.Modelo;

[Table("tb_planes_accion", Schema = "mcol")]
public partial class tb_planes_accion
{
    [Key]
    public int id_plan_accion { get; set; }

    public int? fk_id_incidencia { get; set; }

    [Column(TypeName = "text")]
    public string descripcion { get; set; }

    [Column(TypeName = "date")]
    public DateTime? fecha { get; set; }

    [ForeignKey("fk_id_incidencia")]
    [InverseProperty("tb_planes_accion")]
    public virtual tb_incidencias fk_id_incidenciaNavigation { get; set; }
}