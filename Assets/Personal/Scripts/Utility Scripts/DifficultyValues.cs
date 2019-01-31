using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyValues : MonoBehaviour
{
    [SerializeField] TokenPoolsValues[] tokenPoolsEnemyTypes;
    [SerializeField] SpawnValues spawnDifficultyValues;
    [SerializeField] bool isDefaultValues;

    private void Awake()
    {
        
    }

    public DifficultyValues DestroyOnLoad()
    {
        this.gameObject.DoDestroyOnLoad();
        return this;
    }

    public TokenPoolsValues[] TokenPoolsEnemyTypes
    {
        get
        {
            return tokenPoolsEnemyTypes;
        }
    }

    public SpawnValues SpawnDifficultyValues
    {
        get
        {
            return spawnDifficultyValues;
        }
    }

    public bool isDefault
    {
        get
        {
            return isDefaultValues;
        }
    }

    [System.Serializable]
    public class SpawnValues : object
    {
        [SerializeField] int[] baseSpawnLimits;
        [SerializeField] float[] spawnLimitsGrowth;

        public int[] BaseSpawnLimits
        {
            get
            {
                return baseSpawnLimits;
            }
        }

        public float[] SpawnLimitsGrowth
        {
            get
            {
                return spawnLimitsGrowth;
            }
        }
    }

    [System.Serializable]
    public class TokenPoolsValues : object
    {
        [SerializeField] SpawnManager.EnemyType enemyType;
        [SerializeField] int[] initialTokensPerAttackType;
        [SerializeField] float[] tokenGrowthPerAttackType;
        [SerializeField] float[] attackTypeCooldowns;

        public SpawnManager.EnemyType EnemyType
        {
            get
            {
                return enemyType;
            }
        }

        public int[] InitialTokensPerAttackType
        {
            get
            {
                return initialTokensPerAttackType;
            }
        }

        public float[] TokenGrowthPerAttackType
        {
            get
            {
                return tokenGrowthPerAttackType;
            }
        }

        public float[] AttackTypeCooldowns
        {
            get
            {
                return attackTypeCooldowns;
            }
        }
    }
}
