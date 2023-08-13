using Microsoft.AspNetCore.Mvc.Filters;

namespace Market.Filters;

    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateImageAndVideoFileAttribute : ActionFilterAttribute
    {
        private readonly string _validationType;

        public ValidateImageAndVideoFileAttribute(string validationType = "file")
        {
            _validationType = validationType;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var files = context.HttpContext.Request.Form.Files;

            foreach (var file in files)
            {
                if (!IsValidFile(file, _validationType, context))
                {
                    context.ModelState.AddModelError(file.Name, "File validation failed.");
                }
            }

            base.OnActionExecuting(context);
        }

        private bool IsValidFile(IFormFile file, string validationType, ActionExecutingContext context)
        {
            var configuration = (IConfiguration)context.HttpContext.RequestServices.GetService(typeof(IConfiguration))!;
            var maxFileSize = configuration.GetValue<int>("Files:MaxFileSize");
            var minFileSize = configuration.GetValue<int>("Files:MinFileSize");

            var allowedExtensions = GetAllowedExtensions(validationType);
            var validSignatures = GetValidSignatures(validationType);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileSize = file.Length;

            if (fileSize < minFileSize || fileSize > maxFileSize)
                return false;

            if (!allowedExtensions.ContainsKey(extension))
                return false;

            using var stream = file.OpenReadStream();
            using var reader = new BinaryReader(stream);
            var headerBytes = reader.ReadBytes(validSignatures.Max(signatureList => signatureList.Value[0].Length));

            return validSignatures.Any(signatureList =>
                signatureList.Value.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature)));
        }

        private Dictionary<string, List<string>> GetAllowedExtensions(string validationType)
        {
            var allowedExtensions = new Dictionary<string, List<string>>
            {
                { "file", new List<string> { ".jpg", ".jpeg", ".svg", ".png", ".pdf" } },
                { "video", new List<string> { ".mp4", ".mov" } }
            };

            return allowedExtensions.TryGetValue(validationType, out var extensions) ? new Dictionary<string, List<string>> { { validationType, extensions } } : new Dictionary<string, List<string>>();
        }

        private Dictionary<string, List<byte[]>> GetValidSignatures(string validationType)
        {
            var validSignatures = new Dictionary<string, List<byte[]>>
            {
                {
                    "file", new List<byte[]>
                    {
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, // JPEG
                        new byte[] { 0x89, 0x50, 0x4E, 0x47 }, // PNG
                        new byte[] { 0x25, 0x50, 0x44, 0x46 }  // PDF
                    }
                },
                {
                    "video", new List<byte[]>
                    {
                        new byte[] { 0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70 }, // MP4
                        new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70 }  // MOV
                    }
                }
            };

            return validSignatures.TryGetValue(validationType, out var signatures) ? new Dictionary<string, List<byte[]>> { { validationType, signatures } } : new Dictionary<string, List<byte[]>>();
        }
    }

