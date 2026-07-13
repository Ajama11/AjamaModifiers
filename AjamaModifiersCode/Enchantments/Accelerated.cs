using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Enchantments;

public class Accelerated : AjamaModifiersEnchantment
{
    public override bool CanEnchant(CardModel card)
    {
        return base.CanEnchant(card) && card.EnergyCost.GetWithModifiers(CostModifiers.All) > 0;
    }

    protected override void OnEnchant()
    {
        Card.EnergyCost.UpgradeBy(-1);
    }
}