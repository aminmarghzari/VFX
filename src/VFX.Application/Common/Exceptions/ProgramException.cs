using VFX.Application.Common.Constants;
using VFX.Application.Common.Enums;

namespace VFX.Application.Common.Exceptions;

public static class ProgramException
{
    // Method to create a UserFriendlyException for missing application settings
    public static UserFriendlyException AppsettingNotSetException()
        => new(ErrorCode.Internal, ErrorMessage.AppConfigurationMessage, ErrorMessage.Internal);
}