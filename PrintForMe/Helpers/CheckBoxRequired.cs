using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

public class CheckBoxRequired : ValidationAttribute, IClientValidatable
{
    public override bool IsValid(object value)
    {
        if (value is bool)
        {
            return (bool)value;
        }

        return false;
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        yield return new ModelClientValidationRule
        {
            ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
            ValidationType = "checkboxrequired"
        };
    }
}
