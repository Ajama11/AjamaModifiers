using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class Mirroring() : AjamaModifier
{
    public override int SortOrder => -6;
    
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        foreach (Player otherPlayer in player.RunState.Players)
        {
            if (otherPlayer == player) continue;

            IReadOnlyList<RelicModel> starterRelics = otherPlayer.Character.StartingRelics;

            foreach (RelicModel starterRelic in starterRelics)
            {
                if (player.Relics.All(r => r.Id != starterRelic.Id))
                    await RelicCmd.Obtain(starterRelic.ToMutable(), player);
            }
        }
    }
}