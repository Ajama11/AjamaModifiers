using AjamaModifiers.AjamaModifiersCode.Cards;
using AjamaModifiers.AjamaModifiersCode.Relics;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Random;

namespace AjamaModifiers.AjamaModifiersCode.Cards;

[Pool(typeof(TokenCardPool))]
public class SoulstoneChoice() : AjamaModifiersCard(0,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    private string? _portraitPath;
    public CharacterModel? CharacterModel;
    
    protected override bool IsPlayable => false;
    public override int MaxUpgradeLevel => 0;

    public override CardPoolModel VisualCardPool => CharacterModel?.CardPool ?? base.VisualCardPool;
    public override string CustomPortraitPath => _portraitPath ?? base.CustomPortraitPath;
    
    public override string Title => CharacterModel == null
        ? "?"
        : new LocString("characters", CharacterModel.CharacterSelectTitle).GetFormattedText();
    
    public LocString GetDescription()
    {
        if (CharacterModel == null) return Description;

        var loc = ModelDb.Relic<Soulstone>().DynamicDescription;
        loc.Add("Character", CharacterModel?.Title.GetFormattedText() ?? "?");
        
        return loc;
    }
    
    public static SoulstoneChoice Create(CharacterModel characterModel)
    {
        var a = ModelDb.Card<SoulstoneChoice>().ToMutable();
        if (a is not SoulstoneChoice soulstoneChoice) throw new Exception("SoulstoneChoice model is not a SoulstoneChoice");
        
        CardModel? defend = characterModel.StartingDeck.FirstOrDefault(card => card.Tags.Contains(CardTag.Defend));

        if (defend is CustomCardModel baselibCard && !characterModel.Id.Entry.StartsWith("INTOTHESPIREVERSE"))
            soulstoneChoice._portraitPath = baselibCard.CustomPortraitPath;
        else
            soulstoneChoice._portraitPath = defend?.PortraitPath;

        if (!ResourceLoader.Exists(soulstoneChoice._portraitPath))
            soulstoneChoice._portraitPath = null;
        
        soulstoneChoice.CharacterModel = characterModel;
        
        NCard.FindOnTable(soulstoneChoice)?.Reload();
        
        return soulstoneChoice;
    }
}