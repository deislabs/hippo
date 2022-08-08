using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text;

namespace Hippo.Web.Helpers;

public class ApiControllerNameConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var sb = new StringBuilder();
        foreach (var c in controller.ControllerName)
        {
            if (char.IsUpper(c))
                sb.Append("-");
            sb.Append(char.ToLower(c));
        }

        controller.ControllerName = sb.ToString().TrimStart('-');
    }
}