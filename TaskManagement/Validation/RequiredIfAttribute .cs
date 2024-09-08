using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public class RequiredIfAttribute : ValidationAttribute
{
    private readonly string _dependentProperty;
    private readonly object _targetValue;

    public RequiredIfAttribute(string dependentProperty, object targetValue)
    {
        _dependentProperty = dependentProperty;
        _targetValue = targetValue;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Get the dependent property (e.g., RoleName)
        var property = validationContext.ObjectType.GetProperty(_dependentProperty);
        if (property == null)
        {
            return new ValidationResult($"Unknown property: {_dependentProperty}");
        }

        var dependentValue = property.GetValue(validationContext.ObjectInstance);

        if (dependentValue != null && dependentValue.Equals(_targetValue))
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required.");
            }
        }

        return ValidationResult.Success;
    }
}
