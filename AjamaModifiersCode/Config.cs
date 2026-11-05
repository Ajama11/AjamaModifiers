using System.Diagnostics.CodeAnalysis;
using BaseLib.Config;
using BaseLib.Config.UI;
using Godot;
using MegaCrit.Sts2.Core.Models;

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
    
    [ConfigSection("Soulstoned")]
    [ConfigSlider(2, 1000)]
    public static int SoulstonedCharacterOptions { get; set; } = 2;

    public override void SetupConfigUI(Control optionContainer)
    {
        base.SetupConfigUI(optionContainer);

        int totalOtherCharactersLoaded = ModelDb.AllCharacters.Count() - 1;
        
        var sliderRow = optionContainer.GetNodeOrNull<NConfigOptionRow>($"%{nameof(SoulstonedCharacterOptions)}");
        
        if (sliderRow?.SettingControl is NConfigSlider slider)
        {
            // Update the slider limits
            slider.SetRange(2, totalOtherCharactersLoaded, 1);
        
            // Make the slider 500 px wide (default is 324 px)
            slider.CustomMinimumSize = new Vector2(500, slider.CustomMinimumSize.Y);
        }
    }
}