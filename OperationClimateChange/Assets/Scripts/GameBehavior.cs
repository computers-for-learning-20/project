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
    private int solar_panels = 0;
    private int batteries = 0;
    private int TestTubes = 0;
    private int methane = 0;
    private int h2o = 0;
    private int co2 = 0;
    private int argon = 0;
    private int o2 = 0;
    private int n2 = 0;
    private uint currentplace = 0;

    private int target01_goal = 0;
    private int target02_goal = 0;
    private int target01_collected = 0;
    private int target02_collected = 0;
    private int lastGoal;

    private Text InventoryLabel;
    private Image InventoryImage0;
    private Text InventoryLabel0;
    private Image InventoryImage1;
    private Text InventoryLabel1;

    public Sprite[] spriteArray;
    public string target01 = "solar panels";
    public string target02 = "batteries";
    private uint target01_imageIdx = 0;
    private uint target02_imageIdx = 1;

    //Panel Controls
    public GameObject earthPanel;
    public GameObject instructionsPanel;
    public uint FirstEver = 1;

    // Game Progress Variables
    public bool progressLoaded = false;
    private string file = "Assets/Scripts/goals.txt";
    private Dictionary<string, uint> GoalDict =
        new Dictionary<string, uint>();
    
    private string WinMessage;
    private string LoseMessage;
    private int count = 0;

    private List<string> particle_names
        = new List<string> { "Methane", "H2O", "O2",
            "N2", "Argon", "CO2"};

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

        if (FirstEver == 1)
        { instructionsPanel.SetActive(true); }

        // Load Correct Scene Items
        if (SceneManager.GetSceneByName("Lab").isLoaded)
        {
            if (lastGoal >= 3 && earthPanel.activeSelf)
            {
                foreach (string particle_name in particle_names)
                {
                    string object_name = string.Format("quantity_{0}"
                        , particle_name);
                    InventoryLabel = GameObject.Find(object_name)
                .GetComponent<Text>();
                    InventoryLabel.text = string.Format("{0}/1",
                        GoalDict[particle_name]);
                }
            }

        }

    }
    
    private void Update()
    {

        if (progressLoaded == true
            && !SceneManager.GetSceneByName("WarpSequence").isLoaded
            && FirstEver == 1)
        {

            // Set Prior Health Level
            health_spy = GoalDict["LastHealth"];
            SpyHealthSlider.value = (int)health_spy;
            SpyHealthColor.color =
                SpyGradient.Evaluate(SpyHealthSlider.normalizedValue);

            EarthBalanceSlider.value = balance_earth;
            EarthBalanceColor.color =
                EarthGradient.Evaluate(EarthBalanceSlider.normalizedValue);

            solar_panels = (int)GoalDict["SolarPanels"];
            lastGoal = (int) GoalDict["LastGoal"];
            TestTubes = (int)GoalDict["TestTubes"];
            solar_panels = (int) GoalDict["SolarPanels"];
            batteries = (int) GoalDict["Batteries"];
            co2 = (int) GoalDict["CO2"];
            Argon = (int) GoalDict["Argon"];
            n2 = (int) GoalDict["N2"];
            o2 = (int) GoalDict["O2"];
            methane = (int) GoalDict["Methane"];
            h2o = (int) GoalDict["H2O"];

            // Set Mission Reminder Text
            GetMessageText();

            // Save before time warp
            if (SceneManager.GetSceneByName("TimeTravelInterface").isLoaded)
            { WriteGoalProgress(); }

            progressLoaded = false;
        }

        if (SceneManager.GetSceneByName("Lab").isLoaded
            && earthPanel.activeSelf)
        {
            EarthBalanceSlider.value = this.BalanceEarth;
            EarthBalanceColor.color =
                EarthGradient.Evaluate(EarthBalanceSlider
                .normalizedValue);
        }

    }

    private void OnGUI()
    {
        // Load Inventory
        if (!SceneManager.GetSceneByName("TimeTravelInterface").isLoaded)
        {
            Button warp = GameObject.Find("InventoryTimeMachine")
                    .GetComponent<Button>();
            Text warpLabel = GameObject.Find("TMText")
                .GetComponent<Text>();

            if (lastGoal < 2)
            {
                warp.interactable = false;
                warpLabel.color = new Color(0f, 0f, 0f, 1f);
            }
            else
            { 
                warp.interactable = true;
                warpLabel.color = new Color(100f, 227f, 255f, 1f);
            }

            InventoryImage0 = GameObject.Find("Inventory0Image")
            .GetComponent<Image>();
            InventoryLabel0 = GameObject.Find("Inventory0Text")
                    .GetComponent<Text>();

            InventoryImage0.sprite = spriteArray[target01_imageIdx];

            if (target01_collected > 0)
            {
                InventoryImage0.enabled = true;

                InventoryLabel0.text = string.Format("{0}/{1}",
                    target01_collected, target01_goal);
            }
            else
            {
                InventoryImage0.enabled = false;
                InventoryLabel0.text = "";
            }

            InventoryImage1 = GameObject.Find("Inventory1Image")
                .GetComponent<Image>();
            InventoryLabel1 = GameObject.Find("Inventory1Text")
                .GetComponent<Text>();

            InventoryImage1.sprite = spriteArray[target02_imageIdx];

            if (target02_collected > 0)
            {
                InventoryImage1.enabled = true;

                int denom = target02_goal;
                if (target02 == "test tubes")
                { denom = target01_goal; }
                else if (target02 == "air samples")
                { denom = target02_collected;}

                InventoryLabel1.text = string.Format("{0}/{1}",
                    target02_collected, denom);
            }
            else
            {
                InventoryImage1.enabled = false;
                InventoryLabel1.text = "";
            }


            if(instructionsPanel.activeSelf){
            get_instructionsMessages();}

            // Creating win and lose screen buttons
            // Update this to change mission progress
            // Save and Update mission message
            if (winScreenShow)
            {

                WinOrLoseBox.SetActive(true);
                WinOrLoseTitle.text = ": : : :  SUCCESS!  : : : :";
                WinOrLoseDetail.text = WinMessage;

                Button WinOrLoseButton = GameObject.Find("WinOrLose")
                    .GetComponent<Button>();

                health_spy = 100;

                WinOrLoseButton.onClick.AddListener(
                    delegate {

                        winScreenShow = false;

                        if (count == 0)
                        {
                            if (lastGoal == 9)
                            { ResetGame(); }
                            else
                            {
                                Time.timeScale = 1.0f;
                                WinOrLoseBox.SetActive(false);
                                WriteGoalProgress();
                                SetNextGoal();
                                GetMessageText();
                                count += 1;
                            }
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

                        if (lastGoal == 9)
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

    public void CheckWinCondition()
    {
        Debug.Log("Show Win Screen");
        count = 0;
        winScreenShow = true;
        Time.timeScale = 0.0f;

    }

    public void SetNextGoal()
    {
        // function for advancing the goal
        switch (lastGoal)
        {
            case (0):
            case (1):
            case (2):
            case (3):
            case (4):
            case (5):
            case (8):
                lastGoal += 1;
                break;
            case (6):
                lastGoal = 8;
                break;
            case (7):
                lastGoal = 6;
                break;
            default:
                lastGoal = 9;
                break;
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
        GoalDict["FirstEver"] = 0;
    }

    // erase all game progress and start over
    private void ResetGame()
    {
        FirstEver = 1;
        balance_earth = 50;
        health_spy = 100;
        currentplace = 0;
        lastGoal = 0;
        TestTubes = 6;
        solar_panels = 0;
        batteries = 0;
        co2 = 0;
        argon = 0;
        n2 = 0;
        o2 = 0;
        methane = 0;
        h2o = 0;
        WriteGoalProgress();
        SceneManager.LoadScene(0);
    }

    public void GoToTimeMachine()
    {
        if (lastGoal < 2)
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
                if(lastGoal < 5){ 
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
            else if(change > 0){labelText.text = "Earth getting warmer!";}
            else if(change < 0){labelText.text = "Earth getting colder!";}
            
            EarthBalanceSlider.value = balance_earth;
            EarthBalanceColor.color = EarthGradient
                .Evaluate(EarthBalanceSlider.normalizedValue);

        }
    }

    public int ProgressPoint
    { get { return lastGoal; } }

    public int SolarPanels
    {
        get { return solar_panels; }
        set
        {
            solar_panels = value;

            if (target01 == "solar panels")
            {
                target01_collected = value;
                labelText.text = ItemCollectionMessages(2);
             }
        }
    }

    public int Batteries
    {
        get { return batteries; }
        set
        {
            batteries = value;

            if (target02 == "batteries")
            {
                target02_collected = value;
                labelText.text = ItemCollectionMessages(2);
            }

        }
    }

    public int H2O
    {
        get { return h2o; }
        set
        {
            h2o = value;

            if (target01 == "air samples")
            { 
                target01_collected = SumOfGasses();
                target02_collected = target01_goal
                    - target01_collected;
                labelText.text = ItemCollectionMessages(1);

                if (lastGoal > 3 && earthPanel.activeSelf)
                {
                    InventoryLabel = GameObject.Find("quantity_H2O")
                        .GetComponent<Text>();

                    InventoryLabel.text = string.Format("{0}/1",
                            value);
                }
            }
        }
    }

    public int CO2
    {
        get { return co2; }
        set
        {
            co2 = value;

            if (target01 == "air samples")
            {
                target01_collected = SumOfGasses();
                target02_collected = target01_goal
                    - target01_collected;
                labelText.text = ItemCollectionMessages(1);
                if (lastGoal > 3 && earthPanel.activeSelf)
                {
                    InventoryLabel = GameObject.Find("quantity_CO2")
                        .GetComponent<Text>();

                    InventoryLabel.text = string.Format("{0}/1",
                            value);
                }
            }

        }
    }

    public int Argon
    {
        get { return argon; }
        set
        {
            argon = value;

            if (target01 == "air samples")
            {
                target01_collected = SumOfGasses();
                target02_collected = target01_goal
                    - target01_collected;
                labelText.text = ItemCollectionMessages(1);

                if (lastGoal > 3 && earthPanel.activeSelf)
                {
                    InventoryLabel = GameObject.Find("quantity_Argon")
                        .GetComponent<Text>();

                    InventoryLabel.text = string.Format("{0}/1",
                            value);
                }
            }

        }
    }

    public int N2
    {
        get { return n2; }
        set
        {
            n2 = value;

            if (target01 == "air samples")
            {
                target01_collected = SumOfGasses();
                target02_collected = target01_goal
                    - target01_collected;
                labelText.text = ItemCollectionMessages(1);

                if (lastGoal > 3 && earthPanel.activeSelf)
                {
                    InventoryLabel = GameObject.Find("quantity_N2")
                        .GetComponent<Text>();

                    InventoryLabel.text = string.Format("{0}/1",
                            value);
                }
            }

        }
    }

    public int Methane
    {
        get { return methane; }
        set
        {
            methane = value;

            if(target01 == "air samples")
            {
                target01_collected = SumOfGasses();
                target02_collected = target01_goal
                    - target01_collected;
                labelText.text = ItemCollectionMessages(1);
                if (lastGoal > 3 && earthPanel.activeSelf)
                {
                    InventoryLabel = GameObject.Find("quantity_Methane")
                        .GetComponent<Text>();

                    InventoryLabel.text = string.Format("{0}/1",
                            value);
                }
            }
        }
    }

    public int O2
    {
        get { return o2; }
        set
        {
            o2 = value;

            if (target01 == "air samples")
            {
                target01_collected = SumOfGasses();
                target02_collected = target01_goal
                    - target01_collected;
                labelText.text = ItemCollectionMessages(1);

                if (lastGoal > 3 && earthPanel.activeSelf)
                {
                    InventoryLabel = GameObject.Find("quantity_O2")
                        .GetComponent<Text>();

                    InventoryLabel.text = string.Format("{0}/1",
                            value);
                }
            }
        }
    }

    public uint CurrentPlace
    {
        get { return currentplace; }
        set
        {
            currentplace = value;
        }
    }

    // function for getting a total of collected samples.
    public int SumOfGasses()
    {
        return methane + co2 + h2o + argon + o2 + n2;
    }

    // Save current progress for next scene load
    public void WriteGoalProgress()
    {
        // updating health and place
        GoalDict["LastHealth"] = health_spy;

        if (SceneManager.GetSceneByName("Lab").isLoaded ||
            SceneManager.GetSceneByName("outer_2100").isLoaded)
        { currentplace = 0; }
        else if (SceneManager.GetSceneByName("ca2020").isLoaded)
        { currentplace = 1; }

        GoalDict["CurrentPlace"] = currentplace;
        GoalDict["FirstEver"] = FirstEver;
        GoalDict["BalanceEarth"] = balance_earth;
        GoalDict["LastGoal"] = (uint) lastGoal;
        GoalDict["TestTubes"] = (uint) TestTubes;
        GoalDict["SolarPanels"] = (uint) solar_panels;
        GoalDict["Batteries"] = (uint) batteries;
        GoalDict["CO2"] = (uint) co2;
        GoalDict["Argon"] = (uint) Argon;
        GoalDict["N2"] = (uint) n2;
        GoalDict["O2"] = (uint) o2;
        GoalDict["Methane"] = (uint) methane;
        GoalDict["H2O"] = (uint)h2o;
        GoalDict["Samples"] = (uint) SumOfGasses();

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

        Debug.Log(string.Format("The goal flag is now {0}",
            lastGoal));

        switch (lastGoal)
        {
            case (0):
                target01 = "solar panels";
                target01_collected = solar_panels;
                target01_goal = 2;
                target01_imageIdx = 0;

                target02 = "batteries";
                target02_collected = Batteries;
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
                target01_collected = SolarPanels;
                target01_goal = 2;
                target01_imageIdx = 0;

                target02 = "batteries";
                target02_collected = Batteries;
                target02_goal = 3;
                target02_imageIdx = 1;

                labelText.text = "MISSION TASK: Go see Rachel " +
                    "and deliver those supplies!";

                WinMessage = "Start Mission One! Task 1 of 3: " +
                    "Use the time machine to travel to the past " +
                    "and collect air samples for analysis. Look for " +
                    "the stairs to the glowing room in the lab. " +
                    "Click to continue.";

                LoseMessage = "Are you having trouble finding Rachel? " +
                    "Look for a door into the mountainside. " +
                    "Click to try again.";
                break;

            case (2):
                target01 = "air samples";
                target01_collected = SumOfGasses();
                target01_goal = 6;
                target01_imageIdx = 3;

                target02 = "test tubes";
                target02_collected = 6 - target01_collected;
                target02_goal = 0;
                target02_imageIdx = 2;

                labelText.text = string.Format(
                    "MISSION TASK: Collect {0} {1} "
                    + " from the past. Avoid the fires!",
                    target01_goal, target01);

                WinMessage = "Mission One: Task 1 of 3 complete! " +
                    "Bring your samples back to Rachel. " +
                    " Click to continue.";

                LoseMessage = "Oh no... you weren't able to get everything. " +
                    "Click to try again!";
                break;

            case (3):
                target01 = "air samples";
                target01_collected = SumOfGasses();
                target01_goal = 0;
                target01_imageIdx = 3;

                target02 = "talk to Rachel";
                target02_collected = 0;
                target02_goal = 1;

                labelText.text = "MISSION TASK: Go back to "
                    + "Rachel with the samples.";

                WinMessage = "Start Mission One: Task 2 of 3: " +
                    "Find the earth atmoshere simulator in the lab " +
                    "and test your air samples.";

                LoseMessage = "Having trouble leaving the past? " +
                    "Use the WARP button in your inventory at the " +
                    "bottom-right. " +
                    "Click to try again!";
                break;

            case (4):
                target01 = "air samples";
                target01_collected = SumOfGasses();
                target01_goal = 0;
                target01_imageIdx = 3;

                target02 = "test tubes";
                target02_collected = 0;
                target02_goal = 0;
                target02_imageIdx = 2;

                labelText.text = "MISSION TASK: "
                    + "Experiement with the earth balance simulator.";

                WinMessage = "Mission One: Task 3 of 3 Complete! " +
                    "Talk to Rachel about what to do next. " +
                    "We need to figure out which particles do what.";

                LoseMessage = "Having trouble finding the earth simualtor? " +
                    "It is in the lab in 2100 near the time machine room." +
                    " Click to try again!";
                break;
            

            case (5):
                target01 = "air samples";
                target01_collected = SumOfGasses();
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
                target01 = "air samples";
                target01_collected = SumOfGasses();
                target01_goal = 0;
                target01_imageIdx = 3;

                target02 = "test tubes";
                target02_collected = 0;
                target02_goal = 0;
                target02_imageIdx = 2;

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
                target01_collected = SumOfGasses();
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
                CheckWinCondition();
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

        int target01_remaining = target01_goal
            - target01_collected;

        if (targetCount == 1)
        {
            if ((lastGoal == 6 || lastGoal == 3)
                && SumOfGasses() == 0)
            { CheckWinCondition(); }
            else if (target01_remaining > 1)
            {
                message = string.Format("Nice! Only {0} {1} left to get.",
                    target01_remaining, target01);
            }
            else if (target01_remaining == 1)
            {
                message = string.Format("Awesome! Get the last {0}!",
                    target01);
            }
            else if (target01_remaining <= 0 &&
                (lastGoal == 2 || lastGoal == 5 || lastGoal == 7))
            {
                target01_collected = target01_goal;
                message = "All done!";
                CheckWinCondition();
            }
     
        }
        else
        {
            int target02_remaining = target02_goal
                - target02_collected;

            if (target01_remaining > 0 && target02_remaining > 0)
            {
                message = string.Format(
                    "Nice! {0} {1} and {2} {3} left to get.",
                    target01_remaining, target01,
                    target02_remaining, target02);
            }
            else if (target01_remaining <= 0 && target02_remaining > 0)
            {
                message = string.Format("All {0} collected!"
                    + " {1} {2} left to get.",
                    target01, target02_remaining, target02);
            }
            else if (target02_remaining <= 0 && target01_remaining > 0)
            {
                message = string.Format("All {0} collected!"
                    + " {1} {2} left to get.",
                    target02, target01_remaining, target01);
            }
            else if (target01_remaining <= 0 && target02_remaining <= 0)
            {
                target01_collected = target01_goal;
                target02_collected = target02_goal;
                message = "All done!";
                CheckWinCondition();
            }
        }
        return message;
    }

    
    private void get_instructionsMessages(){
        string instruction = "";

        switch (lastGoal)
        {
            case (0):
                string place = whereIsTheSpy();
                instruction = string.Format("Welcome spy! You are in the {0}", place);
                instruction += " Remember: To move use the arrow keys."+
                    " If you wan to go faster, use the R key."+
                    " To close this window, just press the (?) button."+
                    " The required tasks for any mission appear at the lower bar. But no worries,"+
                    "I'm also here for any help you need."+
                    "For this mission, find 2 solar panels and 3 batteries similar to the pictures below.";
                foreach (string item in new List<string> { "solar_panel_icon", "battery_icon"})
                {
                    Image img = GameObject.Find(item).GetComponent<Image>();
                    img.enabled = true;
                }
                break;

            case (1):
                if(SceneManager.GetSceneByName("Lab").isLoaded){
                    instruction="Great! You are in the lab. Now go to the office area, and meet me with the supplies";
                }else{
                instruction = "You are doing great! Now go to the lab."+
                "And meet me in the office with the supplies..."+
                "Look for a door into the mountainside to get into the lab.";}
                foreach (string item in new List<string> { "solar_panel_icon", "battery_icon" })
                {
                    Image img = GameObject.Find(item).GetComponent<Image>();
                    img.enabled = false;
                }
                break;

            case (2):
                if(SceneManager.GetSceneByName("Lab").isLoaded){
                instruction = "Hi Spy! Now that the time-machine works, your next task is to use "+
                        "it to travel to California of 2020. I have activated the time-machine button."+
                        "Just click in the WARP area."; 
                Image img = GameObject.Find("time_machine_icon").GetComponent<Image>();
                img.enabled = true;}
                else if (SceneManager.GetSceneByName("TimeTravelInterface").isLoaded){
                instruction = "In the Where button choose California, and in the When button choose 2020"+
                    "The mission is to get 6 air samples from there. Be careful with the fires! Save travels friend!";
                Image img = GameObject.Find("time_machine_icon").GetComponent<Image>();
                img.enabled = false;}
                else if(SceneManager.GetSceneByName("ca2020").isLoaded){
                    instruction = "You are in California of 2020. Some parts are severely damaged by the extreme wildfires."+
                    " Other parts of California still show healthy signs."+
                    " Please get 6 air samples from there."+
                    "Each of them will look like the following particles below.";
                    GameObject particle_parent = GameObject.Find("particle_parent");
                    particle_parent.transform.Find("particle_items").gameObject.SetActive(true);
                }
                break;

            case (3):
                instruction = "Great! Come see me with your samples!";
                break;

            case (4):
                instruction = "Use the simulated earth model in the lab " +
                    "to see what would happen if there was a high "+"" +
                    "concentration of a particular type of gas " +
                    "in the atmosphere.";
                break;
            

            case (5):
                instruction = "Okay! Can you figure out which atmospheric " +
                    "gasses make the earth hotter? I know! " +
                    "Grab some new samples from the past and " +
                    "try to avoid molecules that make earth too hot.";
                break;

            case (6):
                instruction = "Nice work so far! " +
                    "Go test your samples and tell me if " +
                    "the temperature balances!";
                break;

            case (7):
                instruction = "You'll have to go back the the past and " +
                    "try sample collection again... the balance " +
                    "with the samples you collected didn't create very " +
                    "livable conditions on earth.";
                break;

            case (8):
                instruction = "Come find me in the lab to tell me what " +
                    "you have learned about atmospheric gasses and " +
                    "earth's temperature!";
                break;

            case (9):
                instruction = "Fascinating! I wonder what caused " +
                    " there to be so much carbon dioxide and methane " +
                    "in our atmosphere now days..." +
                    "Great job beating the demo game! That's all for now, " +
                    "but there is lots of adventure to come!";
                break;
        }
        Text instruction_component = GameObject.Find("instructions")
                    .GetComponent<Text>();
        instruction_component.text = instruction;

    }

    private string whereIsTheSpy(){
        if(SceneManager.GetSceneByName("Lab").isLoaded){
            return "Laboratory";
        }
        else if(SceneManager.GetSceneByName("outer_2100").isLoaded){
            return "Outer environment";
        }
        else if(SceneManager.GetSceneByName("ca2020").isLoaded){
            return "California 2020";
        }
        else{
            return "Time-travel Machine";
        }
    }
}