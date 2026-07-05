using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Modifiers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class Pandora() : AjamaModifier
{
    public override int MySortOrder => -2;
    
    public override IEnumerable<ModifierModel> MutuallyExclusiveGroup => 
    [
        ModelDb.Modifier<Draft>(),
        ModelDb.Modifier<SealedDeck>(),
        ModelDb.Modifier<Insanity>()
    ];
    
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        await RelicCmd.Obtain(ModelDb.Relic<PandorasBox>().ToMutable(), player);
        
        foreach (Player player1 in player.RunState.Players)
        {
            player1.RelicGrabBag.Remove<PandorasBox>();
            player1.RelicGrabBag.Remove<GhostSeed>();
            player1.RelicGrabBag.Remove<NeowsTalisman>();
        }
        
        player.RunState.SharedRelicGrabBag.Remove<PandorasBox>();
        player.RunState.SharedRelicGrabBag.Remove<GhostSeed>();
        player.RunState.SharedRelicGrabBag.Remove<NeowsTalisman>();
    }
}