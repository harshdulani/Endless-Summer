using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public static bool isDayActiveLight = false, isNightActiveLight = true;

    private void Start()
    {
        isDayActiveLight = false;
        isNightActiveLight = true;
    }

    public void NightLightFarAway()
    {
        isNightActiveLight = false;
        isDayActiveLight = true;
    }

    public void NightLightActive()
    {
        isNightActiveLight = true;
        isDayActiveLight = false;
    }
}
