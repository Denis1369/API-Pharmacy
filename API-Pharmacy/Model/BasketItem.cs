using System;
using System.Collections.Generic;

namespace API_Pharmacy.Model;

public partial class BasketItem
{
    public int BasketItemId { get; set; }

    public int? BasketId { get; set; }

    public int? ItemId { get; set; }

    public int? BasketItemCount { get; set; }

    public virtual Basket? Basket { get; set; }

    public virtual Item? Item { get; set; }
}
