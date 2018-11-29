using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    private int score;
    [SerializeField] float comboTimeout;
    [SerializeField] Text scoreText;
    [SerializeField] Text comboText;
    [SerializeField] GameObject scoreCanvas;
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

    public void changeScore(int change, Vector3 position)
    {
        combo++;
        timer = 0;
        int scoreChange = change * combo;
        score += scoreChange;
        scoreText.text = score.ToString();
        comboText.text = combo.ToString();
        createScoreInWorld(scoreChange, position);
    }

    public void createScoreInWorld(int score, Vector3 position)
    {
        GameObject newScoreCanvas = Instantiate(scoreCanvas);
        newScoreCanvas.GetComponentInChildren<Text>().text = score.ToString();
        newScoreCanvas.transform.position = position + new Vector3(0,1.5f,0);
        newScoreCanvas.transform.LookAt(Camera.main.transform.position);
        newScoreCanvas.transform.rotation = new Quaternion(0, newScoreCanvas.transform.rotation.y, 0, newScoreCanvas.transform.rotation.w);
    }
}
