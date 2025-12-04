using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_Pharmacy.Model;

public partial class Item
{
    public int ItemId { get; set; }

    public string? ItemTitle { get; set; }

    public int? ItemBrandId { get; set; }

    public string? ItemDesc { get; set; }

    public string? ItemImg { get; set; }

    public int? ItemCount { get; set; }

    public int? ItemPrice { get; set; }

    public string? ItemStatus { get; set; }

    [Column("item_status_on")]
    public string? ItemStatusOn { get; set; }

    [JsonIgnore]
    public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    
    public virtual Brand? ItemBrand { get; set; }

    public string ItemStatusText => ItemStatus == "1" ? "без рецепта" : "по рецепту";
}
