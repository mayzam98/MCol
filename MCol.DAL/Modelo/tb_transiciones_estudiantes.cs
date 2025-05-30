﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MCol.DAL.Modelo;

[Table("tb_transiciones_estudiantes", Schema = "mcol")]
public partial class tb_transiciones_estudiantes
{
    [Key]
    public int id_transicion { get; set; }

    public int? fk_id_estudiante { get; set; }

    public int? fk_id_grado_actual { get; set; }

    public int? fk_id_grado_siguiente { get; set; }

    public int? fk_id_ano_escolar { get; set; }

    [Column(TypeName = "date")]
    public DateTime? fecha_transicion { get; set; }

    [ForeignKey("fk_id_ano_escolar")]
    [InverseProperty("tb_transiciones_estudiantes")]
    public virtual tb_anos_escolares fk_id_ano_escolarNavigation { get; set; }

    [ForeignKey("fk_id_estudiante")]
    [InverseProperty("tb_transiciones_estudiantes")]
    public virtual tb_estudiantes fk_id_estudianteNavigation { get; set; }

    [ForeignKey("fk_id_grado_actual")]
    [InverseProperty("tb_transiciones_estudiantesfk_id_grado_actualNavigation")]
    public virtual tb_grados fk_id_grado_actualNavigation { get; set; }

    [ForeignKey("fk_id_grado_siguiente")]
    [InverseProperty("tb_transiciones_estudiantesfk_id_grado_siguienteNavigation")]
    public virtual tb_grados fk_id_grado_siguienteNavigation { get; set; }
}