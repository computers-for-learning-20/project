using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameBehavior : MonoBehaviour
{
    // UI Panel Message Text
    public Text labelText;
    public Text instructions_eb;

    public Text WinOrLoseTitle;
    public Text WinOrLoseDetail;
    public GameObject WinOrLoseBox;

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

    //Panel Controls
    public GameObject earthPanel;

    // Game Progress Variables
    public bool progressLoaded = false;
    private string file = "Assets/Scripts/goals.txt";
    protected Dictionary<string, uint> GoalDict =
        new Dictionary<string, uint>();
    private string file = "Assets/Scripts/goals.txt";
    private List<string> particle_names
        = new List<string> { "Methane", "H2O", "O2",
            "N2", "Argon", "CO2"};
    private uint NextGoal;
    private string WinMessage;
    private string LoseMessage;
    

    void Start()
    {
        // Read in file lines to make progress dictionary
        string[] lines = File.ReadAllLines(@file);
        
        foreach (string line in lines)
        {
            string[] itemInfo = line.Split(' ');
            GoalDict.Add(itemInfo[0], uint.Parse(itemInfo[1]));
        }
        Debug.Log("game progress loaded");
        progressLoaded = true;

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
        if ( SceneManager.GetSceneByName("Lab").isLoaded)
        {
            // load items additive
        }
        else if (SceneManager.GetSceneByName("TimeTravelInterface").isLoaded)
        { WriteGoalProgress(); }
    }

    private void OnGUI()
    {
        // Load Inventory
        if (!SceneManager.GetSceneByName("TimeTravelInterface").isLoaded)
        {
            if (GoalDict["LastGoal"] < 2)
            {
                Button warp = GameObject.Find("InventoryTimeMachine")
                    .GetComponent<Button>();
                Text warpLabel = GameObject.Find("TMText")
                    .GetComponent<Text>();

                warp.interactable = false;
                warpLabel.color = new Color(0f, 0f, 0f, 1f);
            }

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
                else if (target02 == "air samples")
                { denom = target02_collected;}

                InventoryLabel.text = string.Format("{0}/{1}",
                    target02_collected, denom);
            }
            else
            {
                InventoryImage.enabled = false;
                InventoryLabel.text = "";
            }

            
            if (GoalDict["LastGoal"] >= 3 && earthPanel.activeSelf){
                foreach (string particle_name in particle_names)
                {
                    string object_name = string.Format("quantity_{0}",particle_name);
                    InventoryLabel = GameObject.Find(object_name)
                .GetComponent<Text>();
                    InventoryLabel.text = string.Format("{0}/1", GoalDict[particle_name]);
                }

            }



            // Creating win and lose screen buttons
            // Update this to change mission progress
            // Save and Update mission message
            if (winScreenShow)
            {
                WinOrLoseBox.SetActive(true);
                WinOrLoseTitle.text = ": : : :  SUCESS!  : : : :";
                WinOrLoseDetail.text = WinMessage;

                Button WinOrLoseButton = GameObject.Find("WinOrLose")
                    .GetComponent<Button>();

                GoalDict["LastGoal"] = NextGoal;
                health_spy = 100;
                WriteGoalProgress();
                GetMessageText();

                WinOrLoseButton.onClick.AddListener(
                    delegate {

                        winScreenShow = false;

                        if (GoalDict["LastGoal"] == 9)
                        { ResetGame(); }
                        else
                        { 
                            Time.timeScale = 1.0f;
                            WinOrLoseBox.SetActive(false);
                        }
                    });
            }

            if (loseScreenShow)
            {
                WinOrLoseBox.SetActive(true);
                WinOrLoseTitle.text = ": : : :  TRY AGAIN  : : : :";
                WinOrLoseDetail.text = LoseMessage;

                Button WinOrLoseButton = GameObject.Find("WinOrLose")
                    .GetComponent<Button>();

                WinOrLoseButton.onClick.AddListener(
                    delegate {

                        loseScreenShow = false;

                        if (GoalDict["LastGoal"] == 9)
                        { ResetGame(); }
                        else
                        {
                            RestartLevel();
                            WinOrLoseBox.SetActive(false);
                        }
                    });
            }
        }
    }

    private void CheckWinCondition()
    {
        // function for advancing the scene
	    GetMessageText();

        if (target01_collected == target01_goal
            && target02_collected == target02_goal)
        {
            winScreenShow = true;
            Time.timeScale = 0.0f;
            
        }
    }

    // will need to build scene refs in lab file
    private void RestartLevel()
    {
        // function for restarting the scene
        health_spy = 100;
        balance_earth = 50;
        WriteGoalProgress();

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        Time.timeScale = 1.0f;
    }

    // erase all game progress and start over
    private void ResetGame()
    {
        GoalDict["FirstEver"] = 1;
        GoalDict["BalanceEarth"] = 50;
        GoalDict["LastHealth"] = 100;
        GoalDict["CurrentPlace"] = 0;
        GoalDict["LastGoal"] = 0;
        GoalDict["TestTubes"] = 6;
        GoalDict["SolarPanels"] = 0;
        GoalDict["Batteries"] = 0;
        GoalDict["CO2"] = 0;
        GoalDict["Argon"] = 0;
        GoalDict["N2"] = 0;
        GoalDict["O2"] = 0;
        GoalDict["Methane"] = 0;
        GoalDict["Samples"] = 0;
        GoalDict["H2O"] = 0;
        WriteGoalProgress();
        SceneManager.LoadScene(0);
    }

    public void GoToTimeMachine()
    {
        if (GoalDict["LastGoal"] < 2)
        {
            labelText.text = "The time machine isn't working yet...";
        }
        else
        {
            WriteGoalProgress();
            SceneManager.LoadScene("TimeTravelInterface");
        }
    }

    // Attribute and Item Inventory Getter/Setters
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
            int change = (int) balance_earth - (int) value;
            balance_earth = value;
            if (balance_earth <= 0 | balance_earth >= 100)
            {
                if(GoalDict["LastGoal"] < 5){ 
                //Change balance earth model to be in less extreme ranges
                    if(change > 0){balance_earth  = 5;}
                    else{balance_earth=95;}
                }
                else{
                labelText.text = "Oh no, Earth Model failed!";
                loseScreenShow = true;
                Time.timeScale = 0.0f;
                }
            }
            else if(change> 0){labelText.text = "Earth getting warmer!";}
            else if(change < 0){labelText.text = "Earth getting colder!";}
            
            EarthBalanceSlider.value = balance_earth;
            EarthBalanceColor.color = EarthGradient
                .Evaluate(EarthBalanceSlider.normalizedValue);

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
                if (value > target01_goal)
                { labelText.text = "You don't need any more solar panels!"; }

                target01_collected = Math.Min(value, target01_goal);
                labelText.text = ItemCollectionMessages(2);

                if (labelText.text == "All done!")
                { CheckWinCondition(); }
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
                if (value > target02_goal)
                { labelText.text = "You don't need any more batteries!"; }

                target02_collected = Math.Min(value, target02_goal);
                labelText.text = ItemCollectionMessages(2);

                if (labelText.text == "All done!")
                { CheckWinCondition(); }
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
            GoalDict["H2O"] = value;

            if (target01 == "air samples")
            {
                target01_collected =
                    Math.Min(GoalDict["Samples"], target01_goal);
                target02_collected =
                    target01_goal - target01_collected;

                labelText.text = ItemCollectionMessages(1);
                if (labelText.text == "All done!")
                { CheckWinCondition(); }
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
                target01_collected =
                    Math.Min(GoalDict["Samples"], target01_goal);
                target02_collected =
                    target01_goal - target01_collected;

                labelText.text = ItemCollectionMessages(1);
                if (labelText.text == "All done!")
                { CheckWinCondition(); }
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
                target01_collected =
                    Math.Min(GoalDict["Samples"], target01_goal);
                target02_collected =
                    target01_goal - target01_collected;

                labelText.text = ItemCollectionMessages(1);
                if (labelText.text == "All done!")
                { CheckWinCondition(); }
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
                target01_collected =
                    Math.Min(GoalDict["Samples"], target01_goal);
                target02_collected =
                    target01_goal - target01_collected;

                labelText.text = ItemCollectionMessages(1);
                if (labelText.text == "All done!")
                { CheckWinCondition(); }
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
                target01_collected =
                    Math.Min(GoalDict["Samples"], target01_goal);
                target02_collected =
                    target01_goal -target01_collected;

                labelText.text = ItemCollectionMessages(1);
                if (labelText.text == "All done!")
                { CheckWinCondition(); }
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
                target01_collected =
                    Math.Min(GoalDict["Samples"], target01_goal);
                target02_collected =
                    target01_goal - target01_collected;

                labelText.text = ItemCollectionMessages(1);
                if (labelText.text == "All done!")
                { CheckWinCondition(); }
            }

        }
    }

    public uint CurrentPlace
    {
        get { return GoalDict["CurrentPlace"]; }
        set
        {
            GoalDict["CurrentPlace"] = value;
        }
    }



    // Save current progress for next scene load
    public void WriteGoalProgress()
    {
        // updating health and place
        GoalDict["LastHealth"] = health_spy;

        if (SceneManager.GetSceneByName("Lab").isLoaded ||
            SceneManager.GetSceneByName("outer_2100").isLoaded)
        { GoalDict["CurrentPlace"] = 0; }
        else if (SceneManager.GetSceneByName("ca2020").isLoaded)
        { GoalDict["CurrentPlace"] = 1; }

        // saving progress
        try
        {
            File.WriteAllLines(@file,
            GoalDict.Select(x => x.Key + " " + x.Value).ToArray());
            Debug.Log("Progress Saved.");
        }
        catch (System.Exception e)
        {
            Debug.Log(string.Format("{0} Exception caught.", e));
        }
    }

    // Get Correct Mission Message
    private void GetMessageText()
    {
        NextGoal = Math.Min(GoalDict["LastGoal"] + 1,
            9);

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

                WinMessage = "Mission Zero Complete! " +
                    "Good job! Rachel can fix the time machine now. " +
                    "Deliver the supplies you collected " +
                    "to her underground lab. Click to continue.";

                LoseMessage = "Oh no... you weren't able to get everything. " +
                    "Click to try again!";
                break;

            case (1):
                target01 = "solar panels";
                target01_collected = GoalDict["SolarPanels"];
                target01_goal = 2;
                target01_imageIdx = 0;

                target02 = "batteries";
                target02_collected = GoalDict["Batteries"];
                target02_goal = 3;
                target02_imageIdx = 1;

                labelText.text = "MISSION TASK: Go see Rachel " +
                    "and deliver those supplies!";

                WinMessage = "Start Mission One! Task 1 of 3: " +
                    "Use the time machine to travel to the past " +
                    "and collect air samples for analysis. Click to continue.";

                LoseMessage = "Are you having trouble finding Rachel? " +
                    "Look for a door into the mountainside. " +
                    "Click to try again.";
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
                    "MISSION TASK: Collect {0} {1} "
                    + "from the past. Avoid the fires!",
                    target01_goal, target01);

                WinMessage = "Mission One: Task 1 of 3 complete! " +
                    "Bring your samples back to Rachel to start " +
                    "task 2. Click to continue.";

                LoseMessage = "Oh no... you weren't able to get everything. " +
                    "Click to try again!";
                break;

            case (3):
                target01 = "air samples";
                target01_collected = GoalDict["Samples"];
                target01_goal = 6;
                target01_imageIdx = 3;

                target02 = "test tubes";
                target02_collected = 6 - target01_collected;
                target02_goal = 0;
                target02_imageIdx = 2;

                labelText.text = "MISSION TASK: Go back to "
                    + "Rachel with the samples.";

                WinMessage = "Start Mission One: Task 2 of 3: " +
                    "Find the earth atmoshere simulator in the lab " +
                    "and test your air samples.";

                LoseMessage = "Having trouble leaving the past? " +
                    "Use the WARP button in your inventory at the bottom-right. " +
                    "Click to try again!";
                break;

            case (4):
                // possible addition of image/inventory
                target01 = "lab puzzle";
                target01_collected = 0;
                target01_goal = 1;

                target02 = "none";
                target02_collected = 0;
                target02_goal = 0;

                labelText.text = "MISSION TASK: "
                    + "Talk with Rachel about "
                    + "the simulation results.";

                WinMessage = "Mission One: Task 2 of 3 Complete! " +
                    "Go tell Rachel what you observed!";

                LoseMessage = "Having trouble finding the earth simualtor? " +
                    "It is in the lab in 2100 near the time machine room." +
                    " Click to try again!";
                break;
            

            case (5):
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

                WinMessage = "Mission Two: Task 1 of 3 Complete! " +
                    "Go try your collected samples out in the simulator!";

                LoseMessage = "Oh no! You weren't able to get the samples." +
                    " Click to try again!";
                break;

            case (6):
                // possible image add
                target01 = "lab puzzle";
                target01_collected = 0;
                target01_goal = 1;

                target02 = "air samples";
                target02_collected = GoalDict["Samples"];
                target02_goal = 0;
                target02_imageIdx = 3;

                labelText.text = "MISSION TASK: "
                    + "Head back to the lab to test your samples!";

                WinMessage = "Mission Two: Task 2 of 3 Complete! " +
                    "Go tell Rachel what you learned!";

                LoseMessage = "Oh no! Earth can't get too hot or too " +
                    "cold. Watch what each kind of gas molecule does " +
                    "on the earth balance meter. " +
                    "Click to try again!";
                break;

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

                WinMessage = "Mission Two: Task 1 of 3 completed again! " +
                    "Go try your newly collected samples out in the simulator!";

                LoseMessage = "Oh no! Click to try again!";
                break;

            case (8):
                // possible addition of image/inventory
                target01 = "lab puzzle";
                target01_collected = 0;
                target01_goal = 1;

                target02 = "none";
                target02_collected = 0;
                target02_goal = 0;

                labelText.text = "MISSION TASK: "
                    + "Talk with Rachel about "
                    + "the simulation results.";

                WinMessage = "Mission Two: Task 3 of 3 Complete! " +
                    "Nice Job! You have completed the game demo!";

                LoseMessage = "Click to try again!";
                break;

            case (9):
                target01 = "none";
                target01_collected = 0;
                target01_goal = 0;

                target02 = "none";
                target02_collected = 0;
                target02_goal = 0;

                labelText.text = "CONGRATS! You finished the game demo!";
                WinMessage = "Click to reset the game.";
                LoseMessage = "Click to reset the game.";
                break;
        }

        if (SceneManager
            .GetSceneByName("TimeTravelInterface").isLoaded)
        {
            labelText.text = labelText.text.Replace("MISSION TASK:", "");
            
        }
    }

    private string ItemCollectionMessages(int targetCount)
    {
        string message = labelText.text;

        int target01_remaining = (int) target01_goal
            - (int) target01_collected;

        if (targetCount == 1)
        {
            if (target01_remaining > 1)
            {
                message = string.Format("Nice! Only {0} {1} left to get.",
                    target01_remaining, target01);
            }
            else if (target01_remaining == 1)
            {
                message = string.Format("Awesome! Get the last {0}!",
                    target01);
            }
            else if (target01_remaining == 0)
            {
                message = "All done!";
            }
        }
        else
        {
            int target02_remaining = (int)target02_goal
                - (int)target02_collected;

            if (target01_remaining > 0 && target02_remaining > 0)
            {
                message = string.Format(
                    "Nice! {0} {1} and {2} {3} left to get.",
                    target01_remaining, target01,
                    target02_remaining, target02);
            }
            else if (target01_remaining == 0 && target02_remaining > 0)
            {
                message = string.Format("All {0} collected!"
                    + " {1} {2} left to get.",
                    target01, target02_remaining, target02);
            }
            else if (target02_remaining == 0 && target01_remaining > 0)
            {
                message = string.Format("All {0} collected!"
                    + " {1} {2} left to get.",
                    target02, target01_remaining, target01);
            }
            else if (target01_remaining == 0 && target02_remaining == 0)
            {
                message = "All done!";
            }
        }
        return message;
    }
}