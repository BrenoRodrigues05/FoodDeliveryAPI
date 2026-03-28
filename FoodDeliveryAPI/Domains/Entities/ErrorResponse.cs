namespace FoodDeliveryAPI.Domains.Entities
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        /// Em produção, permanece nulo por motivos de segurança.
        public string? Details { get; set; }
        public ErrorResponse(int statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
    }
}
