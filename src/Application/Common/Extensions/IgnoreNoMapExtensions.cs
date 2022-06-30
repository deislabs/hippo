using AutoMapper;
using Hippo.Application.Common.Attributes;
using System.ComponentModel;

namespace Hippo.Application.Common.Extensions;

public static class MappingExtensions
{
    public static IMappingExpression IgnoreMarkedAttributes(
        this IMappingExpression expression, Type destinationType)
    {
        foreach (var property in destinationType.GetProperties())
        {
            PropertyDescriptor? descriptor = TypeDescriptor.GetProperties(destinationType)?[property.Name];
            if (descriptor is null || descriptor.Attributes is null)
                continue;

            var attribute = descriptor.Attributes[typeof(NoMapAttribute)];
            if (attribute != null && (NoMapAttribute)attribute != null)
            {
                expression.ForMember(property.Name, opt => opt.Ignore());
            }
        }
        return expression;
    }
}
