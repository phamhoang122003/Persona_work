using Persona_work_management.Entities;

namespace Persona_work_management.Service.Interfaces
{
	public interface IAuthenticationService
	{
		Task<Users?> AuthenticateAsync(string username, string password);
		string GenerateJwtToken(Users user);
	}
}
