using UnityEngine;
public class RotationUtils
{
    public static Quaternion GetRandomSwingRotation()
    {
        var random = new System.Random();
        float randomAngle = (float)(random.NextDouble() * 10 - 5);
        Quaternion swingRotation = Quaternion.Euler(0, 0, randomAngle);
        return swingRotation;
    }
}
