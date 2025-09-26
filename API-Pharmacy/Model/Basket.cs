using System;
using System.Collections.Generic;

namespace API_Pharmacy.Model;

public partial class Basket
{
    public int BasketId { get; set; }

    public int? BasketClientId { get; set; }

    public DateTime? BasketDate { get; set; }

    public string? BasketStatus { get; set; }

    public virtual Client? BasketClient { get; set; }

    public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
}
