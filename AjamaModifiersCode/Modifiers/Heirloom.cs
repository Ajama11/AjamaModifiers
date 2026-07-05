using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class Heirloom() : AjamaModifier
{
    public override int MySortOrder => -3;

    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        RelicModel relic = 
            player.RelicGrabBag.PullFromBack(
                RelicRarity.Rare, _ => true, player.RunState) 
            ?? ModelDb.Relic<Circlet>();

        await RelicCmd.Obtain(relic.ToMutable(), player);
    }
}