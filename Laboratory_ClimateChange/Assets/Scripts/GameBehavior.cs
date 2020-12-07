using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehavior : MonoBehaviour
{
    public HealthBar healthBar;
    public uint maxhealth = 100;
    private uint health_spy = 100;
    public string labelText = "Collect 2 solar panels and 3 batteries. Avoid the fires";
    public int solar_panels = 2;
    public int batteries = 3;
    private bool winScreenShow = false;
    private bool loseScreenShow = false;
    private uint solar_panel_collected = 0;
    private uint batteries_collected = 0;

    void Start(){
        healthBar.SetMaxHealth(maxhealth);
    }
    public uint HealthSpy
    {
        
        get {return health_spy; }
        set
        {
            health_spy = value;
            Debug.Log(health_spy);
            healthBar.SetHealth(health_spy);
            if(health_spy <= 0)
            {
                labelText = "Oh no!";
                WinOrLoseScreen("lose");
            }
            else
            {
                labelText = "Ouch!";
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
                if(batteries_collected == batteries){
                labelText = "You got all the missing pieces of the time-travel machine";
                WinOrLoseScreen("win");}
                else{
                labelText = string.Format("Nice! You got all the solar panels! \n Only {0} batteries left to find!",
                    batteries - batteries_collected);
                }
            }
            else
            {
             if(batteries_collected == batteries){
                labelText = "Only 1 solar panel left to find!";}
                else{
                labelText = string.Format("Nice! 1 solar panel and {0} batteries left to find!",
                    batteries - batteries_collected);}
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
                if(solar_panel_collected == solar_panels){
                labelText = "You got all the missing pieces of the time-travel machine";
                WinOrLoseScreen("win");}
                else{
                labelText = string.Format("Nice! You got all the batteries! \n Only {0} solar panels left to find!",
                    solar_panels - solar_panel_collected);
                }
            }
            else
            {
             if(solar_panel_collected == solar_panels){
                labelText = string.Format("Only {0} batteries left to find!", batteries - batteries_collected);}
                else{
                labelText = string.Format("Nice! {0} batteries and {1} solar panels left to find!",
                    batteries - batteries_collected, solar_panels - solar_panel_collected);}
            }       

        }
    }

    private void OnGUI()
    {
        // adjusting fonts and colors for readablity
        GUI.skin.label.fontSize = 18;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUI.skin.box.fontStyle = FontStyle.Normal;
        GUI.skin.box.fontSize = 18;
        GUI.skin.button.fontSize = 24;
        GUI.backgroundColor = new Color(0, 0, 0, 1);

        // doubilng GUI boxes because alpha element doesn't
        // seem to help with opacity and they are hard to read
        GUI.Box(new Rect(20, 20, 250, 30), "");
        GUI.Box(new Rect(20, 20, 250, 30),
            "Spy's Health: " + health_spy);

        GUI.Box(new Rect(20, 55, 250, 30),"");
        GUI.Box(new Rect(20, 55, 250, 30),
            "Solar Panel Collected: " + solar_panel_collected);

        GUI.Box(new Rect(20, 80, 250, 30), "");
        GUI.Box(new Rect(20, 80, 250, 30),
            "Batteries collected: " + batteries_collected);

        GUI.contentColor = new Color(0, 0, 0);

        GUI.Label(new Rect((Screen.width / 2) - 100,
            Screen.height - 50, 300, 50), labelText);

        GUI.contentColor = new Color(255, 255, 255, 1);

        // Creating win and lose screen buttons
        if(winScreenShow)
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
        if(cond == "win")
        {
            winScreenShow = true;

        }
        else if (cond == "lose")
        {
            loseScreenShow = true;
        }

        Time.timeScale = 0.0f;
    }

}