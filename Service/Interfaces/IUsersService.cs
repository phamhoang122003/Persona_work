using Microsoft.AspNetCore.Mvc;
using Persona_work_management.DTO;

namespace Persona_work_management.Service.Interfaces
{
	public interface IUsersService
	{
		Task<IEnumerable<UserDTO>> GetUser();
		Task<UserDTO> GetUserById(int id);

		Task UpdateUser(UserDTO userDTO, int id);
		Task DeleteUser(int id);

		Task<UserDTO> CreateUser(UserDTO userDTO);
		public string HashPassword(string password);
		public bool VerifyPassword(string password, string storedHashedPassword);
	}
}
