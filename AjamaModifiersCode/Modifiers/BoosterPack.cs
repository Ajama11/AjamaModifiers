using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class BoosterPack() : AjamaModifier
{
    private static int _cardsToChoose = 5;
    
    private static int _numberOfCommonCards = 5;
    private static int _numberOfUncommonCards = 6;
    private static int _numberOfRareCards = 2;
    private static int _numberOfPowerCards = 2;
    
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        CardPoolModel cardPool = player.Character.CardPool;
        
        #region CardCreationOptions
        CardCreationOptions commonCardOptions = CardCreationOptions
            .ForNonCombatWithUniformOdds([cardPool], 
                c => c.Rarity == CardRarity.Common);
        commonCardOptions = Hook.ModifyCardRewardCreationOptions(player.RunState, player, commonCardOptions);
        
        CardCreationOptions uncommonCardOptions = CardCreationOptions
            .ForNonCombatWithUniformOdds([cardPool], 
                c => c.Rarity == CardRarity.Uncommon);
        uncommonCardOptions = Hook.ModifyCardRewardCreationOptions(player.RunState, player, uncommonCardOptions);
        
        CardCreationOptions rareCardOptions = CardCreationOptions
            .ForNonCombatWithUniformOdds([cardPool], 
                c => c.Rarity == CardRarity.Rare);
        rareCardOptions = Hook.ModifyCardRewardCreationOptions(player.RunState, player, rareCardOptions);
        #endregion

        #region Common, Uncommon, and Rare Cards
        IEnumerable<CardCreationResult> commonCards =
            CardFactory.CreateForReward(player, _numberOfCommonCards, commonCardOptions);
        
        IEnumerable<CardCreationResult> uncommonCards =
            CardFactory.CreateForReward(player, _numberOfUncommonCards, uncommonCardOptions);
        
        IEnumerable<CardCreationResult> rareCards =
            CardFactory.CreateForReward(player, _numberOfRareCards, rareCardOptions);
        #endregion

        #region Power Cards
        CardCreationOptions powerCardOptions = CardCreationOptions
            .ForNonCombatWithUniformOdds([cardPool], 
                c => c.Type == CardType.Power &&
                     IsNotAlreadyChosen(c, [..uncommonCards, ..rareCards]));
        powerCardOptions = Hook.ModifyCardRewardCreationOptions(player.RunState, player, powerCardOptions);
        
        IEnumerable<CardCreationResult> powerCards =
            CardFactory.CreateForReward(player, _numberOfPowerCards, powerCardOptions);
        #endregion

        #region Selection Screen

        IEnumerable<CardCreationResult> cardsToChooseFrom = [..commonCards, ..uncommonCards, ..rareCards, ..powerCards];
        
        cardsToChooseFrom = cardsToChooseFrom
            .OrderBy(r => r.Card.Rarity)
            .ThenBy(r => r.Card.TitleLocString.GetRawText());

        CardSelectorPrefs prefs = new (new LocString("modifiers",
                "AJAMAMODIFIERS-BOOSTER_PACK.selectionScreenPrompt"),
            _cardsToChoose)
        {
            Cancelable = false,
            RequireManualConfirmation = true
        };

        IEnumerable<CardModel> chosenCards =
            await CardSelectCmd.FromSimpleGridForRewards(
                new BlockingPlayerChoiceContext(), cardsToChooseFrom.ToList(), player, prefs);

        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(chosenCards, PileType.Deck), 1.2f, CardPreviewStyle.GridLayout);
        
        #endregion

        #region Relics

        List<RelicModel> relics =
        [
            ModelDb.Relic<PreciseScissors>().ToMutable(),
            ModelDb.Relic<PreciseScissors>().ToMutable(),
            ModelDb.Relic<Pomander>().ToMutable()
        ];

        List<Reward> rewards = relics.Select(
                (Func<RelicModel, Reward>) 
                (r => new RelicReward(r, player)))
            .ToList();
        
        await new RewardsSet(player)
            .WithCustomRewards(rewards)
            .Offer();
        
        #endregion
    }
    
    private static bool IsNotAlreadyChosen(CardModel card, IEnumerable<CardCreationResult> alreadyChosen)
    {
        return alreadyChosen.All(alreadyChosenResult => card.Id != alreadyChosenResult.originalCard.Id);
    }
}