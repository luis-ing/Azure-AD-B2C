using App_Azure_OpenId.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App_Azure_OpenId.Models;

namespace App_Azure_OpenId.Controllers
{
	public class AccountController : Controller
	{

		private readonly AzureB2CUserManagement _userManagement;
        private readonly GraphUserService _graphUserService;

        public AccountController(AzureB2CUserManagement userManagement, GraphUserService graphUserService)
		{
			_userManagement = userManagement;
            _graphUserService = graphUserService;
        }

		public async Task<IActionResult> Index()
		{
			try
			{
				var users = await _userManagement.GetAllUsersAsync();
				return View(users);
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Error al cargar usuarios: {ex.Message}";
				return View(new List<Microsoft.Graph.Models.User>());
			}
		}

		public IActionResult Create()
		{
			return View(new CreateUserViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateUserViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _userManagement.CreateUserAsync(
						model.DisplayName,
						model.Email,
						model.Password
					);

					TempData["Success"] = "Usuario creado exitosamente";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"Error al crear usuario: {ex.Message}");
				}
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string userId)
		{
			try
			{
				await _userManagement.DeleteUserAsync(userId);
				TempData["Success"] = "Usuario eliminado exitosamente";
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Error al eliminar usuario: {ex.Message}";
			}

			return RedirectToAction(nameof(Index));
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUserApi(CreateUserViewModel model)
		{
            try
            {
                await _graphUserService.CreateUserAsync(model.DisplayName, model.Email, model.Password);
                TempData["Success"] = "Usuario creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> Update(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("El ID del usuario no es válido.");
            }

            var user = await _userManagement.GetUserByIdAsync(id);

            if (user == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateUserViewModel
            {
				Id = user.Id,
				DisplayName = user.DisplayName,
				JobTitle = user.JobTitle
                ///*Email*/ = user.Identities?.FirstOrDefault()?.IssuerAssignedId,
                // Otros campos necesarios para la actualización
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserApi(UpdateUserViewModel model)
        {
            try
            {
                await _graphUserService.UpdateUserAsync(model);
                TempData["Success"] = "Usuario creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
