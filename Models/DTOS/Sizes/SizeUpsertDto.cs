using System.ComponentModel.DataAnnotations;
using Market.Utils;
using Market.Utils.Constants;

namespace Market.Models.DTOS.Sizes;

public class SizeUpsertDto
{
    private string _productSize;

    [StringLength(Constants.MaxLength, ErrorMessage = "{0} cannot be longer than {1} characters.")]
    public string ProductSize
    {
        get => _productSize;
        set => _productSize = Helper.ToUpper(value);
    }
}