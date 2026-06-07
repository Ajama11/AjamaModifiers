using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class BarricadeGambit() : AjamaModifier
{
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        foreach (Player player1 in player.RunState.Players)
        {
            player1.RelicGrabBag.Remove<SturdyClamp>();
            player1.RelicGrabBag.Remove<Orichalcum>();
            player1.RelicGrabBag.Remove<FakeOrichalcum>();
        }
        player.RunState.SharedRelicGrabBag.Remove<SturdyClamp>();
        player.RunState.SharedRelicGrabBag.Remove<Orichalcum>();
        player.RunState.SharedRelicGrabBag.Remove<FakeOrichalcum>();
    }
}