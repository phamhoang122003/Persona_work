using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persona_work_management.DTO;
using Persona_work_management.Service.Interfaces;

namespace Persona_work_management.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUsersService _usersService;

		public UsersController(IUsersService usersService)
		{
			_usersService = usersService;
		}
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUser()
		{
			var users = await _usersService.GetUser();
			return Ok(users);
		}
		[HttpGet("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<UserDTO>> GetUser(int id) 
		{
			var user = await _usersService.GetUserById(id);
			return Ok(user);
		}
		[HttpPost]
		public async Task<ActionResult<UserDTO>> CreateUser(UserDTO userDTO) 
		{
			var user = await _usersService.CreateUser(userDTO);
			return CreatedAtAction(nameof(GetUser),new {id = user.Id}, user);
		}
		[HttpPut("{id}")]
		public async Task<ActionResult<UserDTO>> UpdateUser(UserDTO userDTO,int id)
		{
			if (userDTO == null|| userDTO.Id != id )
			{
				return BadRequest();
			}
			await _usersService.UpdateUser(userDTO,id);
			return Ok();
		}
	}
}
