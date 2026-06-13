using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Utils;

public class TooManyCardsField
{
    public static readonly SavedSpireField<CardModel, string?> PowerOfFriendshipId = new(() => null, "AjamaModifiers-PowerOfFriendshipId");
}