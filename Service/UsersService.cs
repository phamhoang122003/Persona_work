using AutoMapper;
using Persona_work_management.DTO;
using Persona_work_management.Entities;
using Persona_work_management.Repository.Interfaces;
using Persona_work_management.Service.Interfaces;
using System.Threading.Tasks;

namespace Persona_work_management.Service
{
	public class UsersService : IUsersService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public UsersService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<UserDTO> CreateUser(UserDTO userDTO)
		{
			var user = _mapper.Map<Users>(userDTO);
			var hashedPassword = HashPassword(user.PasswordHash);
			user.PasswordHash = hashedPassword;
			await _unitOfWork.UsersRepository.AddAsync(user);
			await _unitOfWork.CompleteAsync();
			return _mapper.Map<UserDTO>(user);
		}

		public async Task DeleteUser(int id)
		{
			var user = await _unitOfWork.UsersRepository.GetByIdAsync(id);
			if(user != null)
			{
				await _unitOfWork.UsersRepository.DeleteAsync(user);
				await _unitOfWork.CompleteAsync();
			}
		}

		public async Task<IEnumerable<UserDTO>> GetUser()
		{
			var users = await _unitOfWork.UsersRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<UserDTO>>(users);
		}

		public async Task<UserDTO> GetUserById(int id)
		{
			var user = await _unitOfWork.UsersRepository.GetByIdAsync(id);
			return _mapper.Map<UserDTO>(user);
		}

		public string HashPassword(string password)
		{
			// Mã hóa mật khẩu
			var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
			return hashedPassword;
		}

		public async Task UpdateUser(UserDTO userDTO, int id)
		{
			var user = await _unitOfWork.UsersRepository.GetByIdAsync(id);

			user.UserName = userDTO.UserName ?? user.UserName;
			user.Email = userDTO.Email ?? user.Email;
			user.PasswordHash = userDTO.PasswordHash ?? user.PasswordHash;
			user.Role = !string.IsNullOrEmpty(userDTO.Role) ? Enum.Parse<Role>(userDTO.Role) : user.Role;
			await _unitOfWork.CompleteAsync();
		}

		public bool VerifyPassword(string password, string storedHashedPassword)
		{
			// Kiểm tra mật khẩu nhập vào với mật khẩu đã mã hóa
			return BCrypt.Net.BCrypt.Verify(password, storedHashedPassword);
		}
	}
}
