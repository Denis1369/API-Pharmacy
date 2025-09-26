using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_Pharmacy.Model;

public partial class Basket
{
    public int BasketId { get; set; }

    public int? BasketClientId { get; set; }

    public DateTime? BasketDate { get; set; }

    public string? BasketStatus { get; set; }

    [JsonIgnore]
    public virtual Client? BasketClient { get; set; }

    public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
}
