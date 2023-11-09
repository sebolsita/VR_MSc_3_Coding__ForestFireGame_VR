using System.Collections; // Import the System.Collections namespace for working with coroutines.
using System.Collections.Generic; // Import the System.Collections.Generic namespace for working with lists.
using UnityEngine; // Import the UnityEngine namespace for Unity functionality.

public class CellStateCounter : MonoBehaviour
{
    public ForestFire3D forestFire; // Reference to your ForestFire3D script.

    public int AlightCount { get; private set; } // Count of cells that are alight (on fire).
    public int RockCount { get; private set; } // Count of cells that are rocks.
    public int GrassCount { get; private set; } // Count of cells that are grass.
    public int TreeCount { get; private set; } // Count of cells that are trees.
    public int BurntCount { get; private set; } // Count of cells that are burnt.
    public int TotalCellCount { get; private set; } // Total count of all cells.

    public float PercentageBurntRock { get; private set; } // Percentage of burnt and rock cells.
    public float PercentageTreeGrass { get; private set; } // Percentage of tree and grass cells.
    public float PercentageAlight { get; private set; } // Percentage of alight cells.

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

        // Display the counts and percentages in the Unity console (commented out)
        // Debug.Log($"Cell State Counts - Alight: {AlightCount}, Rock: {RockCount}, Grass: {GrassCount}, Tree: {TreeCount}, Burnt: {BurntCount}, Total Cells: {TotalCellCount}, " +
        //           $"Percentage (Burnt + Rock): {PercentageBurntRock}%, Percentage (Tree + Grass): {PercentageTreeGrass}%, Percentage (Alight): {PercentageAlight}%");
    }

    public void CountCellsInEachState()
    {
        // Initialize counts for each cell state
        AlightCount = 0;
        RockCount = 0;
        GrassCount = 0;
        TreeCount = 0;
        BurntCount = 0;

        // Iterate through the grid and count cells in each state
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