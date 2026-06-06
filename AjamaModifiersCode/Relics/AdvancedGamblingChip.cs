using AjamaModifiers.AjamaModifiersCode.Relics;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Relics;

public class AdvancedGamblingChip() : AjamaModifiersRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];
    
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner) return count;
        
        return count + DynamicVars.Cards.BaseValue;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        
        List<CardModel> list = (await CardSelectCmd.FromHandForDiscard(
                choiceContext, Owner, 
                new CardSelectorPrefs(SelectionScreenPrompt, 
                    0, 999999999), 
                null, this))
            .ToList();
        
        if (list.Count == 0) return;
        
        await CardCmd.DiscardAndDraw(choiceContext, list, list.Count);
    }
}