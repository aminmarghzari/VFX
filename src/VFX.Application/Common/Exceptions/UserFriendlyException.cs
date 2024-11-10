using VFX.Application.Common.Enums;

namespace VFX.Application.Common.Exceptions;

// Custom exception class that extends the base Exception class
// This exception is used to provide a user-friendly message alongside an error code
public class UserFriendlyException : Exception
{
    public string UserFriendlyMessage { get; set; }
    public ErrorCode ErrorCode { get; set; }

    public UserFriendlyException(ErrorCode errorCode, string message, string userFriendlyMessage, Exception? innerException = null) : base(message, innerException)
    {
        ErrorCode = errorCode;
        UserFriendlyMessage = userFriendlyMessage;
    }
}
