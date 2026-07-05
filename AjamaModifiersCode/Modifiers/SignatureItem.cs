using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class SignatureItem() : AjamaModifier
{
    public override int MySortOrder => -3;
    
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        List<RelicModel> characterRelics = player.Character.RelicPool
            .GetUnlockedRelics(player.UnlockState)
            .Where(r => 
                r.Rarity 
                    is RelicRarity.Common 
                    or RelicRarity.Uncommon 
                    or RelicRarity.Rare)
            .ToList();
        
        player.PlayerRng.Rewards.Shuffle(characterRelics);
        
        RelicModel relic = 
            characterRelics.FirstOrDefault() 
            ?? ModelDb.Relic<DarkstonePeriapt>();

        await RelicCmd.Obtain(relic.ToMutable(), player);
    }
}