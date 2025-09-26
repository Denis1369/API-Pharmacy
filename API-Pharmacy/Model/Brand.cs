using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_Pharmacy.Model;

public partial class Brand
{
    public int BrandId { get; set; }

    public string? BrandName { get; set; }

    [JsonIgnore]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
    