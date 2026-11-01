using AjamaModifiers.AjamaModifiersCode.Relics;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AjamaModifiers.AjamaModifiersCode.Relics;

public class AdvancedWingedBoots() : AjamaModifiersRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    private const string RoomsKey = "Rooms";
    private const int _roomCount = 3;
    
    private const string ExtraRoomsPerActKey = "ExtraRoomsPerAct";
    private const int _extraRoomsPerAct = 3;
    
    public override bool IsUsedUp => TimesUsed >= _roomCount;
    public override bool ShowCounter => !IsUsedUp;
    public override int DisplayAmount => _roomCount - TimesUsed;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new (RoomsKey, _roomCount),
        new (ExtraRoomsPerActKey, _extraRoomsPerAct)
    ];

    [SavedProperty]
    private int TimesUsed
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            DynamicVars[RoomsKey].BaseValue = (_roomCount - field);
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
            runState.VisitedMapCoords.Count <= 1)
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
        if (Owner.RunState.CurrentActIndex == 0) return Task.CompletedTask;
        TimesUsed -= _extraRoomsPerAct;
        return Task.CompletedTask;
    }

    private void CheckIfUsedUp()
    {
        Status = IsUsedUp ? RelicStatus.Disabled : RelicStatus.Normal;
    }
}