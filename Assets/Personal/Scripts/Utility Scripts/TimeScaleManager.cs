using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour {

    //All time in this manager should be unscaled, as it is the manager for slow motion and times should be measured in realtime

    float timer;
    float timerEnd;
    float normalTimeScale = 1f;
    float currentTimeScale;
    bool lerping;
    bool normal;
    float lerpFactor;
	// Use this for initialization
	void Start () {
        lerping = false;
        lerpFactor = 0f;
        normal = true;
        currentTimeScale = normalTimeScale;
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (!normal)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= timerEnd)
            {
                currentTimeScale = normalTimeScale;
                normal = true;
            }
            else if (lerping)
            {
                currentTimeScale = Mathf.Lerp(currentTimeScale, normalTimeScale, lerpFactor);
            }
            UpdateTimeScale();
        }
    }

    public void ChangeTimeScale(float newTimeScale, float time, float lerpAmount)
    {
        currentTimeScale = newTimeScale;
        if (lerpAmount == float.NegativeInfinity)
        {
            lerping = false;
        }
        else
        {
            lerping = true;
        }
        if (currentTimeScale != 0)
        {
            normal = false;
        }
        else
        {
            normal = true;
        }
        lerpFactor = lerpAmount;
        timerEnd = time;
        timer = 0;
    }

    public void FullspeedTimeScale()
    {
        currentTimeScale = normalTimeScale;
        UpdateTimeScale();
    }

    void UpdateTimeScale()
    {
        Time.timeScale = currentTimeScale;
        Time.fixedDeltaTime = 0.01666667f * Time.timeScale;
    }

    public float Timescale
    {
        get
        {
            return currentTimeScale;
        }
    }
}
