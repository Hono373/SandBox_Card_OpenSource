using ProjectLumiyaDB;
using System.Collections.Generic;
using UnityEngine;
public class MapData
{
    LevelData[][] area;
    public void Set(TaskInfo info, List<LevelInfo> pool)
    {
        area = new LevelData[info.Step][];
        int random;
        for (int i = 0; i < area.Length; i++)
        {
            if (IsEnding(i))
            {
                area[i] = new LevelData[1];
                area[i][0] = new();
                area[i][0].Set(info.End);
                continue;
            }

            random = Random.Range(1, info.Option + 1);
            area[i] = new LevelData[random];

            for (int j = 0; j < area[i].Length; j++)
            {
                area[i][j] = new();
                area[i][j].Set(pool.Pick().Key);
            }
        }
    }
    bool IsEnding(int v)
    {
        return v == area.Length - 1;
    }
    public Vector2Int Pos(LevelData data)
    {
        for (int i = 0; i < area.Length; i++)
        {
            for (int j = 0; j < area[i].Length; j++)
            {
                if (data == area[i][j])
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }
    public LevelData[] this[int index] => area[index];
    public int Length() => area.Length;
}
