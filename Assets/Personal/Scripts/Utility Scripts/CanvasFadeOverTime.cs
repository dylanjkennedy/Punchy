using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CanvasFadeOverTime : MonoBehaviour
{
    [SerializeField] float timeToDestroy;
    float timeAlive;
    [SerializeField] float fadeTime;
    Text text;
    Color fadeColor;
    float currentAlpha;
    float lerp;
    float timeFading;
    [SerializeField] float sinkRate;
    // Use this for initialization
    void Start()
    {
        timeAlive = 0;
        if (fadeTime > 0)
        {
            text = this.gameObject.GetComponentInChildren<Text>();
            fadeColor = text.color;
        }
        timeFading = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y - sinkRate, transform.position.z);
        if (timeAlive >= timeToDestroy)
        {
            Destroy(this.gameObject);
        }

        if (timeToDestroy - timeAlive <= fadeTime && fadeTime > 0)
        {
            timeFading += Time.deltaTime;
            currentAlpha = 1 - (timeFading / fadeTime);
            fadeColor.a = currentAlpha;
            text.color = fadeColor;
        }
    }
}
