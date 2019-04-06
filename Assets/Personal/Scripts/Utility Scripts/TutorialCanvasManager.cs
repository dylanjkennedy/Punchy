using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TutorialCanvasManager : MonoBehaviour
{
    private UnityAction<string> listener;

    float timer;
    [SerializeField] TextMeshProUGUI tutText;
    bool displaying;
    float minDisplayTime = 1;
    bool tutorialConditionMet = false;
    enum TutorialCondition : int { move, jump, dash, hit}
    TutorialCondition currTutorialCondition;
    [SerializeField] SpawnManager spawnManager;
    GameObject tether;

    private void Awake()
    {
        //listener = new UnityAction<string>(ActivateCanvas);
    }

    private void OnEnable()
    {
        //EventManager.StartListening("newWave", listener);
    }

    private void OnDisable()
    {
        //EventManager.StopListening("newWave", listener);
    }
    //EventManager.TriggerEvent("newWave", (waveCount+1).ToString());

    // Start is called before the first frame update
    void Start()
    {
        displaying = true;
        tutText.gameObject.SetActive(true);
        currTutorialCondition = TutorialCondition.move;
        ActivateCanvas("Use WASD to move");
    }

    // Update is called once per frame
    void Update()
    {
        if (displaying)
        {
            timer += Time.deltaTime;
        }

        switch (currTutorialCondition)
        {
            case TutorialCondition.move:
                if (Input.GetButton("Horizontal") || Input.GetButton("Vertical")){   // should change to GetButton and configure buttons
                    tutorialConditionMet = true;
                }
                break;
            case TutorialCondition.jump:
                if (Input.GetButton("Jump"))
                { 
                    tutorialConditionMet = true;
                }
                break;
            case TutorialCondition.dash:
                if (Input.GetButton("Dash"))
                {
                    tutorialConditionMet = true;
                }
                break;
            case TutorialCondition.hit:
                // if enemy is dead done
                if (spawnManager.CurrNumOfEnemiesAlive() == 0)
                {
                    tutorialConditionMet = true;
                }
                if (timer > 30)
                {

                    // display help information
                }
                break;
            default:
                break;
        }

        if (tutorialConditionMet && timer > minDisplayTime)
        {
            ProgressTutorial();
        }

    }

    void ProgressTutorial()
    {
        currTutorialCondition++;
        tutText.gameObject.SetActive(false);
        displaying = false;
        tutorialConditionMet = false;
        switch (currTutorialCondition)
        {
            case TutorialCondition.move:
                ActivateCanvas("Use WASD to move");
                break;
            case TutorialCondition.jump:
                ActivateCanvas("Use SPACEBAR to jump");
                break;
            case TutorialCondition.dash:
                ActivateCanvas("Use SHIFT to dash");
                break;
            case TutorialCondition.hit:
                //spawn enemy 
                tether = spawnManager.FindSpawner();
                spawnManager.SpawnEnemy(SpawnManager.EnemyType.Cylinder, tether);
                ActivateCanvas("To hit an enemy, hold down LEFT MOUSE to charge, aim at enemy, then release when wheel is full and green."
                    + "\n Make sure you aren't too far away from the enemy");
                break;
            default:
                break;
        }
    }

    void ActivateCanvas(string text)
    {
        tutText.text = text;
        tutText.gameObject.SetActive(true);
        displaying = true;
        timer = 0;
    }
}
