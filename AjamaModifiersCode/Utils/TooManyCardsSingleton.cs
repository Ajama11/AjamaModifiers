using AjamaModifiers.AjamaModifiersCode.Modifiers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Utils;

public class TooManyCardsSingleton() : CustomSingletonModel(HookType.Combat)
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
}