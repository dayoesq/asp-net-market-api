using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Market.ApiBehaviours;

public class ControllerGlobalPrefix : IApplicationModelConvention
{
    private const string ApiPrefix = "api/v1";
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            // Add the "api/v1" prefix to the route template for all controllers.
            var selectorModel = controller.Selectors.FirstOrDefault();
            if (selectorModel != null)
            {
                selectorModel.AttributeRouteModel = new AttributeRouteModel
                {
                    Template = AttributeRouteModel
                        .CombineTemplates(ApiPrefix, selectorModel.AttributeRouteModel?.Template)
                };
            }
        }
    }
}