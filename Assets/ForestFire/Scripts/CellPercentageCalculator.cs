using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPercentageCalculator : MonoBehaviour
{
    public float CalculateNotBurningPercentage(int notBurningCount, int totalCells)
    {
        return ((float)notBurningCount / totalCells) * 100f;
    }

    public float CalculateOnFirePercentage(int onFireCount, int totalCells)
    {
        return ((float)onFireCount / totalCells) * 100f;
    }

    public float CalculateBurnedPercentage(int burnedCount, int totalCells)
    {
        return ((float)burnedCount / totalCells) * 100f;
    }
}