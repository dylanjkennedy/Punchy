using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SelectDifficultyAndLoadScene(string sceneName)
    {
        GameObject difficultyValuesObject = GetComponentInChildren<DifficultyValues>().gameObject;
        difficultyValuesObject.transform.SetParent(null);
        DontDestroyOnLoad(difficultyValuesObject);
        SceneManager.LoadScene(sceneName);
    }
}
