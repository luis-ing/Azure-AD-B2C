using System.ComponentModel.DataAnnotations;

namespace App_Azure_OpenId.Models
{
	public class CreateUserViewModel
	{
		[Required(ErrorMessage = "El nombre es requerido")]
		[Display(Name = "Nombre Completo")]
		public string DisplayName { get; set; }

		[Required(ErrorMessage = "El correo electrónico es requerido")]
		[EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
		[Display(Name = "Correo Electrónico")]
		public string Email { get; set; }

		[Required(ErrorMessage = "La contraseña es requerida")]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
			ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un símbolo")]
		[Display(Name = "Contraseña")]
		public string Password { get; set; }

		[Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
		[Display(Name = "Confirmar Contraseña")]
		public string ConfirmPassword { get; set; }
	}
}
