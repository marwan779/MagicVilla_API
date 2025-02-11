using Microsoft.AspNetCore.Mvc.Rendering;
using MagicVilla_Web.Models.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MagicVilla_Web.Models.ViewModels
{
    public class CreateVillaNumberVM
    {
        public CreateVillaNumberVM() 
        {
            CreateVillaNumberDTO = new CreateVillaNumberDTO();
        }

        public CreateVillaNumberDTO CreateVillaNumberDTO { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> VillasList { get; set; }
    }
}
