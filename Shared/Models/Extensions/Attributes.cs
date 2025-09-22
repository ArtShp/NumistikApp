using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Extensions;

public class UsernameAttribute : RegularExpressionAttribute
{
    public UsernameAttribute()
        : base(@"^\w{3,30}$")
    {
        ErrorMessage = "Username must be 3-30 characters and contain only letters, numbers, or underscores.";
    }
}

public class PasswordAttribute : RegularExpressionAttribute
{
    public PasswordAttribute()
        : base(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$")
    {
        ErrorMessage = "Password must be minimum 8 characters, at least 1 letter and 1 number.";
    }
}

public class NameWithSpacesNoNumbersAttribute : RegularExpressionAttribute
{
    public NameWithSpacesNoNumbersAttribute()
        : base(@"^([a-zA-Z]+ )*[a-zA-Z]+$")
    {
        ErrorMessage = "Name must contain only letters and spaces.";
    }
}

public class CurrencyCodeAttribute : RegularExpressionAttribute
{
    public CurrencyCodeAttribute()
        : base(@"^[A-Z]+$")
    {
        ErrorMessage = "Currency code must contain only capital letters.";
    }
}

public class CurrencySymbolAttribute : RegularExpressionAttribute
{
    public CurrencySymbolAttribute()
        : base(@"^.+$")
    {
        ErrorMessage = "Currency code must contain only capital letters.";
    }
}

public class CountryCodeAttribute : RegularExpressionAttribute
{
    public CountryCodeAttribute()
        : base(@"^[A-Z]+$")
    {
        ErrorMessage = "Country code must contain only capital letters.";
    }
}

public class CatalogItemValueAttribute : RegularExpressionAttribute
{
    public CatalogItemValueAttribute()
        : base(@"^\d+(\.\d+)?$")
    {
        ErrorMessage = "Catalog item value must be an integer or a floating point number.";
    }
}
