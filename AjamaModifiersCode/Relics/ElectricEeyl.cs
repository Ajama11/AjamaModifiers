using AjamaModifiers.AjamaModifiersCode.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;

namespace AjamaModifiers.AjamaModifiersCode.Relics;

public class ElectricEeyl() : AjamaModifiersRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CardsVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
        HoverTipFactory.FromEnchantment<PowerImbued>();

    public override async Task AfterObtained()
    {
        PowerImbued imbued = ModelDb.Enchantment<PowerImbued>();

        bool hasPowerThatCanBeEnchanted = Owner.Deck.Cards.Any(
            card => imbued.CanEnchant(card));

        // IEnumerable<CardModel> cardsToEnchant = [];
        
        if (hasPowerThatCanBeEnchanted)
        {
            CardSelectorPrefs prefs = new (CardSelectorPrefs.EnchantSelectionPrompt, 1);

            IEnumerable<CardModel> cardsToEnchant = await CardSelectCmd
                .FromDeckForEnchantment(Owner, imbued, 1, prefs);
            
            foreach (CardModel cardToEnchant in cardsToEnchant)
            {
                CardCmd.Enchant(imbued.ToMutable(), cardToEnchant, 1);
                CardCmd.Preview(cardToEnchant);
            }
        }
        else
        {
            CardCreationOptions options =
                new CardCreationOptions(
                        [Owner.Character.CardPool], 
                        CardCreationSource.Other,
                        CardRarityOddsType.EliteEncounter, 
                        c => c.Type == CardType.Power)
                    .WithFlags(CardCreationFlags.NoUpgradeRoll);

            List<CardModel> cardsToPickFrom = CardFactory.CreateForReward(Owner, DynamicVars.Cards.IntValue, options)
                .Select(r => r.Card)
                .ToList();

            foreach (CardModel card in cardsToPickFrom)
            {
                CardCmd.Enchant(imbued.ToMutable(), card, 1);
            }

            CardModel? chosenCard =
                await CardSelectCmd.FromChooseACardScreen(
                    new BlockingPlayerChoiceContext(), 
                    cardsToPickFrom, Owner, true);

            if (chosenCard != null)
            {
                await CardPileCmd.Add(chosenCard, PileType.Deck);
                // cardsToEnchant = [chosenCard];
            }

            foreach (CardModel cardNotPicked in cardsToPickFrom)
            {
                if (cardNotPicked == chosenCard) continue;
                
                Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(Owner.NetId).CardChoices.Add(new CardChoiceHistoryEntry(cardNotPicked, false));
            }
        }
    }
}