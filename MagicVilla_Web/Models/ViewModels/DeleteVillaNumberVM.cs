using Microsoft.AspNetCore.Mvc.Rendering;
using MagicVilla_Web.Models.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MagicVilla_Web.Models.ViewModels
{
    public class DeleteVillaNumberVM
    {
        public DeleteVillaNumberVM() 
        {
            VillaNumberDTO = new VillaNumberDTO();
        }

        public VillaNumberDTO VillaNumberDTO { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> VillasList { get; set; }
    }
}
