﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MCol.DAL.Modelo;

[Table("tb_historial_academico", Schema = "mcol")]
public partial class tb_historial_academico
{
    [Key]
    public int id_historial { get; set; }

    public int? fk_id_estudiante { get; set; }

    public int? fk_id_grado { get; set; }

    public int? fk_id_ano_escolar { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? promedio { get; set; }

    [Column(TypeName = "text")]
    public string observaciones { get; set; }

    [ForeignKey("fk_id_ano_escolar")]
    [InverseProperty("tb_historial_academico")]
    public virtual tb_anos_escolares fk_id_ano_escolarNavigation { get; set; }

    [ForeignKey("fk_id_estudiante")]
    [InverseProperty("tb_historial_academico")]
    public virtual tb_estudiantes fk_id_estudianteNavigation { get; set; }

    [ForeignKey("fk_id_grado")]
    [InverseProperty("tb_historial_academico")]
    public virtual tb_grados fk_id_gradoNavigation { get; set; }
}