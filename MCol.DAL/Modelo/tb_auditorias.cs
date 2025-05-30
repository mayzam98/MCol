﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MCol.DAL.Modelo;

[Table("tb_auditorias", Schema = "mcol")]
public partial class tb_auditorias
{
    [Key]
    public int id_auditoria { get; set; }

    [Column(TypeName = "text")]
    public string descripcion { get; set; }

    [Column(TypeName = "date")]
    public DateTime? fecha { get; set; }

    [InverseProperty("fk_id_auditoriaNavigation")]
    public virtual ICollection<tb_informes_auditoria> tb_informes_auditoria { get; set; } = new List<tb_informes_auditoria>();
}