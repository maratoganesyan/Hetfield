﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Hetfield.Entities;

public partial class CarConfigurations
{
    public int IdCarConfiguration { get; set; }

    public string CarConfigurationName { get; set; }

    public virtual ICollection<Cars> Cars { get; set; } = new List<Cars>();

    public override string ToString()
    {
        return CarConfigurationName;
    }
}