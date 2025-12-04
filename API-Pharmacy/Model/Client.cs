using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace API_Pharmacy.Model;

public partial class Client
{
    public int ClientId { get; set; }

    public string? ClientEmail { get; set; }

    public string? ClientPassword { get; set; }

    public string? ClientLastName { get; set; }

    public string? ClientName { get; set; }

    [Column("client_status")]
    public string? ClientStatus { get; set; }

    [JsonIgnore]
    public virtual ICollection<Basket> Baskets { get; set; } = new List<Basket>();

    public static (bool isValid, string message) ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return (false, "Пароль не может быть пустым");

        if (password.Length < 8)
            return (false, "Пароль должен содержать минимум 8 символов");

        if (!Regex.IsMatch(password, @"[a-z]"))
            return (false, "Пароль должен содержать хотя бы одну строчную букву");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            return (false, "Пароль должен содержать хотя бы одну заглавную букву");

        if (!Regex.IsMatch(password, @"\d"))
            return (false, "Пароль должен содержать хотя бы одну цифру");

        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
            return (false, "Пароль должен содержать хотя бы один специальный символ");

        return (true, "Пароль соответствует требованиям");
    }
}
