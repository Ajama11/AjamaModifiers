using BaseLib.Abstracts;
using BaseLib.Extensions;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public abstract class AjamaModifier : CustomModifierModel
{
    public override ModifierAlignment Alignment => ModifierAlignment.Good;

    public virtual int MySortOrder => -1;
    
    public override int SortOrder => Config.PlaceModifiersAtBottom ? 99 + MySortOrder : MySortOrder;

    protected override string IconPath =>
        Path.Join(
            MainFile.ModId, "images", "modifiers",
            $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
        );
}