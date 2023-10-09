using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Market.Filters;
[AttributeUsage(AttributeTargets.Method)]
public class ValidateAssetsFilterAttribute : ActionFilterAttribute
{
    private readonly string _allowedExtensions;
    private readonly long _maxFileSize; // Maximum file size in bytes

    public ValidateAssetsFilterAttribute(string allowedExtensions, long maxFileSize = 5 * 1024 * 1024) // Default to 5MB
    {
        _allowedExtensions = allowedExtensions;
        _maxFileSize = maxFileSize;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.HasFormContentType)
        {
            context.Result = new BadRequestObjectResult("Invalid Content-Type. Use 'multipart/form-data' for file uploads.");
            return;
        }

        var files = GetFiles(context);

        foreach (var file in files)
        {
            if (!IsValidFile(file))
            {
                context.ModelState.AddModelError(file.Name, "File validation failed.");
            }
        }

        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
            return;
        }

        base.OnActionExecuting(context);
    }

    private IEnumerable<IFormFile> GetFiles(ActionExecutingContext context)
    {
        // Check if the parameter is a single IFormFile or a List<IFormFile>
        var param = context.ActionArguments.FirstOrDefault();
        if (param.Value is IFormFile singleFile)
        {
            return new List<IFormFile> { singleFile };
        }
        else if (param.Value is List<IFormFile> fileList)
        {
            return fileList;
        }

        return Enumerable.Empty<IFormFile>();
    }

    private bool IsValidFile(IFormFile file)
    {
        var allowedExtensions = _allowedExtensions.Split(',').Select(ext => ext.Trim().ToLowerInvariant());
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            return false;
        }

        if (file.Length > _maxFileSize)
        {
            return false;
        }

        return true;
    }
}

