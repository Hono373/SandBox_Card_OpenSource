using System.Collections.Generic;
public interface IUnitSelector
{
    /// <summary>
    /// 具体实现由胶水层决定
    /// </summary>
    /// <param name="targetList">IUnit的id列表</param>
    void Trim(List<string> targetList);
}
