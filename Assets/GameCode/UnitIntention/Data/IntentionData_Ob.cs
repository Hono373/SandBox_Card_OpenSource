using System;
using System.Collections.Generic;

[Serializable]
public class IntentionData_Ob
{
    public List<int?> skillIndexs;
    public IntentionData_Ob() { }
    public IntentionData_Ob(List<int?> skillIndexs) { this.skillIndexs = skillIndexs; }

    public override string ToString() => $"IntentionData {{ skillIndexs = [{string.Join(", ", skillIndexs)}] }}";
}
