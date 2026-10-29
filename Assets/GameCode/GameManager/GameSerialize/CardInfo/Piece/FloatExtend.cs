using System;

public static class FloatExtend
{
    public static bool Check(this float floatPart)
    {
        // 处理边界情况：确保概率在0~1之间（避免负数或超过1的情况）
        floatPart = Math.Clamp(floatPart, 0f, 1f);

        // 生成0.0~1.0之间的随机数（NextDouble返回[0.0, 1.0)的double）
        double randomValue = UnityEngine.Random.value;

        // 随机数小于概率值则成功
        return randomValue < floatPart;
    }
}