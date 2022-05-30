using AutoMapper;
using Hippo.Application.Common.Attributes;
using System.ComponentModel;

namespace Hippo.Application.Common.Extensions;

public static class IgnoreNoMapExtensions
{
    public static IMappingExpression IgnoreNoMap(
        this IMappingExpression expression, Type sourceType)
    {
        foreach (var property in sourceType.GetProperties())
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(sourceType)[property.Name];
            NoMapAttribute attribute = (NoMapAttribute)descriptor.Attributes[typeof(NoMapAttribute)];
            if (attribute != null)
            {
                expression.ForMember(property.Name, opt => opt.Ignore());
            }
        }
        return expression;
    }
}
