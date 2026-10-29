using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardArrangeCalculation
{
    public ArrangeType arrangeType = ArrangeType.Bezier;
    public Center center = new();
    public ArcConfig arc = new();

    [Serializable]
    public class Center
    {
        public Vector2 center = new(0, 0f);
        public float spacing = 500;
        int unuseCount(int count) => (DeckBuildBattleRule.Get().cardLimit - count).GreaterThanOrEqual();
        public float spacingOffset => spacing / DeckBuildBattleRule.Get().cardLimit;
        public float Spacing(int count) => spacing - spacingOffset * unuseCount(count);

        public float controlY = 200;
        public float controlOffset => controlY / DeckBuildBattleRule.Get().cardLimit;
        public float ControlY(int count) => controlY + controlOffset * unuseCount(count) / 20;

        public float aimY = -500;
        public Vector2 GetStart(int count) => new Vector2(center.x - Spacing(count), center.y);
        public Vector2 GetEnd(int count) => new Vector2(center.x + Spacing(count), center.y);
        public Vector2 GetControl(int count) => new Vector2(center.x, center.y + ControlY(count));
        public Vector2 GetAim() => new Vector2(center.x, center.y + aimY);
    }

    [Serializable]
    public class ArcConfig
    {
        public Vector2 center = new(0, -560f);
        public float aimY = -3000f;
        public float perCardAngle = 3.5f;
        public float maxArcAngle = 30f;
        public float radius2 = 2000f;
        public float radius10 = 2750f;

        public Vector2 GetAim() => new Vector2(center.x, center.y + aimY);
    }

    public ArrangeData ResultOld(int count)
    {
        if (arrangeType == ArrangeType.Arc)
            return ArcResult(count);

        var points = GetBezierPoints(count);
        var aimPoint = center.GetAim();
        var rotations = GetCardRotations(points, aimPoint);
        return new ArrangeData(points.ToArray(), rotations.ToArray(), Vector3.one);
    }
    public List<MotionData> Result(int count)
    {
        var data = ResultOld(count);
        var list = new List<MotionData>();
        for (var i = 0; i < data.points.Length; i++)
        {
            list.Add(new MotionData(data.points[i], data.rotates[i], data.scales));
        }
        return list;
    }

    ArrangeData ArcResult(int count)
    {
        var points = GetArcPoints(count);
        var aimPoint = arc.GetAim();
        var rotations = GetCardRotations(points, aimPoint);
        return new ArrangeData(points.ToArray(), rotations.ToArray(), Vector3.one);
    }

    List<Vector2> GetBezierPoints(int count)
    {
        List<Vector2> points = new();

        var start = center.GetStart(count);
        var end = center.GetEnd(count);
        var control = center.GetControl(count);

        for (int i = 1; i <= count; i++)
        {
            float t = (float)i / (count + 1);
            Vector2 point = Bezier(start, control, end, t);
            points.Add(point);
        }
        return points;
    }

    List<Vector2> GetArcPoints(int count)
    {
        List<Vector2> points = new();
        var p = arc;

        float arcAng = Mathf.Min(count * p.perCardAngle, p.maxArcAngle);
        float half = arcAng / 2f;
        float radius = count <= 2 ? p.radius2
            : count >= 10 ? p.radius10
            : p.radius2 + (p.radius10 - p.radius2) * (count - 2) / 8f;

        for (int i = 0; i < count; i++)
        {
            float t = count == 1 ? 0 : (float)i / (count - 1);
            float ang = (-half + arcAng * t) * Mathf.Deg2Rad;
            float ux = p.center.x + radius * Mathf.Sin(ang);
            float uy = p.center.y - radius + radius * Mathf.Cos(ang); // 上弧（圆顶部）
            points.Add(new Vector2(ux, uy));
        }
        return points;
    }

    Vector2 Bezier(Vector2 start, Vector2 midPoint, Vector2 end, float t)
    {
        float u = 1 - t;
        return u * u * start + 2 * u * t * midPoint + t * t * end;
    }
    List<Quaternion> GetCardRotations(List<Vector2> cardPosList, Vector2 focusPos)
    {
        List<Quaternion> rotations = new();

        foreach (var cardPos in cardPosList)
        {
            Quaternion rotation = GetRotationToTarget(cardPos, focusPos);
            rotations.Add(rotation);
        }
        return rotations;
    }
    Quaternion GetRotationToTarget(Vector2 cardPos, Vector2 focusPos)
    {
        Vector2 direction = focusPos - cardPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        return Quaternion.Euler(0, 0, angle);
    }
}
