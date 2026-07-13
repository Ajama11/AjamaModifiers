using AjamaModifiers.AjamaModifiersCode.Enchantments;
using AjamaModifiers.AjamaModifiersCode.Relics;
using AjamaModifiers.AjamaModifiersCode.RestSiteOptions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AjamaModifiers.AjamaModifiersCode.Relics;

public class CondensedTime() : AjamaModifiersRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    
    private const string AmountRemaining = "AmountRemaining";
    private const int _maxAmount = 3;
    
    public override bool IsUsedUp => TimesUsed >= _maxAmount;
    public override bool ShowCounter => !IsUsedUp;
    public override int DisplayAmount => _maxAmount - TimesUsed;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new (AmountRemaining, _maxAmount)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromEnchantment<Accelerated>()
    ];

    [SavedProperty]
    public int TimesUsed
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            DynamicVars[AmountRemaining].BaseValue = (_maxAmount - field);
            InvokeDisplayAmountChanged();
            CheckIfUsedUp();
        }
    }

    private void CheckIfUsedUp()
    {
        Status = IsUsedUp ? RelicStatus.Disabled : RelicStatus.Normal;
    }

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner) return false;
        if (IsUsedUp) return false;
        
        options.Add(new CondensedTimeRestSiteOption(player));
        return true;
    }
}