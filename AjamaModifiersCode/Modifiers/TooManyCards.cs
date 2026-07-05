using AjamaModifiers.AjamaModifiersCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace AjamaModifiers.AjamaModifiersCode.Modifiers;

public class TooManyCards() : AjamaModifier
{
    public override int MySortOrder => -5;
}