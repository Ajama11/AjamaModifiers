using AjamaModifiers.AjamaModifiersCode.Relics;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class VarietyPack : AjamaModifier
{ 
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        List<RelicModel> relics = 
        [
            ModelDb.Relic<AdvancedScrollBoxes>().ToMutable(),
            ModelDb.Relic<ScrollBoxes>().ToMutable(),
            ModelDb.Relic<NewLeaf>().ToMutable(),
            ModelDb.Relic<NewLeaf>().ToMutable(),
            ModelDb.Relic<Pomander>().ToMutable()
        ];

        List<Reward> rewards = relics.Select(
            (Func<RelicModel, Reward>) 
            (r => new RelicReward(r, player)))
            .ToList();

        await new RewardsSet(player)
            .WithCustomRewards(rewards)
            .WithSkippingDisallowed()
            .Offer();
    }
}