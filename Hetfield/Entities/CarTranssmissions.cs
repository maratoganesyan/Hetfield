﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Hetfield.Entities;

public partial class CarTranssmissions
{
    public int IdTranssmission { get; set; }

    public string TranssmissionName { get; set; }

    public virtual ICollection<Cars> Cars { get; set; } = new List<Cars>();

    public override string ToString()
    {
        return TranssmissionName;
    }
}