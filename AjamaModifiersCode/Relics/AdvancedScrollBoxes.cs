using AjamaModifiers.AjamaModifiersCode.Relics;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

namespace AjamaModifiers.AjamaModifiersCode.Relics;

public class AdvancedScrollBoxes() : AjamaModifiersRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    public override async Task AfterObtained()
    {
        List<IReadOnlyList<CardModel>> randomBundles = GenerateRandomBundles(Owner);
        List<IReadOnlyList<CardModel>> list = [];

        foreach (IReadOnlyList<CardModel> bundle in randomBundles)
        {
            list.Add(bundle.Select(c => Owner.RunState.CreateCard(c, Owner)).ToList());
        }

        foreach (CardModel card in await CardSelectCmd.FromChooseABundleScreen(Owner, list))
        {
            await CardPileCmd.Add(card, PileType.Deck);
        }
    }

    private static List<IReadOnlyList<CardModel>> GenerateRandomBundles(Player player)
    {
        List<IReadOnlyList<CardModel>> results = [];
        
        Rng rng = player.PlayerRng.Rewards;
        CardPoolModel cardPool = player.Character.CardPool;

        CardCreationOptions optionsUncommon = CardCreationOptions
            .ForNonCombatWithUniformOdds([cardPool], c => c.Rarity == CardRarity.Uncommon)
            .WithFlags(CardCreationFlags.NoRarityModification);
        optionsUncommon = Hook.ModifyCardRewardCreationOptions(player.RunState, player, optionsUncommon);
        
        CardCreationOptions optionsRare = CardCreationOptions
            .ForNonCombatWithUniformOdds([cardPool], c => c.Rarity == CardRarity.Rare)
            .WithFlags(CardCreationFlags.NoRarityModification);
        optionsRare = Hook.ModifyCardRewardCreationOptions(player.RunState, player, optionsRare);

        List<CardModel> uncommonCards = optionsUncommon.GetPossibleCards(player).ToList();
        List<CardModel> rareCards = optionsRare.GetPossibleCards(player).ToList();

        HashSet<ModelId> usedCardIds = [];

        for (int packs = 0; packs < 3; ++packs)
        {
            List<CardModel> cardPack = [];
            List<CardModel> possibleUncommonCards = uncommonCards.Where(c => !usedCardIds.Contains(c.Id)).ToList();

            for (int uncommonCardsIndex = 0; uncommonCardsIndex < 2; ++uncommonCardsIndex)
            {
                CardModel uncommonCard = rng.NextItem(possibleUncommonCards)!;
                cardPack.Add(uncommonCard);
                usedCardIds.Add(uncommonCard.Id);
                possibleUncommonCards.Remove(uncommonCard);
            }

            List<CardModel> possibleRareCards = rareCards.Where(c => !usedCardIds.Contains(c.Id)).ToList();

            CardModel rareCard = rng.NextItem(possibleRareCards)!;
            cardPack.Add(rareCard);
            usedCardIds.Add(rareCard.Id);
            
            results.Add(cardPack);
        }

        return results;
    }
}