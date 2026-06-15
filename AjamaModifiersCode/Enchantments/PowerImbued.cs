using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;

namespace AjamaModifiers.AjamaModifiersCode.Enchantments;

public class PowerImbued : CustomEnchantmentModel
{
    protected override string CustomIconPath => 
        ImageHelper.GetImagePath("enchantments/imbued.png");

    public override bool CanEnchantCardType(CardType cardType) => cardType == CardType.Power;

    public override bool ShouldStartAtBottomOfDrawPile => true;

    public override bool ShowAmount => false;

    public override async Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Card.Owner || player.PlayerCombatState!.TurnNumber > 1) return;

        await CardCmd.AutoPlay(choiceContext, Card, null);
    }
}