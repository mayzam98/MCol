﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MCol.DAL.Modelo;

[Table("tb_planes_estudio", Schema = "mcol")]
public partial class tb_planes_estudio
{
    [Key]
    public int id_plan_estudio { get; set; }

    [StringLength(100)]
    public string nombre { get; set; }

    [Column(TypeName = "text")]
    public string descripcion { get; set; }
}