using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour {
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

    public void ChangeScore(int change, Vector3 position)
    {
        combo++;
        timer = 0;
        int scoreChange = change * combo;
        score += scoreChange;
        scoreText.text = score.ToString();
        comboText.text = combo.ToString();
        CreateScoreInWorld(scoreChange, position);
    }

    public void CreateScoreInWorld(int score, Vector3 position)
    {
        GameObject newScoreCanvas = Instantiate(scoreCanvas);
        Text newScoreText = newScoreCanvas.GetComponentInChildren<Text>();
        newScoreText.text = score.ToString();
        if (score <= 50)
        {
            newScoreText.color = Color.Lerp(Color.white, Color.blue, (float)score / 50f);
        }
        else
        {
            newScoreText.color = Color.Lerp(Color.blue, Color.red, (float)(score-50) / 50f);
        }
        newScoreCanvas.transform.position = position + new Vector3(0, 1f, 0);
        newScoreCanvas.transform.position = position + new Vector3(0,1f,0);
        newScoreCanvas.transform.LookAt(Camera.main.transform.position);
        newScoreCanvas.transform.rotation = new Quaternion(0, newScoreCanvas.transform.rotation.y, 0, newScoreCanvas.transform.rotation.w);
    }

    public int Score
    {
        get
        {
            return score;
        }
    }
}
