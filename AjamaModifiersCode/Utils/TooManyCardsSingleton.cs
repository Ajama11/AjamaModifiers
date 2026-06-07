using AjamaModifiers.AjamaModifiersCode.Modifiers;
using BaseLib.Abstracts;
using BaseLib.Hooks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
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

        foreach (Player otherPlayer in combatState.Players)
        {
            if (otherPlayer == player) continue;

            List<CardModel> otherDeck = PileType.Deck.GetPile(otherPlayer).Cards.ToList();

            foreach (CardModel otherCard in otherDeck)
            {
                var otherCardClone = combatState.CloneCard(otherCard);
                otherCardClone._owner = player;
                
                await CardPileCmd.AddGeneratedCardToCombat(otherCardClone,
                    PileType.Draw, null,
                    CardPilePosition.Random);
            }
        }
    }

    public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    {
        if (player.RunState.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return currentMaxHandSize;

        return currentMaxHandSize + (5 * (player.RunState.Players.Count - 1));
    }
    
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        // if (player != Owner) return count;
        if (player.RunState.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return count;
        
        return count + (5 * (player.RunState.Players.Count - 1));
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.RunState.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return;
        
        if (player.PlayerCombatState!.TurnNumber != 1) return;

        var cardsInHand = PileType.Hand.GetPile(player).Cards.Count;
        var handDrawAmount = Hook.ModifyHandDraw(player.Creature.CombatState!, player, 5, out _);

        await CardPileCmd.Draw(choiceContext, handDrawAmount - cardsInHand, player, true);
    }

    public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature? target,
        CardModel? cardSource)
    {
        if (power is not ChainsOfBindingPower) return 0;
        if (target is { IsPlayer: false }) return 0;

        Player player = target!.Player!;
        
        if (player.RunState.Modifiers.All(m => m.Id != ModelDb.Modifier<TooManyCards>().Id)) return 0;

        return 3 * (player.RunState.Players.Count - 1);
    }
}