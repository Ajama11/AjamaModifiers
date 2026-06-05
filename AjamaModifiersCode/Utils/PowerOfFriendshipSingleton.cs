using AjamaModifiers.AjamaModifiersCode.Modifiers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Utils;

public class PowerOfFriendshipSingleton() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.RunState!.Modifiers.All(m => m.Id != ModelDb.Modifier<PowerOfFriendship>().Id)) return;
        
        if (cardPlay.Card.Type != CardType.Power) return;
        if (PowerOfFriendshipField.IsDupe.Get(cardPlay.Card)) return;

        foreach (Player otherPlayer in cardPlay.Card.RunState!.Players)
        {
            if (otherPlayer == cardPlay.Card.Owner) continue;
            
            CardModel clone = cardPlay.Card.CreateClone();
            clone._owner = otherPlayer;
            PowerOfFriendshipField.IsDupe.Set(clone, true);

            // await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Play, otherPlayer);
            await CardCmd.AutoPlay(choiceContext, clone, null);
        }
    }
}