using System.Reflection;
using AjamaModifiers.AjamaModifiersCode.Cards;
using BaseLib.Config;
using BaseLib.Patches.Localization;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "AjamaModifiers"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();
        ModConfigRegistry.Register(ModId, new Config());
        
        //If you want to use scripts defined in your mod for Godot scenes, uncomment the following line.
        //Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);

        Harmony harmony = new(ModId);
        harmony.PatchAll(assembly);

        DescriptionOverrides.CustomizeDescriptionPost += (card, _, ref description) =>
        {
            if (card is not SoulstoneChoice soulstoneChoice) return;

            description = soulstoneChoice.GetDescription().GetFormattedText();
        };
    }
}