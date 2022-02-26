using UnityEngine;

public class MathHelper
{
    public static float GetTimeBasedLerpFactor(float lerpFactor, float expectedFps = 60)
    {
        float ratio = Time.deltaTime * expectedFps;

        return 1 - Mathf.Pow(1 - lerpFactor, ratio);
    }

    public static float TimeBasedLerp(float start, float end, float lerpFactor)
    {
        return Mathf.Lerp(start, end, MathHelper.GetTimeBasedLerpFactor(lerpFactor));
    }

    public static Vector2 TimeBasedLerp(Vector2 start, Vector2 end, float lerpFactor)
    {
        return Vector2.Lerp(start, end, MathHelper.GetTimeBasedLerpFactor(lerpFactor));
    }

    public static float ShortestLoopDelta(float start, float target, float max)
    {
        float delta = target - start;

        if (Mathf.Abs(delta) <= max / 2)
        {
            return delta;

        }
        else
        {
            if (delta >= 0)
            {
                return (target - max) - start;
            }
            else
            {
                return (target + max) - start;
            }
        }
    }
}
