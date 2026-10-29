/// <summary>空修改：技能效果没有可修正项时返回它，保证 ActiveModify/PassiveModify 永不为 null，
/// 消费方可以直接调用 Settle 而不需要判空。</summary>
public class NullGameModify : GameModify
{
}
