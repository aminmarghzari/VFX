using VFX.Application.Common.Constants;
using VFX.Application.Common.Enums;

namespace VFX.Application.Common.Exceptions;

// This class contains exception handling related to transactions
// It provides methods for throwing user-friendly exceptions when a transaction fails
public static class TransactionException
{

    // Method to create a UserFriendlyException when a transaction cannot be committed
    public static UserFriendlyException TransactionNotCommitException()
        => throw new UserFriendlyException(ErrorCode.Internal, ErrorMessage.TransactionNotCommit, ErrorMessage.TransactionNotCommit);

    // Method to create a UserFriendlyException when a transaction cannot be executed
    public static UserFriendlyException TransactionNotExecuteException(Exception ex)
        => throw new UserFriendlyException(ErrorCode.Internal, ErrorMessage.TransactionNotExecute, ErrorMessage.TransactionNotExecute, ex);
}
