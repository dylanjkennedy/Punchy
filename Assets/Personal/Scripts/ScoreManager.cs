using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    private int score;
    [SerializeField] float comboTimeout;
    [SerializeField] Text scoreText;
    [SerializeField] Text comboText;
    float timer;
    int combo;
	// Use this for initialization
	void Start () {
        score = 0;
        timer = 0;
        combo = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer >= comboTimeout)
        {
            combo = 0;
            comboText.text = combo.ToString();
        }
	}

    public void changeScore(int change)
    {
        combo++;
        timer = 0;
        score += change*combo;
        scoreText.text = score.ToString();
        comboText.text = combo.ToString();
    }
}
