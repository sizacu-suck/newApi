using System.Net;
using System.Text.Json;

namespace Teacing_api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        // Через конструктор мы получаем ссылку на следующее Middleware и наш логгер
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger; // Вот этого присвоения как раз не хватало!
        }

        // Этот метод вызывается автоматически при каждом HTTP-запросе
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Просто передаем запрос дальше по конвейеру (к следующим middleware и контроллеру)
                await _next(context);
            }
            catch ( Exception ex)
            {
                _logger.LogError(ex, "Произошло непредвиденное исключение при обработке запроса: {Message}", ex.Message);
                // Если где-то дальше (в контроллере) случилась ошибка — мы её ловим здесь!
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Устанавливаем статус-код ответа (500 Внутренняя ошибка сервера)
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Формируем красивый и понятный JSON для клиента вместо системного лога
            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "На сервере произошла непредвиденная ошибка.",
                Detailed = exception.Message // В продакшене эту строчку обычно скрывают от клиентов
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}