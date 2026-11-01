using System.Diagnostics.CodeAnalysis;
using BaseLib.Config;

namespace AjamaModifiers.AjamaModifiersCode;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
internal class Config : SimpleModConfig
{
    [ConfigHoverTip]
    public static bool TooManyCardsPowerOfFriendship { get; set; } = true;
    
    [ConfigHoverTip]
    public static bool PlaceModifiersAtBottom { get; set; } = false;
    
    [ConfigSection("Winged")]
    [ConfigHoverTip]
    [ConfigSlider(0, 10)]
    public static int WingedBaseUsages { get; set; } = 3;
    
    [ConfigHoverTip]
    [ConfigSlider(0, 10)]
    public static int WingedUsagesPerAct { get; set; } = 3;
}