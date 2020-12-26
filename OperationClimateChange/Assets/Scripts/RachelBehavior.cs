using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RachelBehavior : MonoBehaviour
{
    // speech attributes
    private int ProgressPoint;
    private bool MessagePlayed = false;
    public Text RachelSpeech;
    public GameObject SpeechPanel;

    // game assets
    public Animator RachelAnimator;
    public GameBehavior gameBehavior;

    public GameObject QuizPanel;
    public GameObject Correct;
    public GameObject Incorrect;
    public Toggle h2o;
    public Toggle co2;
    public Toggle methane;
    public Toggle o2;
    public Toggle nitrogen;
    public Toggle argon;

    // iterator for preventing multiple overlapping collisions
    private int count = 0;

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

        QuizPanel = GameObject.Find("QuizCanvas");

        // find toggles
        methane = GameObject.Find("q_methane")
            .GetComponent<Toggle>();
        o2 = GameObject.Find("q_oxygen")
            .GetComponent<Toggle>();
        co2 = GameObject.Find("q_carbondioxide")
            .GetComponent<Toggle>();
        argon = GameObject.Find("q_argon")
            .GetComponent<Toggle>();
        h2o = GameObject.Find("q_water")
            .GetComponent<Toggle>();
        nitrogen = GameObject.Find("q_nitrogen")
            .GetComponent<Toggle>();

        // find responses
        Incorrect = GameObject.Find("Wrong");
        Correct = GameObject.Find("Right");

        Incorrect.SetActive(false);
        Correct.SetActive(false);
        QuizPanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (!MessagePlayed)
        {
            RachelAnimator = GameObject.Find("RachelCarson")
                .GetComponent<Animator>();
            RachelAnimator.SetBool("HasSpeech", true);
            count = 0;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Spy")
        {
            SpeechPanel.SetActive(true);

            if (count == 0)
            { 
                PlayMessage();
                RachelAnimator.SetBool("HasSpeech", false);
                MessagePlayed = true;
                count += 1;
            }
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

        switch (ProgressPoint)
        {
            case (0):
                RachelSpeech.text = "Head outside and see " +
                    "if you can find some supplies for me. " +
                    "Let's get the time machine running! ";
                break;

            case (1):
                RachelSpeech.text = "Thanks! I'll take those and get "
                    + "the time machine enabled right away!";

                gameBehavior.CheckWinCondition();
                CollectItems();
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
                gameBehavior.CheckWinCondition();
                break;

            case (4):

                if ( gameBehavior.SumOfGasses() == 0)
                { 
                RachelSpeech.text = "Interesting! So some kinds of " +
                    "molecules in the air trap more heat on earth than" +
                    " others... I wonder which ones do what!";
                    gameBehavior.CheckWinCondition();
                }
                else
                {
                    RachelSpeech.text = "Did you find the model and try out " +
                        " all your samples?";
                }
                break;

            case (5):
                RachelSpeech.text = "Okay! Can you figure out which atmospheric " +
                    "gasses make the earth hotter? I know! " +
                    "Grab some new samples from the past and " +
                    "try to avoid molecules that make earth too hot.";
                
                break;

            case (6):
                RachelSpeech.text = "Nice work so far! " +
                    "Go test your samples and tell me if " +
                    "the tempurature balances!";
                break;

            case (7):
                RachelSpeech.text = "That's frustrating..." +
                    "Your samples got all used up. Go get some more." +
                    "Try to avoid stuff that makes the model too red.";
                break;

            case (8):
                RachelSpeech.enabled = false;
                QuizPanel.SetActive(true);

                break;
            case (9):
                RachelSpeech.enabled = true;
                RachelSpeech.text = "Fascinating! I wonder what caused " +
                    " there to be so much carbon dioxide and methane " +
                    "in our atmosphere now days...";
                break;
        }
    }

    public void CheckQuiz()
    {
        if (methane.isOn
            && !o2.isOn
            && !nitrogen.isOn
            && h2o.isOn
            && co2.isOn
            && !argon.isOn)
        {
            Correct.SetActive(true);
        }
        else
        {
            Incorrect.SetActive(true);
        }
    }

    public void closeQuiz()
    {
        QuizPanel.SetActive(false);
        gameBehavior.CheckWinCondition();
    }

    public void closeIncorrect()
    {
        Incorrect.SetActive(false);
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
