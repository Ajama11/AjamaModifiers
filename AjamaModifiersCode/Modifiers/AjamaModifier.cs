using BaseLib.Abstracts;
using BaseLib.Extensions;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public abstract class AjamaModifier : CustomModifierModel
{
    public override ModifierAlignment Alignment => ModifierAlignment.Good;

    public override int SortOrder => -2;

    protected override string IconPath =>
        Path.Join(
            MainFile.ModId, "images", "modifiers",
            $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
        );
}