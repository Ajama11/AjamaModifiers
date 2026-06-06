using AjamaModifiers.AjamaModifiersCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Modifiers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class Winged() : AjamaModifier
{
    public override IEnumerable<ModifierModel> MutuallyExclusiveGroup => [ModelDb.Modifier<Flight>()];
    
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        await RelicCmd.Obtain(ModelDb.Relic<AdvancedWingedBoots>().ToMutable(), player);
        
        foreach (Player player1 in player.RunState.Players)
            player1.RelicGrabBag.Remove<WingedBoots>();
        player.RunState.SharedRelicGrabBag.Remove<WingedBoots>();
    }
}