using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RachelBehavior : GameBehavior
{
    private uint ProgressPoint;
    private bool MessagePlayed = false;
    public string RachelSpeech = "Hiya!";

    public Animator RachelAnimator;

    // Start is called before the first frame update
    void Start()
    {
        ProgressPoint = GoalDict["LastGoal"];
    }

    // Update is called once per frame
    void Update()
    {
        if (!MessagePlayed)
        {
            RachelAnimator = GameObject.Find("RachelCarson")
                .GetComponent<Animator>();
            RachelAnimator.SetBool("HasSpeech", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Spy")
        {
            PlayMessage();
            RachelAnimator.SetBool("HasSpeech", false);
            MessagePlayed = true;
            RachelSpeech = "How's it going?";
        }
    }

    private void PlayMessage()
    {
        switch (ProgressPoint)
        {
            case (0):
                RachelSpeech = "Head outside and see " +
                    "if you can find some supplies for me. " +
                    "Let's get the time machine running! ";
                break;

            case (1):
                RachelSpeech = "Thanks! I'll take those and get"
                    + "the time machine enabled right away!";

                CollectItems();
                target01_collected = 0;
                target01_goal = 0;
                target02_collected = 0;
                target02_goal = 0;
                CheckWinCondition();
                break;

            case (2):
                RachelSpeech = "Please travel to the past and " +
                    "observe the environment. It would be helpful " +
                    "to have some air samples. I have a feeling what's " +
                    "happening to our planet has something " +
                    "to do with the air...";
                break;

            case (3):
                RachelSpeech = "Great! I've built a simulator for how " +
                    "the sun's light hits the earth's atmosphere. " +
                    "Add these samples from the past to see what happens" +
                    " and report back to me!";
                break;

            case (4):
                RachelSpeech = "What did you learn?";
                // quiz

                // if passed
                CollectItems();
                target01_collected = 0;
                target01_goal = 0;
                target02_collected = 0;
                target02_goal = 0;
                CheckWinCondition();
                break;

            case (5):
                RachelSpeech = "Okay! Can you figure out which atmospheric " +
                    "gasses make the earth hotter? I know! " +
                    "Grab some new samples from the past and " +
                    "try to avoid molecules that make earth too hot.";
                break;

            case (6):
                RachelSpeech = "Great! Which gasses did you avoid?";
                // quiz

                // if passed
                CollectItems();
                target01_collected = 0;
                target01_goal = 0;
                target02_collected = 0;
                target02_goal = 0;
                CheckWinCondition();
                break;

            case (7):
                RachelSpeech = "That's frustrating..." +
                    "Your samples got all used up. Go get some more." +
                    "Try to avoid stuff that makes the model too red.";
                CollectItems();
                break;

            case (8):
                RachelSpeech = "Fascinating! I wonder what caused " +
                    " there to be so much carbon dioxide and methane " +
                    "in our atmosphere now days...";
                break;

        }
    }

    private void CollectItems()
    {
        GoalDict["SolarPanels"] = 0;
        GoalDict["Batteries"] = 0;
        GoalDict["CO2"] = 0;
        GoalDict["Argon"] = 0;
        GoalDict["N2"] = 0;
        GoalDict["O2"] = 0;
        GoalDict["Methane"] = 0;
        GoalDict["Samples"] = 0;
        GoalDict["H2O"] = 0;
    }
}
