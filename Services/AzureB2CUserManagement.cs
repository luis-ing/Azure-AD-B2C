using Microsoft.Graph.Models;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Azure.Identity;

namespace App_Azure_OpenId.Services
{
	public class AzureB2CUserManagement
	{
		private readonly GraphServiceClient _graphClient;
		private readonly string _b2cDomain;

		public AzureB2CUserManagement(string clientId, string clientSecret, string tenantId, string b2cDomain)
		{
			_b2cDomain = b2cDomain;

			var credential = new ClientSecretCredential(
				tenantId,
				clientId,
				clientSecret
			);

			_graphClient = new GraphServiceClient(credential);
		}

		public async Task<User> CreateUserAsync(string displayName, string email, string password)
		{
			var user = new User
			{
				AccountEnabled = true,
				DisplayName = displayName,
				Identities = new List<ObjectIdentity>
				{
					new ObjectIdentity
					{
						SignInType = "emailAddress",
						Issuer = _b2cDomain,
						IssuerAssignedId = email
					}
				},
				PasswordProfile = new PasswordProfile
				{
					Password = password,
					ForceChangePasswordNextSignIn = false
				},
				PasswordPolicies = "DisablePasswordExpiration"
			};

			try
			{
				return await _graphClient.Users.PostAsync(user);
			}
			catch (Microsoft.Graph.Models.ODataErrors.ODataError ex)
			{
				throw new Exception($"Error creating user: {ex.Message}", ex);
			}
		}

		public async Task<User> GetUserByEmailAsync(string email)
		{
			try
			{
				var users = await _graphClient.Users.GetAsync(requestConfiguration =>
				{
					requestConfiguration.QueryParameters.Filter =
						$"identities/any(id:id/issuerAssignedId eq '{email}' and id/issuer eq '{_b2cDomain}')";
				});

				return users?.Value?.FirstOrDefault();
			}
			catch (Microsoft.Graph.Models.ODataErrors.ODataError ex)
			{
				throw new Exception($"Error getting user: {ex.Message}", ex);
			}
		}

		public async Task DeleteUserAsync(string userId)
		{
			try
			{
				await _graphClient.Users[userId].DeleteAsync();
			}
			catch (Microsoft.Graph.Models.ODataErrors.ODataError ex)
			{
				throw new Exception($"Error deleting user: {ex.Message}", ex);
			}
		}

		public async Task<IList<User>> GetAllUsersAsync()
		{
			try
			{
				var users = await _graphClient.Users.GetAsync(requestConfiguration =>
				{
					requestConfiguration.QueryParameters.Select = new string[]
					{
						"id",
						"displayName",
						"identities",
						"accountEnabled"
					};
				});

				return users?.Value ?? new List<User>();
			}
			catch (Microsoft.Graph.Models.ODataErrors.ODataError ex)
			{
				throw new Exception($"Error getting users: {ex.Message}", ex);
			}
		}

		public async Task<bool> UpdateUserAsync(string userId, string displayName)
		{
			try
			{
				var user = new User
				{
					DisplayName = displayName
				};

				await _graphClient.Users[userId].PatchAsync(user);
				return true;
			}
			catch (Microsoft.Graph.Models.ODataErrors.ODataError ex)
			{
				throw new Exception($"Error updating user: {ex.Message}", ex);
			}
		}

        public async Task<User> GetUserByIdAsync(string userId)
        {
            try
            {
                // Usa el servicio Graph para obtener el usuario por ID
                var user = await _graphClient.Users[userId].GetAsync();

                return user;
            }
            catch (ServiceException ex)
            {
                // Maneja errores de Graph API, por ejemplo, cuando el usuario no existe
                Console.WriteLine($"Error al obtener usuario: {ex.Message}");
                return null;
            }
        }
    }
}
