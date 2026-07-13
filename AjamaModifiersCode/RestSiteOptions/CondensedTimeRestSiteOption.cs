using AjamaModifiers.AjamaModifiersCode.Enchantments;
using AjamaModifiers.AjamaModifiersCode.Relics;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.RestSiteOptions;

public class CondensedTimeRestSiteOption(Player owner) : AjamaModifiersRestSiteOption(owner, "accelerate")
{
    public override async Task<bool> OnSelect()
    {
        Accelerated accelerated = ModelDb.Enchantment<Accelerated>();
        
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };
        
        List<CardModel> cardsToEnchant = (await CardSelectCmd
            .FromDeckForEnchantment(Owner, accelerated, 1, prefs))
            .ToList();

        if (cardsToEnchant.Count == 0) return false;
        
        foreach (CardModel cardToEnchant in cardsToEnchant)
        {
            CardCmd.Enchant(accelerated.ToMutable(), cardToEnchant, 1);
            CardCmd.Preview(cardToEnchant);
        }
        
        //If the player somehow has multiple Condensed Times, like using the console to fix a misclick Enchant, properly use a charge, instead of defaulting to the first one that's already Used Up and letting the 2nd one infinitely provide Enchants.
        IEnumerable<RelicModel> relics = Owner.Relics
            .Where(r => r is CondensedTime);

        foreach (var relic in relics)
        {
            CondensedTime condensedTime = (relic as CondensedTime)!;
            
            if (condensedTime.IsUsedUp) continue;
            
            condensedTime.TimesUsed++;
            break;
        }

        return true;
    }
}