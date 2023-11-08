using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellStateCounter : MonoBehaviour
{
    public ForestFire3D forestFire; // Reference to your ForestFire3D script

    public int AlightCount { get; private set; }
    public int RockCount { get; private set; }
    public int GrassCount { get; private set; }
    public int TreeCount { get; private set; }
    public int BurntCount { get; private set; }
    public int TotalCellCount { get; private set; }

    public float PercentageBurntRock { get; private set; }
    public float PercentageTreeGrass { get; private set; }
    public float PercentageAlight { get; private set; }

    private void Start()
    {
        // Call CountCellStates every 0.25 second and repeat it
        InvokeRepeating("CountCellStates", 0.25f, 0.25f);
    }

    public void CountCellStates()
    {
        CountCellsInEachState();

        // Calculate the total cell count and percentages
        CalculatePercentages();

        // Display the counts and percentages in the Unity console
/*        Debug.Log($"Cell State Counts - Alight: {AlightCount}, Rock: {RockCount}, Grass: {GrassCount}, Tree: {TreeCount}, Burnt: {BurntCount}, Total Cells: {TotalCellCount}, " +
                  $"Percentage (Burnt + Rock): {PercentageBurntRock}%, Percentage (Tree + Grass): {PercentageTreeGrass}%, Percentage (Alight): {PercentageAlight}%");*/
    }

    public void CountCellsInEachState()
    {
        AlightCount = 0;
        RockCount = 0;
        GrassCount = 0;
        TreeCount = 0;
        BurntCount = 0;

        for (int xCount = 0; xCount < forestFire.gridSizeX; xCount++)
        {
            for (int yCount = 0; yCount < forestFire.gridSizeY; yCount++)
            {
                switch (forestFire.forestFireCells[xCount, yCount].cellState)
                {
                    case ForestFireCell.State.Alight:
                        AlightCount++;
                        break;
                    case ForestFireCell.State.Rock:
                        RockCount++;
                        break;
                    case ForestFireCell.State.Grass:
                        GrassCount++;
                        break;
                    case ForestFireCell.State.Tree:
                        TreeCount++;
                        break;
                    case ForestFireCell.State.Burnt:
                        BurntCount++;
                        break;
                }
            }
        }

        // Calculate the total cell count
        TotalCellCount = AlightCount + RockCount + GrassCount + TreeCount + BurntCount;
    }

    public void CalculatePercentages()
    {
        // Calculate the percentage values
        PercentageBurntRock = (float)(BurntCount + RockCount) / TotalCellCount * 100f;
        PercentageTreeGrass = (float)(TreeCount + GrassCount) / TotalCellCount * 100f;
        PercentageAlight = (float)AlightCount / TotalCellCount * 100f;
    }
}