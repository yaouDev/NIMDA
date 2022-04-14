using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text; //only for debug

public class Level
{
    public Module[,] matrix;
    public Module root { get; private set; }
    public Module lastActive;

    public Level(int width, int height)
    {
        CreateMatrix(width, height);
        lastActive = root;
    }

    private void CreateMatrix(int width, int height)
    {
        Module[,] matrix = new Module[width, height];

        List<Module> list;
        LevelGenerator.instance.rotations.TryGetValue("nswe", out list);
        root = PickRandom(list);
        matrix[width / 2, height / 2] = root;

        //build north requires s
        //build south requires n
        //build west requires e
        //build east requires w

        /*
        //this will draw from top left to bottom right, going left to right
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                List<Module> list = new List<Module>(); //initialize list to avoid null/error

                if(x == 0)
                {
                    //x == 0 will mean that you cannot go north
                    if(y == 0) //north-west corner
                    {
                        
                        //appropriate values are s, e, se
                        rotations.TryGetValue("se", out list); //i chose se for testing purposes
                        break;
                    }

                    if(x == width) //north-east corner
                    {
                        //appropriate values are s, w, se
                        rotations.TryGetValue("se", out list); //i chose se for testing purposes
                        break;
                    }
                }

                if (x == 0 && y == 0)
                {
                    
                }



                matrix[x, y] = PickRandom(list); //pick a random index from chosen list
            }
        }
        */

        this.matrix = matrix;
    }

    private Module PickRandom(List<Module> list)
    {
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }

    public void AppendToModule(Direction side)
    {
        Vector2Int insertionIndex = new Vector2Int(-1, -1); //initialized to avoid null
        Vector2Int temp = FindModule(lastActive); //same as module

        switch (side)
        {
            //switch lastActive for parameter module?
            case Direction.North:
                if (!lastActive.north)
                {
                    Debug.LogError("No opening on to desired direction!");
                    return;
                }
                insertionIndex = new Vector2Int(temp.x - 1, temp.y); //above module
                break;
            case Direction.South:
                if (!lastActive.south)
                {
                    Debug.LogError("No opening on to desired direction!");
                    return;
                }
                insertionIndex = new Vector2Int(temp.x + 1, temp.y); //below module
                break;
            case Direction.West:
                if (!lastActive.west)
                {
                    Debug.LogError("No opening on to desired direction!");
                    return;
                }
                insertionIndex = new Vector2Int(temp.x, temp.y - 1); // left of module
                break;
            case Direction.East:
                if (!lastActive.east)
                {
                    Debug.LogError("No opening on to desired direction!");
                    return;
                }
                insertionIndex = new Vector2Int(temp.x, temp.y + 1); //right of module
                break;
            default:
                break;
        }

        Module moduleToAppend = LevelGenerator.instance.GetModulesWithDirection(side)[0];
        matrix[insertionIndex.x, insertionIndex.y] = moduleToAppend; //testing purposes. real is random
        lastActive = moduleToAppend;
    }

    public Vector2Int FindModule(Module module)
    {
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                if ((Module)matrix.GetValue(x, y) == module)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.Log("Didn't find module: " + module.name);
        return new Vector2Int(-1, -1);
    }

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();

        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                int atPos = 0;
                if (matrix.GetValue(x, y) != null) atPos = 1;
                result.Append(atPos);
            }
            result.Append("\n");
        }

        return result.ToString();
    }
}
