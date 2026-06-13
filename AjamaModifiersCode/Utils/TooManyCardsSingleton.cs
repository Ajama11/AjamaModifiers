using AjamaModifiers.AjamaModifiersCode.Modifiers;
using BaseLib.Abstracts;
using BaseLib.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaModifiers.AjamaModifiersCode.Utils;

public class TooManyCardsSingleton() : CustomSingletonModel(HookType.Combat), IMaxHandSizeModifier
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (combatState.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return;
        
        if (player.PlayerCombatState!.TurnNumber > 1) return;
        
        if (combatState.Modifiers.Any(m => m.Id == ModelDb.Modifier<PowerOfFriendship>().Id))
        {
            foreach (CardModel ownCard in player.PlayerCombatState.AllCards)
            {
                if (ownCard is { Type: CardType.Power, DeckVersion: not null })
                {
                    TooManyCardsField.PowerOfFriendshipId.Set(
                        ownCard, 
                        TooManyCardsField.PowerOfFriendshipId.Get(ownCard.DeckVersion));
                }
            }
        }

        foreach (Player otherPlayer in combatState.Players)
        {
            if (otherPlayer == player) continue;

            List<CardModel> otherDeck = PileType.Deck.GetPile(otherPlayer).Cards.ToList();

            foreach (CardModel otherCard in otherDeck)
            {
                var otherCardClone = combatState.CloneCard(otherCard);
                otherCardClone._owner = player;
                otherCardClone.DeckVersion = otherCard;
                
                await CardPileCmd.AddGeneratedCardToCombat(otherCardClone,
                    PileType.Draw, null,
                    CardPilePosition.Random);

                if (combatState.Modifiers.Any(m => m.Id == ModelDb.Modifier<PowerOfFriendship>().Id) &&
                    otherCard.Type == CardType.Power)
                {
                    string? sharedId = TooManyCardsField.PowerOfFriendshipId.Get(otherCard);
                    
                    TooManyCardsField.PowerOfFriendshipId.Set(otherCardClone, sharedId);
                    // MainFile.Logger.Info(
                    //     player.Character.Title.GetRawText() + 
                    //     " is creating a copy of " + 
                    //     otherPlayer.Character.Title.GetRawText() + 
                    //     "'s " + 
                    //     sharedId);
                }
            }
        }
        
        // foreach (var combatCard in player.PlayerCombatState!.AllCards)
        // {
        //     string? combatCardId = TooManyCardsField.PowerOfFriendshipId.Get(combatCard);
        //     
        //     if (combatCardId != null)
        //         MainFile.Logger.Info(player.Character.Title.GetRawText() + ": " + combatCard.Title + " is " + combatCardId);
        // }
    }

    public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    {
        if (player.RunState.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return currentMaxHandSize;

        return currentMaxHandSize + (5 * (player.RunState.Players.Count - 1));
    }
    
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player.RunState.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return count;
        
        return count + (5 * (player.RunState.Players.Count - 1));
    }

    /// Fix the first turn draw thinking max Hand size is 10 and not drawing past that
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.RunState.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return;
        
        if (player.PlayerCombatState!.TurnNumber != 1) return;

        var cardsInHand = PileType.Hand.GetPile(player).Cards.Count;
        var handDrawAmount = Hook.ModifyHandDraw(player.Creature.CombatState!, player, 5, out _);

        await CardPileCmd.Draw(choiceContext, handDrawAmount - cardsInHand, player, true);
    }

    /// Scale Queen's debuff to match the new draw-per-turn amount
    public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature? target,
        CardModel? cardSource)
    {
        if (power is not ChainsOfBindingPower) return 0;
        if (target is { IsPlayer: false }) return 0;

        Player player = target!.Player!;
        
        if (player.RunState.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return 0;

        return 3 * (player.RunState.Players.Count - 1);
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.CombatState!.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return;
        if (cardPlay.Card.CombatState!.Modifiers.All(m => m.Id != ModelDb.Modifier<PowerOfFriendship>().Id)) return;
        
        if (TooManyCardsField.PowerOfFriendshipId.Get(cardPlay.Card) == null)
        {
            // MainFile.Logger.Info(
            //     cardPlay.Card.Owner.Character.Title.GetRawText() + 
            //     " played " + 
            //     cardPlay.Card.Title + 
            //     " and " + 
            //     LocalContext.GetMe(cardPlay.Card.CombatState)!.Character.Title.GetRawText() + 
            //     " sees PowerOfFriendshipId as NULL");
            return;
        }

        string powerOfFriendshipId = TooManyCardsField.PowerOfFriendshipId.Get(cardPlay.Card)!;
        
        // MainFile.Logger.Info(
        //     cardPlay.Card.Owner.Character.Title.GetRawText() + 
        //     " played " + 
        //     cardPlay.Card.Title + 
        //     " and " + 
        //     LocalContext.GetMe(cardPlay.Card.CombatState)!.Character.Title.GetRawText() + 
        //     " sees PowerOfFriendshipId as " +
        //     powerOfFriendshipId);

        foreach (Player otherPlayer in cardPlay.Card.CombatState!.Players)
        {
            if (otherPlayer == cardPlay.Card.Owner) continue;

            await CardPileCmd.RemoveFromCombat(otherPlayer.PlayerCombatState!.AllCards.Where(c =>
                TooManyCardsField.PowerOfFriendshipId.Get(c) == powerOfFriendshipId)
                .ToList());
        }
    }
}

public class TooManyCardsSingletonRunHooks() : CustomSingletonModel(HookType.Run)
{
    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        newCard = null;
        
        if (card.Owner.RunState.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return false;
        if (card.Owner.RunState.Modifiers.All(m => m.Id != ModelDb.Modifier<PowerOfFriendship>().Id)) return false;

        if (card.Type != CardType.Power) return false;

        newCard = card.Owner.RunState.CloneCard(card);

        string powerOfFriendshipId = newCard + " from Player " +
                                     newCard.Owner.NetId;

        // int hash = RuntimeHelpers.GetHashCode(this);
        
        TooManyCardsField.PowerOfFriendshipId.Set(newCard, powerOfFriendshipId);
        
        // MainFile.Logger.Info(
        //     newCard.Owner.Character.Title.GetRawText() + 
        //     " can add " + 
        //     powerOfFriendshipId + 
        //     " to the deck"
        //     );

        return true;
    }
}