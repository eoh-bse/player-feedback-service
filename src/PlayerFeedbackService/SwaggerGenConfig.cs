using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PlayerFeedbackService
{
    public class AddRequiredUbiUserIdHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.HttpMethod.Equals("POST"))
            {
                return;
            }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = UbiUserIdHeader.KeyName,
                In = ParameterLocation.Header,
                Required = true,
            });
        }
    }
}
