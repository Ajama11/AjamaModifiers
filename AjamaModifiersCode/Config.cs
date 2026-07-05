using BaseLib.Config;

namespace AjamaModifiers.AjamaModifiersCode;

internal class Config : SimpleModConfig
{
    [ConfigHoverTip]
    public static bool TooManyCardsPowerOfFriendship { get; set; } = true;
}