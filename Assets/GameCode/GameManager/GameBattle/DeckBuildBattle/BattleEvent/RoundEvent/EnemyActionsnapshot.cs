public class EnemyActionsnapshot
{
    public Unit source;
    EnemyActionsnapshot() { }
    internal static EnemyActionsnapshot CreateToExcute(Unit source)
    {
        var e = new EnemyActionsnapshot();
        e.source = source;
        return e;
    }
}