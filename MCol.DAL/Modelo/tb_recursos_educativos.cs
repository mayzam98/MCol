﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MCol.DAL.Modelo;

[Table("tb_recursos_educativos", Schema = "mcol")]
public partial class tb_recursos_educativos
{
    [Key]
    public int id_recurso { get; set; }

    [StringLength(50)]
    public string nombre { get; set; }

    [Column(TypeName = "text")]
    public string descripcion { get; set; }
}