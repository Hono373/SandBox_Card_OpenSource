/// <summary>空流程：只占位、不 Join 任何动画，供 NullSkillEffect 使用。</summary>
public class NullFlow : BattleFlow
{
    public NullFlow Start()
    {
        Init();
        return this;
    }
}
