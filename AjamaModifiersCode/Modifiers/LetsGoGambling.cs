using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class LetsGoGambling() : AjamaModifier
{
    public override int MySortOrder => 1;

    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        LocString selectionScreenPrompt =
            LocString.GetIfExists("modifiers", "AJAMAMODIFIERS-LETS_GO_GAMBLING.selectionScreenPrompt")!;
        CardSelectorPrefs prefs = new (selectionScreenPrompt, 2);

        List<CardModel> selectedCards = (await CardSelectCmd.FromDeckForTransformation(player, prefs)).ToList();

        await CardCmd.TransformTo<Jackpot>(selectedCards[0]);
        await CardCmd.TransformTo<Discovery>(selectedCards[1]);
    }
}