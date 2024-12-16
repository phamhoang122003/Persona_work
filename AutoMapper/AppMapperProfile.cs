using AutoMapper;
using Persona_work_management.DTO;
using Persona_work_management.Entities;

namespace Persona_work_management.AutoMapper
{
	public class AppMapperProfile : Profile
	{
		public AppMapperProfile() { 
			CreateMap<Notification, NotificationDTO>().ReverseMap();

			// Ánh xạ từ TaskDTO sang Tasks
			CreateMap<TaskDTO, Tasks>()
				.ForMember(dest => dest.Priority, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Priority) ? Priority.Medium : Enum.Parse<Priority>(src.Priority)))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Status) ? Status.Pending : Enum.Parse<Status>(src.Status)))
				.ForMember(dest => dest.Color, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Color) ? LabelColor.Green : Enum.Parse<LabelColor>(src.Color)))
					.ForMember(dest => dest.User, opt => opt.Ignore())
				.ForMember(dest => dest.Notifications, opt => opt.Ignore()); // Bỏ qua Notifications để tránh vòng lặp;  // Không ánh xạ User, tránh vòng lặp

			// Ánh xạ ngược từ Tasks sang TaskDTO
			CreateMap<Tasks, TaskDTO>()
				.ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))   // Chuyển từ enum sang chuỗi
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))      // Chuyển từ enum sang chuỗi
				.ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color.ToString()))       // Chuyển từ enum sang chuỗi
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)) // Ánh xạ UserId từ User entity
				.ForMember(dest => dest.Notifications, opt => opt.Ignore()); // Bỏ qua Notifications để tránh vòng lặp



			// Mapping cho User
			CreateMap<Users, UserDTO>()
				.ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Avatar != null ? $"/uploads/avatars/{src.Avatar}" : null))
				.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

			CreateMap<UserCreateDTO, Users>()
				.ForMember(dest => dest.Avatar, opt => opt.Ignore()); // Avatar xử lý riêng

			CreateMap<UserUpdateDTO, Users>()
				.ForMember(dest => dest.Avatar, opt => opt.Ignore()); // Avatar xử lý riêng

		}

	}
}
