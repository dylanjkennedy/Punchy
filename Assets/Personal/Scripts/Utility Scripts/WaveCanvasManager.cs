using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class WaveCanvasManager : MonoBehaviour
{
    private UnityAction<string> waveListener;

    [SerializeField] float nextWaveTime;
    [SerializeField] float waveCompleteTime;
    float timer;
    [SerializeField] TextMeshProUGUI waveText;
    bool displaying;
    bool nextWaveDisplayed;
    string wave;

    private void Awake()
    {
        waveListener = new UnityAction<string>(ActivateCanvas);
    }

    private void OnEnable()
    {
        EventManager.StartListening("newWave", waveListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening("newWave", waveListener);
    }

    // Start is called before the first frame update
    void Start()
    {
        wave = "1";
        timer = waveCompleteTime;
        nextWaveDisplayed = true;
        displaying = true;
        waveText.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (displaying)
        {
            timer += Time.deltaTime;
        }

        if (timer >= waveCompleteTime && !nextWaveDisplayed)
        {
            waveText.text = "Wave " + wave.ToString();
            nextWaveDisplayed = true;
        }

        if (timer >= waveCompleteTime + nextWaveTime && displaying)
        {
            waveText.gameObject.SetActive(false);
            displaying = false;
            nextWaveDisplayed = false;
            timer = 0;
        }
    }

    void ActivateCanvas(string waveNumber)
    {
        waveText.text = "Wave " + wave + " complete";
        wave = waveNumber;
        waveText.gameObject.SetActive(true);
        displaying = true;
        timer = 0;
    }
}
