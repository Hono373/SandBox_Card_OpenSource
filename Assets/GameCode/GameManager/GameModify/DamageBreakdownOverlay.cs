// 2026-10-23 伤害明细浮层：F8 开关，屏幕左下角显示最近几次命中。
//
// 排版原则（2026-10-23 第二次改，用户原话「语序太烂了，这谁看得懂啊」）：
//   · 一次命中 = 一行，不再拆「施法者行 + 目标行」两行说同一件事
//   · 没有修正就只给结果「火大地精 → 铁甲战士  4」，不摆「无修正 → 4」这种废话
//   · 有修正才展开成第二行，把每一步和中间值都写出来
//   · 连续相同的命中合并成 ×2，AOE 打同一只怪不刷屏
//
// 为什么用 OnGUI 而不是 UGUI：零 prefab、零场景改动，编辑器和打包本体都能跑；
// 也正因为零依赖，换新抽象（主循环设计那版）时这两个文件能整块搬走。
using UnityEngine;

public class DamageBreakdownOverlay : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        var go = new GameObject("[DamageBreakdownOverlay]");
        DontDestroyOnLoad(go);
        go.AddComponent<DamageBreakdownOverlay>();
    }

    const KeyCode ToggleKey = KeyCode.F8;

    bool visible = true;
    int cachedHeight = -1;
    GUIStyle headStyle;
    GUIStyle nameStyle;
    GUIStyle chainStyle;

    void RebuildStyles(int height)
    {
        float s = height / 1080f;
        int fontSize = Mathf.RoundToInt(19 * s);
        if (fontSize < 10) fontSize = 10;

        headStyle = new GUIStyle(GUI.skin.label);
        headStyle.fontSize = fontSize + 1;
        headStyle.fontStyle = FontStyle.Bold;
        headStyle.normal.textColor = new Color(1f, 0.85f, 0.4f);

        nameStyle = new GUIStyle(GUI.skin.label);
        nameStyle.fontSize = fontSize;
        nameStyle.normal.textColor = Color.white;

        chainStyle = new GUIStyle(GUI.skin.label);
        chainStyle.fontSize = fontSize;
        chainStyle.normal.textColor = new Color(0.98f, 0.82f, 0.35f);

        cachedHeight = height;
    }

    string BuildName(in DamageHit hit)
    {
        // 句式跟 Damage.Desc() 一致（「造成 N 点伤害」），不另造一套说法。
        string text = hit.caster + " → " + hit.target
                    + "   造成 " + hit.finalValue + " 点伤害";

        if (hit.repeat > 1)
            text += "   ×" + hit.repeat;

        return text;
    }

    /// <summary>只有发生修正时才画这一行：每一步连同中间值一起给出来</summary>
    string BuildChain(in DamageHit hit)
    {
        // 用「→ 力量 2 层 → 10」这种走法，倍率不写 ×1.5 —— 中间值本身就把结果说清楚了，
        // 再标一遍倍率等于同一件事说两次。
        string text = "      " + hit.baseValue;
        int current = hit.baseValue;

        if (hit.casterFlat != 0)
        {
            current += hit.casterFlat;
            text += "   →力量 " + hit.casterFlat + " 层→   " + current;
        }

        if (hit.targetFlat != 0)
        {
            current += hit.targetFlat;
            text += "   →减伤 " + (-hit.targetFlat) + "→   " + current;
        }

        if (hit.percent != 0)
            text += "   →易伤→   " + hit.finalValue;

        if (hit.forced)
            text += "   →强制值→   " + hit.finalValue;

        // 引擎的整除取整和这里的推算可能差一点，差就补一步，不让读者自己心算
        if (current != hit.finalValue && hit.percent == 0 && !hit.forced)
            text += "   →   " + hit.finalValue;

        return text;
    }

    void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == ToggleKey)
        {
            visible = !visible;
            Event.current.Use();
        }
        if (!visible) return;

        if (cachedHeight != Screen.height)
            RebuildStyles(Screen.height);

        var hits = DamageBreakdown.Hits;
        if (hits.Count == 0) return;

        float s = Screen.height / 1080f;
        float rowHeight = nameStyle.fontSize + 11 * s;

        int rowCount = 1;                       // 标题
        for (int i = 0; i < hits.Count; i++)
            rowCount += hits[i].Modified ? 2 : 1;

        // 2026-10-23 横幅形态：底色横跨整屏，文字块居中但内部左对齐。
        // 原来放左下角会压住手牌和抽/弃牌按钮，顶部这条本来就是空的。
        float textWidth = 820 * s;
        float height = rowHeight * rowCount + 18 * s;
        var banner = new Rect(0f, 14 * s, Screen.width, height);
        float textLeft = (Screen.width - textWidth) * 0.5f;

        Color saved = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.68f);
        GUI.DrawTexture(banner, Texture2D.whiteTexture);
        GUI.color = saved;

        float y = banner.y + 8 * s;
        GUI.Label(new Rect(textLeft, y, textWidth, rowHeight), "伤害明细（F8 关闭）", headStyle);
        y += rowHeight;

        for (int i = 0; i < hits.Count; i++)
        {
            DamageHit hit = hits[i];
            GUI.Label(new Rect(textLeft, y, textWidth, rowHeight), BuildName(hit), nameStyle);
            y += rowHeight;

            if (!hit.Modified) continue;
            GUI.Label(new Rect(textLeft, y, textWidth, rowHeight), BuildChain(hit), chainStyle);
            y += rowHeight;
        }
    }
}
