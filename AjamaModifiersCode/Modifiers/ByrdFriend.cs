using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class ByrdFriend() : AjamaModifier
{
    public override IEnumerable<ModifierModel> MutuallyExclusiveGroup => [ModelDb.Modifier<Goopert>()];
    
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        await RelicCmd.Obtain(ModelDb.Relic<Byrdpip>().ToMutable(), player);

        CardModel card = player.RunState.CreateCard<ByrdSwoop>(player);
        
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
    }
}