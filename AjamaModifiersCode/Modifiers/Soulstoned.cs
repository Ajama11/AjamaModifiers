using AjamaModifiers.AjamaModifiersCode.Cards;
using AjamaModifiers.AjamaModifiersCode.Relics;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class Soulstoned() : AjamaModifier
{
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return () => DoThings(eventModel.Owner!);
    }

    private static async Task DoThings(Player player)
    {
        List<SoulstoneChoice> choices = [];
        
        List<CharacterModel> characters = ModelDb.AllCharacters
            .Where(character => character.GetType() != player.Character.GetType())
            .TakeRandom(Config.SoulstonedCharacterOptions, player.PlayerRng.Rewards)
            .ToList();

        foreach (var character in characters)
        {
            choices.Add(SoulstoneChoice.Create(character));
        }

        CardModel? card;
        
        if (Config.SoulstonedCharacterOptions > 3)
        {
            LocString prompt = new LocString("modifiers", "AJAMAMODIFIERS-SOULSTONED.selectionScreenPrompt");
            
            card = (await CardSelectCmd.FromSimpleGrid(
                    new BlockingPlayerChoiceContext(), choices, player,
                    new CardSelectorPrefs(prompt, 1)
                    {
                        Cancelable = false
                    }))
                .FirstOrDefault();
        }
        else
        {
            card = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), choices, player);
        }
        
        if (card is not SoulstoneChoice selection) return;

        var soulstone = (Soulstone) ModelDb.Relic<Soulstone>().ToMutable();
        soulstone.Initialize(selection.CharacterModel);

        await RelicCmd.Obtain(soulstone, player);
    }
    
    public override LocString Description
    {
        get
        {
            var loc = new LocString("modifiers", Id.Entry + ".description");
            
            loc.Add("Options", Config.SoulstonedCharacterOptions);
            
            return loc;
        }
    }
}