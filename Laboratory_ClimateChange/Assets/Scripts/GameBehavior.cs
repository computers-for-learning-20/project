using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameBehavior : MonoBehaviour
{
    public HealthBar healthBar;
    public uint maxhealth = 100;
    private uint health_spy = 100;
    public Text labelText;
    public int solar_panels = 2;
    public int batteries = 3;

    public Slider SpyHealth;
    public Image SpyHealthColor;

    public Color criticalColor;
    public Color warningColor;
    public Color okayColor;

    private bool winScreenShow = false;
    private bool loseScreenShow = false;
    private uint solar_panel_collected = 0;
    private uint batteries_collected = 0;
    private string target01 = "solar panels";
    private string target02 = "batteries";

    private Text InventoryLabel;
    private Image InventoryImage;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "ca2020" ||
            SceneManager.GetActiveScene().name == "outer_2100")
        {
            target01 = "solar panels";
            target02 = "batteries";
        }

        labelText.text = string.Format("MISSION TASK :: Find {0} {1} " +
            "and {2} {3}. Avoid the fires", solar_panels, target01,
            batteries, target02);
    }


    void Start(){
        healthBar.SetMaxHealth(maxhealth);
    }
    public uint HealthSpy
    {
        get { return health_spy; }
        set
        {
            health_spy = value;
            healthBar.SetHealth(health_spy);
            if (health_spy <= 0)
            {
                labelText.text = "Oh no!";
                WinOrLoseScreen("lose");
            }
            else
            {
                labelText.text = "Ouch!";
            }
        }
    }

    public uint SolarPanels
    {
        get { return solar_panel_collected; }
        set
        {
            solar_panel_collected = value;

            if (solar_panel_collected == solar_panels)
            {
                if (batteries_collected == batteries) {
                    labelText.text = "You got all the missing pieces of the "
                            + "time-travel machine";
                    WinOrLoseScreen("win"); }
                else {
                    labelText.text = string.Format("Nice! You got all " +
                        "the solar panels!" +
                        "Only {0} batteries left to find!",
                        batteries - batteries_collected);
                }
            }
            else
            {
                if (batteries_collected == batteries) {
                    labelText.text = "Only 1 solar panel left to find!"; }
                else {
                    labelText.text = string.Format("Nice! 1 solar panel "
                        + "and {0} batteries left to find!",
                        batteries - batteries_collected); }
            }

        }
    }

    public uint Batteries
    {
        get { return batteries_collected; }
        set
        {
            batteries_collected = value;

            if (batteries_collected == batteries)
            {
                if (solar_panel_collected == solar_panels) {
                    labelText.text = "You got all the missing pieces"
                            + " of the time-travel machine";
                    WinOrLoseScreen("win"); }
                else {
                    labelText.text = string.Format("Nice! You got all"
                        + "the batteries! Only {0} solar panels left to find!",
                        solar_panels - solar_panel_collected);
                }
            }
            else
            {
                if (solar_panel_collected == solar_panels) {
                    labelText.text = string.Format("Only {0} batteries "
                        + "left to find!", batteries - batteries_collected); }
                else {
                    labelText.text = string.Format("Nice! {0} batteries and"
                        + " {1} solar panels left to find!",
                        batteries - batteries_collected,
                        solar_panels - solar_panel_collected); }
            }

        }
    }

    private void OnGUI()
    {
        SpyHealth.value = health_spy;

        if (SpyHealth.value >= 75)
        { SpyHealthColor.color = okayColor; }
        else if (SpyHealth.value >= 25)
        { SpyHealthColor.color = warningColor; }
        else if (SpyHealth.value > 0)
        { SpyHealthColor.color = criticalColor; }
        else
        { loseScreenShow = true; }

        InventoryImage = GameObject.Find("Inventory0Image")
            .GetComponent<Image>();
        InventoryLabel = GameObject.Find("Inventory0Text")
                .GetComponent<Text>();

        if (solar_panel_collected > 0)
        {
            InventoryImage.enabled = true;

            InventoryLabel.text = string.Format("{0}/{1}",
                solar_panel_collected, solar_panels);
        }
        else
        {
            InventoryImage.enabled = false;
        }

        InventoryImage = GameObject.Find("Inventory1Image")
            .GetComponent<Image>();
        InventoryLabel = GameObject.Find("Inventory1Text")
            .GetComponent<Text>();

        if (batteries_collected > 0)
        {
            InventoryImage.enabled = true;

            InventoryLabel.text = string.Format("{0}/{1}",
                batteries_collected, batteries);
        }
        else
        {
            InventoryImage.enabled = false;
        }

        // Creating win and lose screen buttons
        if (winScreenShow)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 250,
                Screen.height / 2 - 50, 500, 100),
                "You Win!!! (Click to restart)"))
            {
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

    private void RestartLevel()
    {
        // function for restarting the scene
        SceneManager.LoadScene(0);
        Time.timeScale = 1.0f;
    }

    private void WinOrLoseScreen(string cond)
    {
        // function for choosing which freeze
        // screen button to show
        if (cond == "win")
        {
            winScreenShow = true;

        }
        else if (cond == "lose")
        {
            loseScreenShow = true;
        }

        Time.timeScale = 0.0f;
    }

    public void GoToTimeMachine()
    {
        SceneManager.LoadScene("TimeTravelInterface");
    }

}