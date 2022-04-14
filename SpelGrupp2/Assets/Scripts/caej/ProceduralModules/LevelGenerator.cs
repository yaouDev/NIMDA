using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text; //only for debug


//kinda buggy
public class LevelGenerator : MonoBehaviour
{
    public List<Module> modules;

    public Dictionary<string, List<Module>> rotations;

    public static LevelGenerator instance;
    private void Awake()
    {
        instance ??= this;
    }

    public void InitializeModules()
    {
        //where is an opening?
        //n = north, s = south, w = west, e = east
        rotations = new Dictionary<string, List<Module>>();
        rotations.Add("nswe", new List<Module>());

        rotations.Add("nsw", new List<Module>());
        rotations.Add("nse", new List<Module>());
        rotations.Add("nwe", new List<Module>());
        rotations.Add("swe", new List<Module>());

        rotations.Add("ns", new List<Module>());
        rotations.Add("nw", new List<Module>());
        rotations.Add("ne", new List<Module>());
        rotations.Add("n", new List<Module>());

        rotations.Add("sw", new List<Module>());
        rotations.Add("se", new List<Module>());
        rotations.Add("s", new List<Module>());

        rotations.Add("we", new List<Module>());
        rotations.Add("w", new List<Module>());

        rotations.Add("e", new List<Module>());

        foreach (Module module in modules)
        {
            string rotation = "";

            if (module.north) rotation += "n";
            if (module.south) rotation += "s";
            if (module.west) rotation += "w";
            if (module.east) rotation += "e";

            if (rotation.Equals(""))
            {
                Debug.LogWarning(module.name + " has no openings!");
                return;
            }

            //find the key with the appropriate rotation list
            bool existsCheck = false;
            foreach (KeyValuePair<string, List<Module>> entry in rotations)
            {
                if (entry.Key.Equals(rotation))
                {
                    entry.Value.Add(module);
                    existsCheck = true; //ensures it was added somewhere
                }
            }
            if (!existsCheck) Debug.LogWarning(module.name + ": Tried to add a module but it didn't have anywhere to go :(");
        }

        foreach (KeyValuePair<string, List<Module>> entry in rotations)
        {
            if (entry.Value.Count > 0) Debug.Log(entry.Key.ToUpper() + " contains " + entry.Value.Count + " modules");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeModules();
        Level testLevel = new Level(9, 9);
        testLevel.AppendToModule(Direction.East);
        testLevel.AppendToModule(Direction.North);
        testLevel.AppendToModule(Direction.East);
        testLevel.AppendToModule(Direction.North);
        Debug.Log(testLevel);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Module> GetModulesWithDirection(Direction direction, Direction illegalDirection = Direction.NoExit)
    {
        if (direction == illegalDirection) Debug.LogError("Bruh.");

        List<Module> result = new List<Module>(); //initialized to avoid null

        switch (direction)
        {
            case Direction.North:
                //possible n, ne, ns, nw, nwe, nse, nwse
                rotations.TryGetValue("nswe", out result); //test!!
                break;
            case Direction.South:
                rotations.TryGetValue("nswe", out result); //test!!
                break;
            case Direction.West:
                rotations.TryGetValue("nswe", out result); //test!!
                break;
            case Direction.East:
                rotations.TryGetValue("nswe", out result); //test!!
                break;
            default:
                break;
        }

        return result;
    }

    private Module PickRandom(List<Module> list)
    {
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
}
public enum Direction
{
    North, South, West, East, NoExit
}
