using AjamaModifiers.AjamaModifiersCode.Modifiers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Utils;

public class MirroringSingleton() : CustomSingletonModel(HookType.Combat)
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (combatState.Modifiers.All(m => m.Id != ModelDb.Modifier<Mirroring>().Id)) return;
        
        if (player.PlayerCombatState!.TurnNumber > 1) return;
        
        int highestOrbSlotsAmount = player.Character.BaseOrbSlotCount;
        
        foreach (Player otherPlayer in player.RunState.Players)
        {
            if (otherPlayer == player) continue;

            if (otherPlayer.Character.BaseOrbSlotCount > highestOrbSlotsAmount)
                highestOrbSlotsAmount = otherPlayer.Character.BaseOrbSlotCount;
        }

        if (highestOrbSlotsAmount > player.Character.BaseOrbSlotCount)
            await OrbCmd.AddSlots(player, highestOrbSlotsAmount - player.PlayerCombatState!.OrbQueue.Capacity);
    }
}