using System.ComponentModel.DataAnnotations;

namespace App_Azure_OpenId.Models.ViewModels
{
	public class NoteViewModel
	{
		[Required]
		[Display(Name = "Título")]
		public string? Title { get; set; }

		[Required]
		[Display(Name = "Descripción")]
		public string? Content { get; set; }
	}
}
