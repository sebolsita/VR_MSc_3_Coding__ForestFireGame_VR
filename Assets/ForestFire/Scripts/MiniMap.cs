using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public ForestFire3D forestFire3D; // reference to the main forest fire 3D script

    public GameObject cellSprite; // sprite used to represent a cell on the grid

    public Transform spawnPosition; // initial spawn position
    public SpriteRenderer[,] cellSpriteRenderers = new SpriteRenderer[0, 0]; // an array to hold references to the sprite renderer component attached to each game object

    public GameObject player; // Reference to the player object (you'll need to assign this in the inspector)
    private Vector3 previousPlayerPosition; // [DEBUG]

    // Start is a built-in Unity function that is called before the first frame update
    private void Start()
    {
        // spawnPosition.position = new Vector3(spawnPosition.position.x - 50f, spawnPosition.position.y, spawnPosition.position.z);
        CreateGrid(forestFire3D.gridSizeX, forestFire3D.gridSizeY);
        // Initialize previousPlayerPosition to the player's initial position [DEBUG]
        previousPlayerPosition = player.transform.position;
    }

    private void CreateGrid(int sizeX, int sizeY)
    {
        // initialise the array of sprite renderers that will visualize the grid
        cellSpriteRenderers = new SpriteRenderer[sizeX, sizeY];

        for (int xCount = 0; xCount < sizeX; xCount++)
        {
            for (int yCount = 0; yCount < sizeY; yCount++)
            {
                // create cell sprite for each cell in the grid
                GameObject newCell = Instantiate(cellSprite);

                newCell.transform.SetParent(spawnPosition, true);
                newCell.transform.localPosition = Vector3.zero;
                newCell.transform.localRotation = Quaternion.identity;
                newCell.transform.localScale = new Vector3(0.035f, 0.035f, 0.035f);

                // position the cell on the grid, spacing them out using the x and y count as coordinates with a small offset
                newCell.transform.localPosition = new Vector3(xCount * 0.005f - (0.005f*20f), yCount * 0.005f + 0.005f, 0.0f);

                // add a reference of this sprite renderer to the array so we can change it later quickly
                cellSpriteRenderers[xCount, yCount] = newCell.GetComponent<SpriteRenderer>();
            }
        }
    }

    // Update is a built-in Unity function that is called once per frame
    private void Update()
    {
        // Go through every sprite in the mini-map grid and assign the color based on the state of each cell in the forest fire 3D script   
        for (int xCount = 0; xCount < forestFire3D.gridSizeX; xCount++)
        {
            for (int yCount = 0; yCount < forestFire3D.gridSizeY; yCount++)
            {
                if (forestFire3D.forestFireCells[xCount, yCount].cellState == ForestFireCell.State.Alight)
                {
                    cellSpriteRenderers[xCount, yCount].color = Color.red;
                }
                else if (forestFire3D.forestFireCells[xCount, yCount].cellState == ForestFireCell.State.Rock)
                {
                    cellSpriteRenderers[xCount, yCount].color = Color.grey;
                }
                else if (forestFire3D.forestFireCells[xCount, yCount].cellState != ForestFireCell.State.Rock && forestFire3D.forestFireCells[xCount, yCount].cellFuel <= 0)
                {
                    cellSpriteRenderers[xCount, yCount].color = Color.black;
                }
                else if (forestFire3D.forestFireCells[xCount, yCount].cellState == ForestFireCell.State.Grass)
                {
                    cellSpriteRenderers[xCount, yCount].color = Color.yellow;
                }
                else if (forestFire3D.forestFireCells[xCount, yCount].cellState == ForestFireCell.State.Tree)
                {
                    cellSpriteRenderers[xCount, yCount].color = Color.green;
                }
                else
                {
                    Debug.LogError("Object state is not recognized.");
                }
            }
        }
        playerPosition();

    }

    void playerPosition() // show player position on the minimap
    {
        Vector3 playerPosition = player.transform.position; // get player position
        int playerX = Mathf.RoundToInt(playerPosition.x / 4f); // divide and round the number to scale player's position in game world to the minimap grid size
        int playerY = Mathf.RoundToInt(playerPosition.z / 4f); // y position on the map grid

        if (playerX >= 0 && playerX < forestFire3D.gridSizeX && playerY >= 0 && playerY < forestFire3D.gridSizeY) // check if player is within the grid
        {
            cellSpriteRenderers[playerX, playerY].color = Color.blue; // render blue cell to represent the player position
        }
        // If the player's position has changed [DEBUG]
        if (player.transform.position != previousPlayerPosition)
        {
            // Print the player's new position
            Debug.Log("Player position: " + player.transform.position);
            Debug.Log("Minimap position: " + playerX + ", " + playerY);

            // Update previousPlayerPosition to the player's current position
            previousPlayerPosition = player.transform.position;
        }
    }
}