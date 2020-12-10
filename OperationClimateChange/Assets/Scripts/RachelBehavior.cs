using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RachelBehavior : MonoBehaviour
{

    // turn towards player attributes
    private Transform target;

    // speech attributes
    private uint ProgressPoint;
    private bool MessagePlayed = false;
    public Text RachelSpeech;
    public GameObject SpeechPanel;

    // game assets
    public Animator RachelAnimator;
    public GameBehavior gameBehavior;

    // Start is called before the first frame update
    void Start()
    {
        // attaching game objects
        gameBehavior = GameObject.Find("GameManager")
            .GetComponent<GameBehavior>();

        RachelSpeech = GameObject.Find("RachelSpeech")
            .GetComponent<Text>();

        SpeechPanel = GameObject.Find("RachelBubble");
        SpeechPanel.SetActive(false);

        // finding the player
        target = GameObject.Find("Spy").transform;
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

    private void LateUpdate()
    {
        this.transform.LookAt(target);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Spy")
        {
            PlayMessage();
            RachelAnimator.SetBool("HasSpeech", false);
            MessagePlayed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Spy")
        {
            RachelSpeech.text = "How's it going?";
            SpeechPanel.SetActive(false);
            
        }
    }

    private void PlayMessage()
    {
        // Finding game progress for message
        ProgressPoint = gameBehavior.ProgressPoint;

        SpeechPanel.SetActive(true);

        switch (ProgressPoint)
        {
            case (0):
                RachelSpeech.text = "Head outside and see " +
                    "if you can find some supplies for me. " +
                    "Let's get the time machine running! ";
                break;

            case (1):
                RachelSpeech.text = "Thanks! I'll take those and get"
                    + "the time machine enabled right away!";

                CollectItems();
                gameBehavior.ForceGoalClear();
                gameBehavior.CheckWinCondition();
                break;

            case (2):
                RachelSpeech.text = "Please travel to the past and " +
                    "observe the environment. It would be helpful " +
                    "to have some air samples. I have a feeling what's " +
                    "happening to our planet has something " +
                    "to do with the air...";
                break;

            case (3):
                RachelSpeech.text = "Great! I've built a simulator for how " +
                    "the sun's light hits the earth's atmosphere. " +
                    "Add these samples from the past to see what happens" +
                    " and report back to me!";
                break;

            case (4):
                RachelSpeech.text = "What did you learn?";
                // quiz

                // if passed
                CollectItems();
                gameBehavior.ForceGoalClear();
                gameBehavior.CheckWinCondition();
                break;

            case (5):
                RachelSpeech.text = "Okay! Can you figure out which atmospheric " +
                    "gasses make the earth hotter? I know! " +
                    "Grab some new samples from the past and " +
                    "try to avoid molecules that make earth too hot.";
                break;

            case (6):
                RachelSpeech.text = "Great! Which gasses did you avoid?";
                // quiz

                // if passed
                CollectItems();
                gameBehavior.ForceGoalClear();
                gameBehavior.CheckWinCondition();
                break;

            case (7):
                RachelSpeech.text = "That's frustrating..." +
                    "Your samples got all used up. Go get some more." +
                    "Try to avoid stuff that makes the model too red.";
                CollectItems();
                break;

            case (8):
                RachelSpeech.text = "Fascinating! I wonder what caused " +
                    " there to be so much carbon dioxide and methane " +
                    "in our atmosphere now days...";
                break;
        }
    }

    private void CollectItems()
    {
        gameBehavior.SolarPanels = 0;
        gameBehavior.Batteries = 0;
        gameBehavior.CO2 = 0;
        gameBehavior.Argon = 0;
        gameBehavior.N2 = 0;
        gameBehavior.O2 = 0;
        gameBehavior.Methane = 0;
        gameBehavior.H2O = 0;
    }
}
