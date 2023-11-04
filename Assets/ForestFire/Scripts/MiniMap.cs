using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;


public class MiniMap : MonoBehaviour
{
    public ForestFire3D forestFire3D; // reference to the main forest fire 3D script

    public GameObject cellSprite; // sprite used to represent a cell on the grid

    public Transform spawnPosition; // initial spawn position
    public SpriteRenderer[,] cellSpriteRenderers = new SpriteRenderer[0, 0]; // an array to hold references to the sprite renderer component attached to each game object

    public GameObject player; // Reference to the player object @seb
    private Vector3 previousPlayerPosition; // [DEBUG] @seb

    private int notBurningCount = 0; // Count of cells that are not burning
    private int onFireCount = 0;     // Count of cells that are on fire
    private int burnedCount = 0;     // Count of cells that are burned

    public GameObject notBurningLabel; // Reference to the GameObject containing TextMeshPro components
    public GameObject onFireLabel;
    public GameObject burnedLabel;

    public GameObject playerHealthLabel;
    public PlayerHealthController playerHealthController;

    public TextMeshPro scoreLabel;
    public TextMeshPro timeLabel;


    /*public Slider healthSlider; // Reference to the Slider component for displaying health @seb*/
    public float NotBurningPercentage { get; private set; }
    public float OnFirePercentage { get; private set; }
    public float BurnedPercentage { get; private set; }

    // Start is a built-in Unity function that is called before the first frame update
    private void Start()
    {
        // Check if GameObjects are assigned
        if (notBurningLabel == null)
        {
            Debug.LogError("notBurningLabel is not assigned.");
        }
        if (onFireLabel == null)
        {
            Debug.LogError("onFireLabel is not assigned.");
        }
        if (burnedLabel == null)
        {
            Debug.LogError("burnedLabel is not assigned.");
        }

        CreateGrid(forestFire3D.gridSizeX, forestFire3D.gridSizeY);
        previousPlayerPosition = player.transform.position; // Initialize previousPlayerPosition to the player's initial position (to refresh only when player moves) [DEBUG] @seb
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
                newCell.transform.localPosition = new Vector3(xCount * 0.005f - (0.005f*20f), yCount * 0.005f + 0.075f, 0.0f); // offset the spawn position - minimap in the middle @seb

                // add a reference of this sprite renderer to the array so we can change it later quickly
                cellSpriteRenderers[xCount, yCount] = newCell.GetComponent<SpriteRenderer>();
            }
        }
    }

    // Update is a built-in Unity function that is called once per frame
    private void Update()
    {
        // Reset the counts at the beginning of each frame @seb
        notBurningCount = 0;
        onFireCount = 0;
        burnedCount = 0;

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

        // Go through every sprite in the mini-map grid and update counts @seb
        for (int xCount = 0; xCount < forestFire3D.gridSizeX; xCount++)
        {
            for (int yCount = 0; yCount < forestFire3D.gridSizeY; yCount++)
            {
                // Update counts based on the state of each cell in the forest fire 3D script @seb   
                if (forestFire3D.forestFireCells[xCount, yCount].cellState == ForestFireCell.State.Alight)
                {
                    onFireCount++;
                }
                else if (forestFire3D.forestFireCells[xCount, yCount].cellFuel <= 0) // Stones included @seb
                {
                    burnedCount++;
                }
                else
                {
                    notBurningCount++;
                }
            }
        }

        // Calculate percentages @seb
        int totalCells = forestFire3D.gridSizeX * forestFire3D.gridSizeY;
        NotBurningPercentage = (float)notBurningCount / totalCells * 100f;
        OnFirePercentage = (float)onFireCount / totalCells * 100f;
        BurnedPercentage = (float)burnedCount / totalCells * 100f;

        // Output the percentages DEBUG @seb
/*        Debug.Log("notBurningLabel: " + notBurningLabel);
        Debug.Log("onFireLabel: " + onFireLabel);
        Debug.Log("burnedLabel: " + burnedLabel);
        Debug.Log("Not Burning Percentage: " + NotBurningPercentage);
        Debug.Log("On Fire Percentage: " + OnFirePercentage);
        Debug.Log("Burned Percentage: " + BurnedPercentage);*/

        // Check if the GameObjects are not null before accessing their TMP components
        if (notBurningLabel != null && onFireLabel != null && burnedLabel != null)
        {
            // Access the TextMeshPro component within the GameObject @seb
            TextMeshPro notBurningText = notBurningLabel.GetComponent<TextMeshPro>();
            TextMeshPro onFireText = onFireLabel.GetComponent<TextMeshPro>();
            TextMeshPro burnedText = burnedLabel.GetComponent<TextMeshPro>();

            // Update the text content of the TMP labels with the calculated percentages @seb
            notBurningText.text = NotBurningPercentage.ToString("F0") + "%"; // "F0" formats the float to two decimal places
            onFireText.text = OnFirePercentage.ToString("F0") + "%";
            burnedText.text = BurnedPercentage.ToString("F0") + "%";
        }
        else
        {
            Debug.Log("Labels not found (NULL)");
        }
        playerPosition(); // @seb
        int playerHealth = playerHealthController.GetPlayerHealth();
        TextMeshPro playerHealthText = playerHealthLabel.GetComponent<TextMeshPro>();
        playerHealthText.text = playerHealth.ToString("F0") + "HP";
    }

    void playerPosition() // show player position on the minimap @seb
    {
        Vector3 playerPosition = player.transform.position; // get player position
        int playerX = Mathf.RoundToInt(playerPosition.x / 4f); // divide and round the number to scale player's position in game world to the minimap grid size
        int playerY = Mathf.RoundToInt(playerPosition.z / 4f); // y position on the map grid

        if (playerX >= 0 && playerX < forestFire3D.gridSizeX && playerY >= 0 && playerY < forestFire3D.gridSizeY) // check if player is within the grid
        {
            cellSpriteRenderers[playerX, playerY].color = Color.blue; // render blue cell to represent the player position
        }
        // If the player's position has changed [DEBUG] @seb
/*        if (player.transform.position != previousPlayerPosition)
        {
            // Print the player's new position
            Debug.Log("Player position: " + player.transform.position);
            Debug.Log("Minimap position: " + playerX + ", " + playerY);

            // Update previousPlayerPosition to the player's current position
            previousPlayerPosition = player.transform.position;
        }*/
    }
}