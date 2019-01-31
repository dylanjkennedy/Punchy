using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] SpawnManager spawnManager;
    [SerializeField] EnemyAttackTokenPool tokenPool;
    [SerializeField] ScoreTracker scoreTracker;
    [SerializeField] float difficulty;
    DifficultyValues difficultyValues;

    void Awake()
    {
        DifficultyValues[] difficulties = FindObjectsOfType<DifficultyValues>();
        for (int i = 0; i < difficulties.Length; i++)
        {
            if (difficulties.Length == 1)
            {
                difficultyValues = difficulties[i];
            }
            else if (difficulties[i].isDefault && difficulties.Length > 1)
            {
                Destroy(difficulties[i].gameObject);
            }
            else if (difficulties.Length > 1 && !difficulties[i].isDefault)
            {
                difficultyValues = difficulties[i];
            }
        }

        difficultyValues = difficultyValues.DestroyOnLoad();
    }

    // Update is called once per frame
    void Update()
    {
        difficulty = (2 * Mathf.Sqrt(scoreTracker.Score) + Time.timeSinceLevelLoad) / 10;
    }

    public float Difficulty
    {
        get
        {
            return difficulty;
        }
    }

    public DifficultyValues DifficultyValues
    {
        get
        {
            return difficultyValues;
        }
    }
}
