using AjamaModifiers.AjamaModifiersCode.Cards;
using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.Frame), MethodType.Getter)]
public static class SoulstoneCardFramePatch
{
    [HarmonyPrefix]
    static bool UseAltTexture(CardModel __instance, ref Texture2D? __result)
    {
        if (__instance is not SoulstoneChoice soulstoneChoice) return true;
        
        if (__instance.VisualCardPool is not CustomCardPoolModel customCardPool) return true;
        
        __result = customCardPool.CustomFrame(soulstoneChoice);
        return __result == null;
    }
}