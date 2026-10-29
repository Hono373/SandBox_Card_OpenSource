/// <summary>
/// 游戏 UI 通用接口：Clear 用于回收前清空状态（置空引用、复位文本、隐藏等）。
/// 实现方约定：回收前调用 Clear() 后自身状态与新建对象等价，可安全复用。
/// </summary>
public interface IGameUI
{
    void Clear();
}
