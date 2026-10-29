using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(CardInfoCreator))]
public class CardInfoCreator : InfoCreator<CardInfo>
{
    /// <summary>把 CardInfo 摊平成 DB.xlsx 的行集合，供「同步回表格」按钮使用。</summary>
    protected override bool TryMapRows(CardInfo obj, out List<Dictionary<int, object>> rows)
    {
        return CardInfoRowMapper.TryToRows(obj, out rows);
    }

    /// <summary>卡表的源表格，用于「数据是否已过期」检查。</summary>
    protected override string SourceXlsxRelativePath => @"Assets\GameDataBase\DB.xlsx";

    /// <summary>下拉显示为 "[key]中文名"，便于辨认（Odin 与各处 ValueDropdown 共用）。</summary>
    protected override string GetDisplayName(CardInfo obj) =>
        string.IsNullOrEmpty(obj?.cardName) ? null : obj.cardName;
}
