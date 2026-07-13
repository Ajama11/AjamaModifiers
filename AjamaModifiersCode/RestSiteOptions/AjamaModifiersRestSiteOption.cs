using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;

namespace AjamaModifiers.AjamaModifiersCode.RestSiteOptions;

public abstract class AjamaModifiersRestSiteOption(Player owner, string id) : CustomRestSiteOption(owner)
{
    public override string OptionId => $"AJAMAMODIFIERS-{id.ToUpperInvariant()}";

    public override string CustomIconPath =>
        Path.Join(
            MainFile.ModId, "images", "rest_site_options",
            $"{id.ToLowerInvariant()}.png"
        );
}