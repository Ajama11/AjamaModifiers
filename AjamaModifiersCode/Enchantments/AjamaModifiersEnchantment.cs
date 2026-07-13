using BaseLib.Abstracts;
using BaseLib.Extensions;

namespace AjamaModifiers.AjamaModifiersCode.Enchantments;

public abstract class AjamaModifiersEnchantment : CustomEnchantmentModel
{
    protected override string CustomIconPath => 
        Path.Join(
            MainFile.ModId, "images", "enchantments",
            $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
        );
}