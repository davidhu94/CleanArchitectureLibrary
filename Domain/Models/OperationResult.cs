namespace Domain.Models
{
    public class OperationResult<T>
    {
        public bool IsSuccessful { get; private set; }
        public string Message { get; private set; }
        public T Data { get; private set; }
        public string ErrorMessage { get; private set; }

        private OperationResult(bool isSuccessful, T data, string message,  string errorMessage)
        {
            IsSuccessful = isSuccessful;
            Message = string.IsNullOrWhiteSpace(message) ? (isSuccessful ? "Operation Successful" : "An error occurred.") : message;
            Data = data;
            ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) && !isSuccessful ? "An error occurred." : errorMessage;
        }

        public static OperationResult<T> Success(T data, string message = "Operation Successful")
        {
            return new OperationResult<T> (true, data, message, null);
        }
        public static OperationResult<T> Failure(string errorMessage, string message = null)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("Error message must not be null or empty", nameof(errorMessage));

            return new OperationResult<T>(false, default, message ?? errorMessage, errorMessage);
        }
    }
}
