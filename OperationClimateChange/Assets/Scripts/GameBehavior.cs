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
    public GameObject instructionsPanel;

    // Game Progress Variables
    public bool progressLoaded = false;
    private string file = "Assets/Scripts/goals.txt";
    private Dictionary<string, uint> GoalDict =
        new Dictionary<string, uint>();
    
    private string WinMessage;
    private string LoseMessage;
    
    private List<string> particle_names
        = new List<string> { "Methane", "H2O", "O2",
            "N2", "Argon", "CO2"};
    private List<uint> levelsThatCountDown
        = new List<uint> {4, 6, 8};

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

        if(GoalDict["FirstEver"] == 1){
            instructionsPanel.SetActive(true);
        }
    }

    private void Update()
    {
        if (progressLoaded == true
            && !SceneManager.GetSceneByName("WarpSequence").isLoaded)
        {

            // Set Prior Health Level
            health_spy = GoalDict["LastHealth"];
            SpyHealthSlider.value = (int)health_spy; // this line is causing an error
            SpyHealthColor.color =
                SpyGradient.Evaluate(SpyHealthSlider.normalizedValue);

            EarthBalanceSlider.value = balance_earth;
            EarthBalanceColor.color =
                EarthGradient.Evaluate(EarthBalanceSlider.normalizedValue);

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
            EarthGradient.Evaluate(EarthBalanceSlider.normalizedValue);
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

            if (GoalDict["LastGoal"] < 2)
            {
                warp.interactable = false;
                warpLabel.color = new Color(0f, 0f, 0f, 1f);
            }
            else
            { 
                warp.interactable = true;
                warpLabel.color = new Color(100f, 227f, 255f, 1f);
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

            // Load Correct Scene Items
            if (SceneManager.GetSceneByName("Lab").isLoaded)
            {
                if (GoalDict["LastGoal"] >= 3 && earthPanel.activeSelf)
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

    public void CheckWinCondition()
    {
        // function for advancing the scene
        if (target01_collected == target01_goal
            && target02_collected == target02_goal)
        {
            Debug.Log("Show Win Screen");
            if (GoalDict["LastGoal"] == 6)
            { GoalDict["LastGoal"] = GoalDict["LastGoal"] + 2; }
            else if (GoalDict["LastGoal"] == 7)
            { GoalDict["LastGoal"] = 6; }
            else { GoalDict["LastGoal"] = GoalDict["LastGoal"] + 1; }

            WriteGoalProgress();

            GoalDict["FirstEver"] = 1;
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
        GoalDict["FirstEver"] = 0;
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

    public void ForceGoalClear()
    {
        target01_collected = 0;
        target01_goal = 0;
        target02_collected = 0;
        target02_goal = 0;
        CheckWinCondition();
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
            else if(change > 0){labelText.text = "Earth getting warmer!";}
            else if(change < 0){labelText.text = "Earth getting colder!";}
            
            EarthBalanceSlider.value = balance_earth;
            EarthBalanceColor.color = EarthGradient
                .Evaluate(EarthBalanceSlider.normalizedValue);

        }
    }

    public uint ProgressPoint
    { get { return GoalDict["LastGoal"]; } }

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

            GoalDict["H2O"] = value;
            GoalDict["Samples"] = SumOfGasses();
            target01_collected = SumOfGasses();
            CheckWinCondition();
        }
    }

    public uint CO2
    {
        get { return GoalDict["CO2"]; }
        set
        {
            GoalDict["CO2"] = value;
            GoalDict["Samples"] = SumOfGasses();
            target01_collected = SumOfGasses();
            CheckWinCondition();

        }
    }

    public uint Argon
    {
        get { return GoalDict["Argon"]; }
        set
        {
            GoalDict["Argon"] = value;
            GoalDict["Samples"] = SumOfGasses();
            target01_collected = SumOfGasses();
            CheckWinCondition();

        }
    }

    public uint N2
    {
        get { return GoalDict["N2"]; }
        set
        {
            GoalDict["N2"] = value;
            GoalDict["Samples"] = SumOfGasses();
            target01_collected = SumOfGasses();
            CheckWinCondition();

        }
    }

    public uint Methane
    {
        get { return GoalDict["Methane"]; }
        set
        {
            GoalDict["Methane"] = value;
            GoalDict["Samples"] = SumOfGasses();
            target01_collected = SumOfGasses(); 
            CheckWinCondition();
        }
    }


    public uint O2
    {
        get { return GoalDict["O2"]; }
        set
        {
            GoalDict["O2"] = value;
            GoalDict["Samples"] = SumOfGasses();
            target01_collected = SumOfGasses();
            CheckWinCondition();

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

    public uint FirstEver
    {
        get { return GoalDict["FirstEver"]; }
        set
        {
            GoalDict["FirstEver"] = value;
        }
    }

    // function for getting a total of collected samples.
    private uint SumOfGasses()
    {
        return (GoalDict["Methane"] +
            GoalDict["CO2"] +
            GoalDict["H2O"] +
            GoalDict["Argon"] +
            GoalDict["O2"] +
            GoalDict["N2"]);
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

        Debug.Log(string.Format("The goal flag is now {0}",
            GoalDict["LastGoal"]));

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
                target01_goal = 6;
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
                    + "Talk with Rachel about "
                    + "the simulation results.";

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
                winScreenShow = true;
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

    
    private void get_instructionsMessages(){
        string instruction = "";

        switch (GoalDict["LastGoal"])
        {
            case (0):
                string place = whereIsTheSpy();
                instruction = string.Format("Welcome spy! You are in the {0}", place);
                instruction += " Remember: To move use the arrow keys."+
                    " If you wan to go faster, use the R key."+
                    " To close this windown, just press the (?) button."+
                    " The required tasks for any mission appear at the lower bar. But no worries,"+
                    "I'm also here for any help you need."+
                    "For this mission, find 2 solar panels and 3 baterries similar to the pictures below.";
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
                    instruction = "You are in California of 2020. Some parts are severly damaged by the extreme wildfires."+
                    " Other parts of California still show healthy signs."+
                    " Please get 6 air samples from there."+
                    "Each of them will look like the following particles below.";
                    GameObject particle_parent =GameObject.Find("particle_parent");
                    particle_parent.transform.Find("particle_items").gameObject.SetActive(true);
                }
                break;

            case (3):
                break;

            case (4):
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
                    "the tempurature balances!";
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
                    "but there is lots of adventures to come!";
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