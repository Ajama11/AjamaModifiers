using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class ShrympHeavenNow() : AjamaModifier
{
    public override int SortOrder => -1;
    
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        await RelicCmd.Obtain(ModelDb.Relic<ElectricShrymp>().ToMutable(), player);
    }
}