﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ForestFire2D : MonoBehaviour
{
    public int gridSizeX; // x size of the grid
    public int gridSizeY; // y size of the grid
    public int nlight; // the number of trees to set alight at the start of the game
    public int xC, yC; // used for picking random x, y points

    public Sprite cellSprite; // sprite used to represent a cell on the grid
    public Text gameRunningText; // text used to display whether the game is running
    public int[,] gameArray = new int[0, 0];   // an array to hold the state data for each cell in the grid. int 0 represents dead state, int 1 represents alight state
    public int[,] objectArray = new int[0, 0]; // an array to hold the type of object for each cell in the grid. int 0 represents rock, 1 represents grass, 2 represents tree
    public int[,] fuelArray = new int[0, 0];   // an array to hold the amount of flammable material for each cell in the grid. 0 represents burnt out or non-flammable (e.g. rocks), >0 means potential to burn
    public int[,] gameArrayNextGen = new int[0, 0]; // an array to hold the state data for each cell in the grid for the next generation of the game
    public GameObject[,] cellGameObjects = new GameObject[0, 0]; // an array to hold references to each gameobject that make up grid
    public SpriteRenderer[,] cellSpriteRenderers = new SpriteRenderer[0, 0]; // an array to hold references to the sprite renderer component attached to each gameobject
    public bool gameRunning = false; // bool controlling whether the game is currently running

    [Range(0.01f, 3f)]
    public float updateRate; // used to define how often will the game update (in seconds)
    private float _gameTimer; // a variable that will be used detect when the game should update 

    private Camera gameCamera; // the game camera pointing at the board

    // Awake is a built-in Unity function that is only called once, before the Start function
    private void Awake()
    {
        // find the camera in the scene and store it for later
        gameCamera = FindObjectOfType<Camera>();
    }

    // Start is a built-in Unity function that is called before the first frame update
    private void Start()
    {
        CreateGrid(gridSizeX, gridSizeY);
        PauseGame(true);
        UpdateGridVisuals();
    }

    // this function controls whether or not to pause the game
    private void PauseGame(bool setGamePause)
    {
        // if setGamePause is true the game should stop running
        if (setGamePause)
        {
            gameRunning = false;
            gameRunningText.text = "Game Paused";
            gameRunningText.color = Color.red;
        }
        else // else if setGamePause is false unpause the game
        {
            gameRunning = true;
            gameRunningText.text = "Game Running";
            gameRunningText.color = Color.green;
        }
    }

    // Update is a built-in Unity function that is called once per frame 
    private void Update()
    {

        // check if the spacebar key has been pressed. this key will toggle between whether the game is currently running or paused
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // if the gameRunning is true, pause the game
            if (gameRunning)
            {
                PauseGame(true);
            }
            else // if the gameRunning is false, unpause the game
            {
                PauseGame(false);
            }
        }


        // check if the R key has been pressed. this key will randomise the grid
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            nlight = 2; // how many trees to set on fire
            // iterate through every cell in the cell in the grid and set its state to dead, decide what type of object is present and if flammable assign an amount of fuel
            for (int xCount = 0; xCount < gridSizeX; xCount++)
            {
                for (int yCount = 0; yCount < gridSizeY; yCount++)
                {
                    gameArray[xCount, yCount] = 0;  // nothing burning yet, set all objects to dead state
                    gameArrayNextGen[xCount, yCount] = 0;

                    xC = UnityEngine.Random.Range(0, 100); // generate a random number between 0 and 100

                    if (xC < 5)
                    {
                        objectArray[xCount, yCount] = 0; // set to rock, a 5% chance
                        fuelArray[xCount, yCount] = 0;   // if it's a rock then set fuel to 0
                    }
                    else if (xC < 30)
                    {
                        objectArray[xCount, yCount] = 1; // set to grass, a 25% chance
                        fuelArray[xCount, yCount] = UnityEngine.Random.Range(1, 5);  // if it's grass initiate a small amount of fuel (random between 1 and 5)
                    }
                    else
                    {
                        objectArray[xCount, yCount] = 2; // set to tree, a 70% chance
                        fuelArray[xCount, yCount] = UnityEngine.Random.Range(15, 25);  // if it's a tree initiate a larger amount of fuel (random between 15 and 25)
                    }
                }
            }



            do
            {
            xC = UnityEngine.Random.Range(0, gridSizeX);// now pick some trees at random and set them alight
            yC = UnityEngine.Random.Range(0, gridSizeY);
                if (objectArray[xC, yC] == 2)
                {
                    gameArray[xC, yC] = 1; // set cell to alight
                    nlight = nlight - 1;     // decrase number of trees to light by 1
                }
            } while (nlight > 0);  // when you've lit them all exit this loop
        }
                // update the visual state of each cell
                UpdateGridVisuals();

        // if the game is not running, return here to prevent the rest of the code in this Update function from running    
        if (gameRunning == false)
            return;

        if (_gameTimer < updateRate)
        {
            _gameTimer += Time.deltaTime;
        }
        else
        {
            UpdateCells();
            _gameTimer = 0f;
        }
    }


    // update the status of each cell on grid according to the rules of the game
    private void UpdateCells()
    {
        // iterate through each cell in the rows and columns
        for (int xCount = 0; xCount < gridSizeX; xCount++)
        {
            for (int yCount = 0; yCount < gridSizeY; yCount++)
            {
                // find out the number of alight neighbours this cell has
                int alightNeighbourCells = CountAlightNeighbourCells(xCount, yCount);

                if (gameArray[xCount, yCount] == 1) // if the cell is currently alight let it burn but reduce it's fuel and see if it goes out
                {
                    fuelArray[xCount, yCount] = fuelArray[xCount, yCount] - 1;  // reduce fuel by 1
                    if (fuelArray[xCount, yCount] <= 0)                         // has it burned all its fuel?
                    {
                        // cell has no fuel so is burned out 
                        gameArrayNextGen[xCount, yCount] = 0;
                    }
                    else // cell still has fuel so carries on burning
                    {
                        gameArrayNextGen[xCount, yCount] = gameArray[xCount, yCount];
                    }
                }
                else if (fuelArray[xCount, yCount] > 0)// the cell has fuel but is not alight yet
                {
                    // A dead cell with an alight neighbour which has fuel has a probablility of becoming an alight cell
                    if (alightNeighbourCells >0)
                    {
                        xC = UnityEngine.Random.Range(0, 100); // generate a random number between 0 and 100
                        if (xC < 10 * alightNeighbourCells) // the more alight neighbours the greater the probability of catching light
                                                            // e.g. 1 alight neighbour = 10 * 1 = 10% chance of catching fire, 2 alight neighbours = 10 * 2 = 20% chance of catching fire, etc.
                        {
                            gameArrayNextGen[xCount, yCount] = 1;  // a 10% chance of catching fire
                        }
                    }
                    else // no neighbours on fire, keep it dead for the next generation of the game
                    {
                        gameArrayNextGen[xCount, yCount] = 0;
                    }
                }
            }
        }

        // now the state of each cell has been calculated, apply the results by setting the current game array values to that of the next generation
        for (int xCount = 0; xCount < gridSizeX; xCount++)
        {
            for (int yCount = 0; yCount < gridSizeY; yCount++)
            {
                gameArray[xCount, yCount] = gameArrayNextGen[xCount, yCount];
            }
        }
    }

    // count the alight cells surrounding a specified cell on the grid 
    private int CountAlightNeighbourCells(int cellPositionX, int cellPositionY)
    {
        // create local variable to keep track of alight neighbour cells
        int alightNeighbourCells = 0;

        // the code below tries to iterate through the neighbour cells immediately surrounding the specified cell on the grid as well as the specified cell
        //
        // On the grid the it would like this
        //
        //  N N N
        //  N C N
        //  N N N
        //
        // N = neighbour C = cell that's neighbours are being counted

        for (int xPosition = cellPositionX - 1; xPosition < cellPositionX + 2; xPosition++)
        {
            for (int yPosition = cellPositionY - 1; yPosition < cellPositionY + 2; yPosition++)
            {
                // only continue if the neighbour is valid
                if (IsNeighbourValid(xPosition, yPosition))
                {
                    // if the cell is currently alight (1), increase the count of alight neighbours by one
                    if (gameArray[xPosition, yPosition] == 1)
                    {
                        alightNeighbourCells++;

                        // we don't want to check if the specified cell is alight, only its neighbours so it was added, subtract it
                        if (xPosition == cellPositionX && yPosition == cellPositionY)
                        {
                            alightNeighbourCells--;
                        }
                    }
                }
            }
        }

        // return the number of alight neighbour cells
        return alightNeighbourCells;
    }

    // make sure that the cell we are trying to count is not beyond the range of the game grid (edges of the game board)
    private bool IsNeighbourValid(int cellPositionX, int cellPositionY)
    {
        if (cellPositionX < 0 || cellPositionY < 0)
            return false;

        if (cellPositionX >= gridSizeX || cellPositionY >= gridSizeY)
            return false;

        return true;
    }

    // this function creates the grid of the game
    private void CreateGrid(int sizeX, int sizeY)
    {
        // initialise the game and fuel arrays to the size of grid
        gameArray = new int[sizeX, sizeY];
        objectArray = new int[sizeX, sizeY];
        fuelArray = new int[sizeX, sizeY];

        // initialise the game array that will contain the state for each cell in the next generation fo the game
        gameArrayNextGen = new int[sizeX, sizeY];

        // initialise the array of gameobjects that will hold the sprite renderers on the grid
        cellGameObjects = new GameObject[sizeX, sizeY];

        // initialise the array of sprite renderers that will visualise the grid
        cellSpriteRenderers = new SpriteRenderer[sizeX, sizeY];

        for (int xCount = 0; xCount < sizeX; xCount++)
        {
            for (int yCount = 0; yCount < sizeY; yCount++)
            {
                // create cell object and name it according to its position
                GameObject newCell = new GameObject("cell " + xCount + " " + yCount);

                //position the cell on the grid, spacing them out using the x and y count as coordinates 
                newCell.transform.position = new Vector3(xCount, yCount, 0);

                // add a sprite renderer to the cell object and assign the sprite it will use
                newCell.AddComponent<SpriteRenderer>().sprite = cellSprite;

                // add a reference of this sprite renderer to the array so we can change it later quickly
                cellSpriteRenderers[xCount, yCount] = newCell.GetComponent<SpriteRenderer>();

                // the size of the sprite is quite small, so increase the scale so there are no visable gaps in the grid
                newCell.transform.localScale = new Vector3(7.5f, 7.5f, 0f);

                // add a box collider to the cell so we can detect clicks from the mouse
                newCell.AddComponent<BoxCollider>();

                // add the gameobject of the cell to the array that stores references of the cell sprites
                cellGameObjects[xCount, yCount] = newCell;
            }
        }
    }

    // udpate the grid sprites colours according to their current state
    // this function will be called every frame of the game so the grid is always up to date 
    private void UpdateGridVisuals()
    {
        // iterate through each cell in the rows and columns
        for (int xCount = 0; xCount < gridSizeX; xCount++)
        {
            for (int yCount = 0; yCount < gridSizeY; yCount++)
            {
                // check if the state of the cell is 1 (alight)
                if (gameArray[xCount, yCount] == 1)
                {
                    cellSpriteRenderers[xCount, yCount].color = Color.red;
                }
                else if (objectArray[xCount, yCount] == 0) // rock
                {
                    cellSpriteRenderers[xCount, yCount].color = Color.grey;
                }
                else if (objectArray[xCount, yCount] != 0 && fuelArray[xCount, yCount] <= 0) // it's not a rock but it's fuel is zero, therefore it must be burnt out grass or tree
                {
                    cellSpriteRenderers[xCount, yCount].color = Color.black;
                }
                else if (objectArray[xCount, yCount] == 1) // unburnt grass
                {
                    cellSpriteRenderers[xCount, yCount].color = Color.yellow;
                }
                else if (objectArray[xCount, yCount] == 2) // unburnt tree
                {
                    cellSpriteRenderers[xCount, yCount].color = Color.green;
                }
                else // something has gone wrong, display an error message
                {
                    Debug.LogError("objectArray is not 0, 1 or 2, check code for errors");
                }
            }
        }
    }
}