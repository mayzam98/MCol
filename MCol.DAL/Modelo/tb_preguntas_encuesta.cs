﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MCol.DAL.Modelo;

[Table("tb_preguntas_encuesta", Schema = "mcol")]
public partial class tb_preguntas_encuesta
{
    [Key]
    public int id_pregunta { get; set; }

    public int? fk_id_encuesta { get; set; }

    [Column(TypeName = "text")]
    public string pregunta { get; set; }

    [ForeignKey("fk_id_encuesta")]
    [InverseProperty("tb_preguntas_encuesta")]
    public virtual tb_encuestas_satisfaccion fk_id_encuestaNavigation { get; set; }

    [InverseProperty("fk_id_preguntaNavigation")]
    public virtual ICollection<tb_respuestas_encuesta> tb_respuestas_encuesta { get; set; } = new List<tb_respuestas_encuesta>();
}