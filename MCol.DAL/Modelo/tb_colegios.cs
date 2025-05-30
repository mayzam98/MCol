﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MCol.DAL.Modelo;

[Table("tb_colegios", Schema = "mcol")]
public partial class tb_colegios
{
    [Key]
    public int id_colegio { get; set; }

    [StringLength(100)]
    public string nombre { get; set; }

    [StringLength(255)]
    public string direccion { get; set; }

    [InverseProperty("fk_id_colegioNavigation")]
    public virtual ICollection<tb_colegios_perfiles> tb_colegios_perfiles { get; set; } = new List<tb_colegios_perfiles>();

    [InverseProperty("fk_id_colegioNavigation")]
    public virtual ICollection<tb_comunicaciones_institucionales> tb_comunicaciones_institucionales { get; set; } = new List<tb_comunicaciones_institucionales>();

    [InverseProperty("fk_id_colegioNavigation")]
    public virtual ICollection<tb_inscripciones> tb_inscripciones { get; set; } = new List<tb_inscripciones>();
}