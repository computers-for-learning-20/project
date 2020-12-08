using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameBehavior : MonoBehaviour
{
    // UI Panel Message Text
    public Text labelText;

    // Health Related Variables
    public Slider SpyHealthSlider;
    public Image SpyHealthColor;
    public Gradient SpyGradient;

    public Slider EarthBalanceSlider;
    public Image EarthBalanceColor;
    public Gradient EarthGradient;

    private uint health_spy = 100;
    private uint balance_earth = 50;
    private bool winScreenShow = false;
    private bool loseScreenShow = false;
    

    // Inventory Related Variables
    public string target01 = "solar panels";
    public string target02 = "batteries";

    private uint target01_collected = 0;
    private uint target02_collected = 0;
    private uint target01_goal = 0;
    private uint target02_goal = 0;

    private Text InventoryLabel;
    private Image InventoryImage;
    
    public Sprite[] spriteArray;
    private uint target01_imageIdx = 0;
    private uint target02_imageIdx = 1;

    // Game Progress Variables
    protected Dictionary<string, uint> GoalDict =
        new Dictionary<string, uint>();
    private string file = "Assets/Scripts/goals.txt";
    

    void Start()
    {
        // Read in file lines to make progress dictionary
        string[] lines = File.ReadAllLines(@file);

        foreach (string line in lines)
        {
            string[] itemInfo = line.Split(' ');

            Debug.Log(itemInfo[0]);
            Debug.Log(itemInfo[1]);

            GoalDict.Add(itemInfo[0], uint.Parse(itemInfo[1]));
        }

        // Set Prior Health Level
        health_spy = GoalDict["LastHealth"];
        SpyHealthSlider.value = health_spy;
        SpyHealthColor.color =
            SpyGradient.Evaluate(SpyHealthSlider.normalizedValue);
        
        EarthBalanceSlider.value = balance_earth;
        EarthBalanceColor.color =
            EarthGradient.Evaluate(EarthBalanceSlider.normalizedValue);

        // Set Mission Reminder Text
        GetMessageText();

        // Load Correct Scene Items
        if (SceneManager.GetSceneByName("ca2020").isLoaded)
        {
            if (GoalDict["LastGoal"] <= 2)
            {
                // load items additive
            }
            else { }
        }
        else if ( SceneManager.GetSceneByName("outer_2100").isLoaded)
        {

            if (GoalDict["LastGoal"] < 1)
            {
                // load items additive
            }
            else { }
        }
        else if ( SceneManager.GetSceneByName("Lab").isLoaded)
        {
            // load items additive
        }
        else if (SceneManager.GetSceneByName("TimeTravelInterface").isLoaded)
        {
            TimeMachineBehavior localManager =
                GameObject.Find("ButtonControls")
                .GetComponent<TimeMachineBehavior>();

            switch (GoalDict["CurrentPlace"])
            {
                case ( 0 ):
                    localManager.Now = "2100";
                    localManager.Here = "LAB";
                    break;
                case ( 1 ):
                    localManager.Now = "2020";
                    localManager.Here = "CA";
                    break;
            }
        }
    }

    private void OnGUI()
    {
        // Load Inventory
        if (!SceneManager.GetSceneByName("TimeTravelInterface").isLoaded)
        {


            InventoryImage = GameObject.Find("Inventory0Image")
            .GetComponent<Image>();
            InventoryLabel = GameObject.Find("Inventory0Text")
                    .GetComponent<Text>();

            InventoryImage.sprite = spriteArray[target01_imageIdx];

            if (target01_collected > 0)
            {
                InventoryImage.enabled = true;

                InventoryLabel.text = string.Format("{0}/{1}",
                    target01_collected, target01_goal);
            }
            else
            {
                InventoryImage.enabled = false;
                InventoryLabel.text = "";
            }

            InventoryImage = GameObject.Find("Inventory1Image")
                .GetComponent<Image>();
            InventoryLabel = GameObject.Find("Inventory1Text")
                .GetComponent<Text>();

            InventoryImage.sprite = spriteArray[target02_imageIdx];

            if (target02_collected > 0)
            {
                InventoryImage.enabled = true;

                uint denom = target02_goal;
                if (target02 == "test tubes")
                { denom = target01_goal; }

                InventoryLabel.text = string.Format("{0}/{1}",
                    target02_collected, denom);
            }
            else
            {
                InventoryImage.enabled = false;
                InventoryLabel.text = "";
            }

            // Creating win and lose screen buttons
            // Update this to change mission progress
            // Save and Update mission message
            if (winScreenShow)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 250,
                    Screen.height / 2 - 50, 500, 100),
                    "You Completed the Mission Task! (Click to Continue)"))
                {
                    GoalDict["LastGoal"] += 1;
                    GetMessageText();
                    RestartLevel();
                }
            }

            if (loseScreenShow)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 250,
                    Screen.height / 2 - 50, 500, 100),
                    "Oh no! You've lost! (Click to restart)"))
                {
                    RestartLevel();
                }
            }
        }
    }

    private void CheckWinCondition()
    {
        // function for advancing the scene
	GetMessageText();

        if (target01_collected >= target01_goal
            && target02_collected >= target02_goal)
        {
            winScreenShow = true;
            Time.timeScale = 0.0f;
        }
    }

    // will need to build scene refs in lab file
    private void RestartLevel()
    {
        // function for restarting the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1.0f;
    }

    public void GoToTimeMachine()
    {
        SceneManager.LoadScene("TimeTravelInterface");
    }

    // Attribute and Item Inventory Getter/Setters
    // Can we generalize better? We need to know
    // each molecule type separately for the simluator...
           
    public uint HealthSpy
    {
        get { return health_spy; }
        set
        {
            health_spy = value;
            SpyHealthSlider.value = health_spy;
            SpyHealthColor.color = SpyGradient
                .Evaluate(SpyHealthSlider.normalizedValue);
            if (health_spy <= 0)
            {
                labelText.text = "Oh no!";
                loseScreenShow = true;
                Time.timeScale = 0.0f;
            }
            else
            {
                labelText.text = "Ouch!";
            }
        }
    }

    public uint BalanceEarth
    {
        get { return balance_earth; }
        set
        {
            balance_earth = value;
            EarthBalanceSlider.value = balance_earth;
            EarthBalanceColor.color = EarthGradient
                .Evaluate(EarthBalanceSlider.normalizedValue);
            if (balance_earth <= 0 | balance_earth >= 100)
            {
                labelText.text = "Oh no, Earth model failed!";
                loseScreenShow = true;
                Time.timeScale = 0.0f;
            }
            else
            {
                labelText.text = "Careful with the Earth!";
            }
        }
    }

    public uint SolarPanels
    {
        get { return GoalDict["SolarPanels"]; }
        set
        {
            GoalDict["SolarPanels"] = value;

            if (target01 == "solar panels")
            {
                target01_collected = value;
                CheckWinCondition();
            }
        }
    }

    public uint Batteries
    {
        get { return GoalDict["Batteries"]; }
        set
        {
            GoalDict["Batteries"] = value;

            if (target02 == "batteries")
            {
                target02_collected = value;
                CheckWinCondition();
            }

        }
    }

    public uint H2O
    {
        get { return GoalDict["H2O"]; }
        set
        {
            uint priorValue = GoalDict["H2O"];
            GoalDict["Samples"] -= priorValue;
            GoalDict["Samples"] += value;
            GoalDict["H20"] = value;

            if (target01 == "air samples")
            {
                target01_collected = GoalDict["Samples"];
                CheckWinCondition();
            }

        }
    }

    public uint CO2
    {
        get { return GoalDict["CO2"]; }
        set
        {
            uint priorValue = GoalDict["CO2"];
            GoalDict["Samples"] -= priorValue;
            GoalDict["Samples"] += value;
            GoalDict["CO2"] = value;

            if (target01 == "air samples")
            {
                target01_collected = GoalDict["Samples"];
                CheckWinCondition();
            }

        }
    }

    public uint Argon
    {
        get { return GoalDict["Argon"]; }
        set
        {
            uint priorValue = GoalDict["Argon"];
            GoalDict["Samples"] -= priorValue;
            GoalDict["Samples"] += value;
            GoalDict["Argon"] = value;

            if (target01 == "air samples")
            {
                target01_collected = GoalDict["Samples"];
                CheckWinCondition();
            }

        }
    }

    public uint N2
    {
        get { return GoalDict["N2"]; }
        set
        {
            uint priorValue = GoalDict["N2"];
            GoalDict["Samples"] -= priorValue;
            GoalDict["Samples"] += value;
            GoalDict["N2"] = value;

            if (target01 == "air samples")
            {
                target01_collected = GoalDict["Samples"];
                CheckWinCondition();
            }

        }
    }

    public uint Methane
    {
        get { return GoalDict["Methane"]; }
        set
        {
            uint priorValue = GoalDict["Methane"];
            GoalDict["Samples"] -= priorValue;
            GoalDict["Samples"] += value;
            GoalDict["Methane"] = value;

            if (target01 == "air samples")
            {
                target01_collected = GoalDict["Samples"];
                CheckWinCondition();
            }

        }
    }

    public uint O2
    {
        get { return GoalDict["O2"]; }
        set
        {
            uint priorValue = GoalDict["O2"];
            GoalDict["Samples"] -= priorValue;
            GoalDict["Samples"] += value;
            GoalDict["O2"] = value;

            if (target01 == "air samples")
            {
                target01_collected = GoalDict["Samples"];
                CheckWinCondition();
            }

        }
    }

    // Get Correct Mission Message
    private void GetMessageText()
    {
        switch (GoalDict["LastGoal"])
        {
            case (0):
                target01 = "solar panels";
                target01_collected = GoalDict["SolarPanels"];
                target01_goal = 2;
                target01_imageIdx = 0;

                target02 = "batteries";
                target02_collected = GoalDict["Batteries"];
                target02_goal = 3;
                target02_imageIdx = 1;

                labelText.text = string.Format(
                "MISSION TASK: Find {0} {1} " +
                "and {2} {3}. Avoid the fires!",
                target01_goal, target01,
                target02_goal, target02);
                break;

            case (1):
                target01 = "see Rachel";
                target01_collected = 0;
                target01_goal = 1;

                target02 = "none";
                target02_collected = 0;
                target02_goal = 0;

                labelText.text = string.Format(
                "MISSION TASK: Go {0} " +
                "and deliver those supplies!",
                target01);
                break;

            case (2):
                target01 = "air samples";
                target01_collected = GoalDict["Samples"];
                target01_goal = 6;
                target01_imageIdx = 3;

                target02 = "test tubes";
                target02_collected = 6 - target01_collected;
                target02_goal = 0;
                target02_imageIdx = 2;

                labelText.text = string.Format(
                "MISSION TASK: Collect {0} {1} " +
                "from the past. Avoid the fires!",
                target01_goal, target01);
                break;

            case (3):
                target01 = "see Rachel";
                target01_collected = 0;
                target01_goal = 1;

                target02 = "none";
                target02_collected = 0;
                target02_goal = 0;

                labelText.text = string.Format(
                "MISSION TASK: Go {0} with the samples.", target01);
                break;

            case (4):
            case (8):
                target01 = "lab puzzle";
                target01_collected = 0;
                target01_goal = 1;

                target02 = "none";
                target02_collected = 0;
                target02_goal = 0;

                labelText.text = "MISSION TASK: "
                    + "Talk with Rachel about "
                    + "the simulation results.";
                break;

            case (5):
            case (7):
                target01 = "air samples";
                target01_collected = GoalDict["Samples"];
                target01_goal = 4;
                target01_imageIdx = 3;

                target02 = "test tubes";
                target02_collected = 4 - target01_collected;
                target02_goal = 0;
                target02_imageIdx = 2;

                labelText.text = string.Format(
                "MISSION TASK: Collect {0} {1} " +
                "from the past. Try for good balance!",
                target01_goal, target01);
                break;

            case (6):
                target01 = "lab puzzle";
                target01_collected = 0;
                target01_goal = 1;

                target02 = "none";
                target02_collected = 0;
                target02_goal = 0;

                labelText.text = "MISSION TASK: "
                    + "Head back to the lab to test your samples!";
                break;

            case (9):
                target01 = "none";
                target01_collected = 0;
                target01_goal = 0;

                target02 = "none";
                target02_collected = 0;
                target02_goal = 0;

                labelText.text = "CONGRATS: You finished the game demo!";
                break;
        }
    }
}