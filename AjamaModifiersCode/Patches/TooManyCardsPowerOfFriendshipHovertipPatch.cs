using AjamaModifiers.AjamaModifiersCode.Modifiers;
using AjamaModifiers.AjamaModifiersCode.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Patches;

[HarmonyPatch(typeof(CardModel), "HoverTips", MethodType.Getter)]
public static class TooManyCardsPowerOfFriendshipHovertipPatch
{
    [HarmonyPostfix]
    public static IEnumerable<IHoverTip> HashDisplayPostfix(IEnumerable<IHoverTip> __result, CardModel __instance)
    {
        if (__instance.CombatState == null) return __result;
        
        if (__instance.RunState!.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return __result;
        if (__instance.RunState.Modifiers.All(m => m.Id != ModelDb.Modifier<PowerOfFriendship>().Id)) return __result;
        
        // Check config here

        string? field = TooManyCardsField.PowerOfFriendshipId.Get(__instance);
        if (field == null) return __result;

        string hash = field.Split("(")[1].Split(")")[0];

        LocString title = new LocString("static_hover_tips", "AJAMAMODIFIERS-POWER_OF_FRIENDSHIP.title");
        
        LocString description = new LocString("static_hover_tips", "AJAMAMODIFIERS-POWER_OF_FRIENDSHIP.description");

        Texture2D icon = ModelDb.Modifier<PowerOfFriendship>().Icon;
        
        description.Add("Hash", hash);
        description.Add("Power", __instance.TitleLocString.GetFormattedText());

        HoverTip hoverTip = new HoverTip(title, description, icon);

        return [..__result, hoverTip];
    }
}