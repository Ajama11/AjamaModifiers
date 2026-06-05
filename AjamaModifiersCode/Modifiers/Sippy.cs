using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class Sippy() : AjamaModifier
{
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        await PotionCmd.TryToProcure(
            PotionFactory.CreateRandomPotionOutOfCombat(player, player.RunState.Rng.CombatPotionGeneration).ToMutable(), player);
    }
}