using DG.Tweening;
using System.Collections.Generic;

public class UnitRoundEndSnapshot
{
    public List<Unit> targets;
    public Sequence gameSequencePackage;
    public UnitRoundEndSnapshot() { }
    public UnitRoundEndSnapshot(Unit player) { targets = new List<Unit> { player }; }
    public UnitRoundEndSnapshot(List<Unit> targets) { this.targets = targets; }
}
