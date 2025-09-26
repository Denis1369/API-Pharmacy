using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API_Pharmacy.Model;

public partial class BasketItem
{
    public int BasketItemId { get; set; }

    public int? BasketId { get; set; }

    public int? ItemId { get; set; }

    public int? BasketItemCount { get; set; }

    [JsonIgnore]
    public virtual Basket? Basket { get; set; }

    public virtual Item? Item { get; set; }
}
