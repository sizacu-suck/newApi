namespace Teacing_api.Registr
{
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // По умолчанию регистрируем как обычного юзера
    }
}
