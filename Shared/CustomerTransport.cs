﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace BlazorAppForms.Shared;

public sealed class CustomerTransport : TransportBase
{
    public CustomerTransport()
    {
        CustomerLocation = new List<CustomerLocationTransport>();
    }

    public int IdCustomer { get; set; }
    public string Name { get; set; }
    public DateTime Birth { get; set; }

    public List<CustomerLocationTransport> CustomerLocation { get; set; }
}