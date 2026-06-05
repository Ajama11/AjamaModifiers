using AjamaModifiers.AjamaModifiersCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Modifiers;

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
    }
}