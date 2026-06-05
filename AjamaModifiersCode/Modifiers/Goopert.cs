using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class Goopert() : AjamaModifier
{
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    public override IEnumerable<ModifierModel> MutuallyExclusiveGroup => [ModelDb.Modifier<ByrdFriend>()];

    private static async Task DoThings(Player player)
    {
        await RelicCmd.Obtain(ModelDb.Relic<PaelsLegion>().ToMutable(), player);
    }
}