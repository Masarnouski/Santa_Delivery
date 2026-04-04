using UnityEngine;

// Глобальная скорость прокрутки мира
public static class WorldSpeed
{
    public static float Multiplier { get; private set; } = 1f;

    public static void SetBoost(bool boosting)
    {
        Multiplier = boosting ? 2.5f : 1f;
    }
}
