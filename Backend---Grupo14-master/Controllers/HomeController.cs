using Backend___Grupo14.Entities;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend___Grupo14.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly FirebaseService _firebaseService;

        public HomeController()
        {
            _firebaseService = new FirebaseService();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto user)
        {
            try
            {
                var userId = await _firebaseService.RegisterUser(user.Email, user.Senha);
                return Ok(new { message = "Usuário registrado!", userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                var token = await _firebaseService.LoginUser(login.Email, login.Senha);

                var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(login.Email);
                GlobalUsr.UserId = user.Uid;
                GlobalUsr.Nome = user.DisplayName ?? "Nome não definido";
                GlobalUsr.Email = user.Email;

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("save-dates")]
        public async Task<IActionResult> SaveDates([FromBody] DateRecordDto dateRecord)
        {
            try
            {
                if (string.IsNullOrEmpty(GlobalUsr.UserId))
                {
                    return BadRequest(new { error = "Usuário não autenticado." });
                }

                await _firebaseService.SaveDates(GlobalUsr.UserId, dateRecord.Data1, dateRecord.Data2, dateRecord.Data3);

                return Ok(new { message = "Datas salvas com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("get-dates")]
        public async Task<IActionResult> GetDates()
        {
            try
            {
                if (string.IsNullOrEmpty(GlobalUsr.UserId))
                {
                    return BadRequest(new { error = "Usuário não autenticado." });
                }

                var dataRecord = await _firebaseService.GetUserDates(GlobalUsr.UserId);

                if (dataRecord == null)
                {
                    return NotFound(new { error = "Nenhuma data encontrada para este usuário." });
                }

                return Ok(new
                {
                    dataRecord.Data1,
                    dataRecord.Data2,
                    dataRecord.Data3
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok("funcional");
        }

    }
}
public class UserDto
{
    public string Nome { get; set; } = "gui";
    public string Email { get; set; } = "gui@gmail";
    public string Senha { get; set; } = "123";
}


public class LoginDto
{
    public string Email { get; set; }
    public string Senha { get; set; }
}