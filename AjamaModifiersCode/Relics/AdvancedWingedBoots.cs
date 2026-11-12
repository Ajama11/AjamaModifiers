using AjamaModifiers.AjamaModifiersCode.Relics;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AjamaModifiers.AjamaModifiersCode.Relics;

public class AdvancedWingedBoots() : AjamaModifiersRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    private const string UsagesKey = "Usages";
    private int Usages { get; set; } = Config.WingedBaseUsages;
    
    private const string ExtraUsagesPerActKey = "ExtraUsagesPerAct";
    private int ExtraUsagesPerAct { get; set; } = Config.WingedUsagesPerAct;
    
    public override bool IsUsedUp => TimesUsed >= Usages;
    public override bool ShowCounter => !IsUsedUp;
    public override int DisplayAmount => Usages - TimesUsed;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new (UsagesKey, Usages),
        new (ExtraUsagesPerActKey, ExtraUsagesPerAct)
    ];

    [SavedProperty]
    private int TimesUsed
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            DynamicVars[UsagesKey].BaseValue = (Usages - field);
            InvokeDisplayAmountChanged();
            CheckIfUsedUp();
        }
    }

    public override bool ShouldAllowFreeTravel() => !IsUsedUp;
    
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (IsUsedUp || 
            Owner.RunState.CurrentRoomCount > 1 || 
            Owner.RunState is not RunState runState || 
            runState.VisitedMapCoords.Count <= 1 ||
            (NMapScreen.Instance?.IsDebugTravelEnabled ?? false))
        {
            return Task.CompletedTask;
        }
        
        IReadOnlyList<MapCoord> visitedMapCoords = runState.VisitedMapCoords;
        
        MapCoord coord = visitedMapCoords[^2];
        MapPoint? point = runState.Map.GetPoint(coord);
        
        if (point == null) return Task.CompletedTask;
        
        MapPoint? currentMapPoint = Owner.RunState.CurrentMapPoint;
        
        if (currentMapPoint == null || point.Children.Contains(currentMapPoint))
            return Task.CompletedTask;
        
        ++TimesUsed;
        
        return Task.CompletedTask;
    }

    public override Task AfterActEntered()
    {
        if (Owner.RunState.CurrentActIndex == 0)
        {
            // SfxCmd.Play(FmodSfx.merchantDisappointment, 5);
            // MainFile.Logger.Warn($"Advanced Winged Boots say it's Act 1 and received AfterActEntered because of some other mod.\nCurrentActIndex: {Owner.RunState.CurrentActIndex}\nTimesUsed before: {TimesUsed}\nTimesUsed theoretically afterwards but got skipped: {TimesUsed - ExtraUsagesPerAct}\nExtraUsagesPerAct: {ExtraUsagesPerAct}\nUsages: {Usages}");
            return Task.CompletedTask;
        }
        TimesUsed -= ExtraUsagesPerAct;
        return Task.CompletedTask;
    }

    private void CheckIfUsedUp()
    {
        Status = IsUsedUp ? RelicStatus.Disabled : RelicStatus.Normal;
    }
}