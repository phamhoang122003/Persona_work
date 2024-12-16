using AutoMapper;
using Persona_work_management.DTO;
using Persona_work_management.Entities;
using Persona_work_management.Repository.Interfaces;
using Persona_work_management.Service.Interfaces;

public class UsersService : IUsersService
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;

	public UsersService(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	// Phương thức tạo người dùng
	public async Task<UserDTO> CreateUser(UserCreateDTO userCreateDto)
	{
		var user = _mapper.Map<Users>(userCreateDto);

		// Hash mật khẩu
		var hashedPassword = HashPassword(userCreateDto.Password);
		user.PasswordHash = hashedPassword;

		// Xử lý lưu file ảnh (nếu có)
		if (userCreateDto.Avatar != null)
		{
			var avatarFileName = await SaveAvatarAsync(userCreateDto.Avatar);
			user.Avatar = avatarFileName;
		}

		// Thêm vào repository
		await _unitOfWork.UsersRepository.AddAsync(user);
		await _unitOfWork.CompleteAsync();

		return _mapper.Map<UserDTO>(user);
	}

	// Phương thức xóa người dùng
	public async Task DeleteUser(int id)
	{
		var user = await _unitOfWork.UsersRepository.GetByIdAsync(id);
		if (user != null)
		{
			// Xóa file avatar (nếu có)
			if (!string.IsNullOrEmpty(user.Avatar))
			{
				DeleteAvatar(user.Avatar);
			}

			await _unitOfWork.UsersRepository.DeleteAsync(user);
			await _unitOfWork.CompleteAsync();
		}
	}

	// Lấy danh sách người dùng
	public async Task<IEnumerable<UserDTO>> GetUser()
	{
		var users = await _unitOfWork.UsersRepository.GetAllAsync();
		return _mapper.Map<IEnumerable<UserDTO>>(users);
	}

	// Lấy thông tin chi tiết của người dùng
	public async Task<UserDTO> GetUserById(int id)
	{
		var user = await _unitOfWork.UsersRepository.GetByIdAsync(id);
		return _mapper.Map<UserDTO>(user);
	}

	// Cập nhật thông tin người dùng
	public async Task UpdateUser(UserUpdateDTO userUpdateDto, int id)
	{
		var user = await _unitOfWork.UsersRepository.GetByIdAsync(id);

		if (user == null || user.Id != id)
			throw new Exception("User not found");

		// Cập nhật các trường thông tin
		user.UserName = userUpdateDto.UserName ?? user.UserName;
		user.Email = userUpdateDto.Email ?? user.Email;

		// Cập nhật mật khẩu (nếu có)
		if (!string.IsNullOrEmpty(userUpdateDto.Password))
		{
			user.PasswordHash = HashPassword(userUpdateDto.Password);
		}

		// Xử lý ảnh đại diện
		if (userUpdateDto.Avatar != null)
		{
			// Xóa avatar cũ (nếu có)
			if (!string.IsNullOrEmpty(user.Avatar))
			{
				DeleteAvatar(user.Avatar);
			}

			// Lưu avatar mới
			var avatarFileName = await SaveAvatarAsync(userUpdateDto.Avatar);
			user.Avatar = avatarFileName;
		}

		await _unitOfWork.CompleteAsync();
	}

	// Hash mật khẩu
	public string HashPassword(string password)
	{
		return BCrypt.Net.BCrypt.HashPassword(password);
	}

	// Kiểm tra mật khẩu
	public bool VerifyPassword(string password, string storedHashedPassword)
	{
		return BCrypt.Net.BCrypt.Verify(password, storedHashedPassword);
	}

	// Lưu file ảnh
	private async Task<string> SaveAvatarAsync(IFormFile avatarFile)
	{
		var uploadDirectory = Path.Combine("wwwroot", "uploads", "avatars");

		// Kiểm tra và tạo thư mục nếu chưa có
		if (!Directory.Exists(uploadDirectory))
		{
			Directory.CreateDirectory(uploadDirectory);
		}

		// Tạo tên file duy nhất
		var avatarFileName = $"{Guid.NewGuid()}_{avatarFile.FileName}";
		var filePath = Path.Combine(uploadDirectory, avatarFileName);

		try
		{
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await avatarFile.CopyToAsync(stream);
			}

			return avatarFileName;
		}
		catch (Exception ex)
		{
			// Log lỗi nếu có vấn đề khi lưu ảnh
			throw new Exception("Failed to save avatar", ex);
		}
	}

	// Xóa file ảnh
	private void DeleteAvatar(string avatarFileName)
	{
		var filePath = Path.Combine("wwwroot", "uploads", "avatars", avatarFileName);
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
	}
}
