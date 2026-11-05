using AjamaModifiers.AjamaModifiersCode.Relics;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AjamaModifiers.AjamaModifiersCode.Relics;

public class Soulstone() : AjamaModifiersRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    [SavedProperty]
    private ModelId? CharacterId { get; set; }

    private CharacterModel? Character => CharacterId != null ?
        ModelDb.GetByIdOrNull<CharacterModel>(CharacterId) :
        null;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DisplayVar<Soulstone>("Character", soulstone => soulstone.Character?.Title.GetFormattedText() ?? "?")
    ];

    public override LocString Title
    {
        get
        {
            var title = base.Title;
            
            title.Add("Character", Character?.Title.GetFormattedText() ?? "?");
            
            return title;
        }
    }
    
    public override string PackedIconPath =>
        Character?.IconTexture.ResourcePath ?? base.PackedIconPath;
    
    protected override string PackedIconOutlinePath =>
        Character?.IconOutlineTexture.ResourcePath ?? base.PackedIconOutlinePath;

    protected override string BigIconPath =>
        Character?.IconTexture.ResourcePath ?? base.BigIconPath;

    public void Initialize(CharacterModel? character)
    {
        AssertMutable();
        CharacterId = character?.Id;
        
        NRun.Instance?.GlobalUi.RelicInventory.RelicNodes
            .FirstOrDefault(holder => holder.Relic.Model == this)
            ?.Relic.Reload();
    }

    public override bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions)
    {
        if (Owner != player || creationOptions.Source != CardCreationSource.Encounter) return false;

        CardPoolModel? cardPool = Character?.CardPool;
        if (cardPool == null) return false;

        CardModel? card = CardFactory.CreateForReward(Owner, 1,
                new CardCreationOptions([cardPool],
                        CardCreationSource.None, creationOptions.RarityOdds)
                    .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications))
            .FirstOrDefault()
            ?.Card;

        if (card == null) return false;

        CardCreationResult result = new CardCreationResult(card);
        result.ModifyCard(card, this);
        cardRewardOptions.Add(result);
        
        return true;
    }

    public override Task AfterObtained()
    {
        if (CharacterId != null) return Task.CompletedTask;
        
        Initialize(ModelDb.AllCharacters.
            TakeRandom(1, Owner.PlayerRng.Rewards)
            .FirstOrDefault());
        
        return Task.CompletedTask;
    }
}