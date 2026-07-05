using AjamaModifiers.AjamaModifiersCode.Modifiers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaModifiers.AjamaModifiersCode.Utils;

public class BarricadeGambitSingleton() : CustomSingletonModel(HookType.Combat)
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (combatState.Modifiers.All(m => m.Id != ModelDb.Modifier<BarricadeGambit>().Id)) return;
        
        if (player.PlayerCombatState!.TurnNumber > 1) return;

        await CreatureCmd.GainBlock(player.Creature, 50, ValueProp.Unpowered, null, true);

        await PowerCmd.Apply<BarricadePower>(choiceContext, player.Creature, 1, player.Creature, null);
        
        await PowerCmd.Apply<TheGambitPower>(choiceContext, player.Creature, 1, player.Creature, null);
    }
}