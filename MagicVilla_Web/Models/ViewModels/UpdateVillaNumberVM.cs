using MagicVilla_Web.Models.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.ViewModels
{
	public class UpdateVillaNumberVM
	{
		public UpdateVillaNumberVM()
		{
			UpdateVillaNumberDTO = new UpdateVillaNumberDTO();
		}
		public UpdateVillaNumberDTO UpdateVillaNumberDTO { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> VillasList { get; set; }
	}
}
