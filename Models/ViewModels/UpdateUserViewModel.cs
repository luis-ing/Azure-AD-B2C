using System.ComponentModel.DataAnnotations;

namespace App_Azure_OpenId.Models
{
    public class UpdateUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre Completo")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Ingrese su nombre")]
        [Display(Name = "Correo Electrónico")]
        public string GivenName { get; set; }

		[Required(ErrorMessage = "Ingrese su título de trabajo")]
		[Display(Name = "Título de trabajo")]
		public string JobTitle { get; set; }

	}
}
