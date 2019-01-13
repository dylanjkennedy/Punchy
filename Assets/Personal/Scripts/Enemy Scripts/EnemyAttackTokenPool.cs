using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTokenPool : MonoBehaviour {
    SpawnManager spawnManager;
    [SerializeField] EnemyTypeTokens[] enemyTypes;
    DifficultyManager difficultyManager;

    // Use this for initialization
    void Start()
    {
        difficultyManager = gameObject.GetComponent<DifficultyManager>();
        spawnManager = gameObject.GetComponent<SpawnManager>();
        foreach (EnemyTypeTokens EnemyType in enemyTypes)
        {
            EnemyType.Start();
        }
    }

    private void Update()
    {
        foreach (EnemyTypeTokens EnemyType in enemyTypes)
        {
            EnemyType.Update(difficultyManager.Difficulty);
        }
    }

    //Request a token for a specific attack type from a enemy, remove token from pool if available
    public Token RequestToken(GameObject enemy, int attackType)
    {
        int enemyType = (int)enemy.GetComponent<EnemyController>().Type;
        if (enemyTypes[enemyType].isAttackAvailable(attackType))
        {
            return enemyTypes[enemyType].RemoveToken(enemy, attackType);
        }
        return null;
    }

    //Return a token for a specific attack type for a specific enemy type to the pool
    public void ReturnToken(SpawnManager.EnemyType type, Token token)
    {
        enemyTypes[(int)type].ReturnToken(token);
    }


    //Structured this way since you need to have a serializable class and an array of the serializable class to have
    //nested arrays in Unity that are editor-visible. This allows us to have a top-level array of enemy types which 
    //internally have an array of attack types which are all tracked individually.

        //Should this be implemented as an array of lists?
    [System.Serializable]
    class EnemyTypeTokens : object
    {
        [SerializeField] SpawnManager.EnemyType enemyType;
        [SerializeField] int[] initialTokensPerAttackType;
        [SerializeField] float[] tokenGrowthPerAttackType;
        [SerializeField] float[] attackTypeCooldowns;

        //Token[][] tokens;
        //private int[] tokensPerAttackType;
        List<Token>[] availableTokens;
        List<Token>[] coolingTokens;
        List<Token>[] takenTokens;

        public void Start()
        {
            availableTokens = new List<Token>[initialTokensPerAttackType.Length];
            coolingTokens = new List<Token>[initialTokensPerAttackType.Length];
            takenTokens = new List<Token>[initialTokensPerAttackType.Length];
            for (int i = 0; i < initialTokensPerAttackType.Length; i++)
            {
                availableTokens[i] = new List<Token>();
                coolingTokens[i] = new List<Token>();
                takenTokens[i] = new List<Token>();
                while (availableTokens[i].Count < initialTokensPerAttackType[i])
                {
                    availableTokens[i].Add(new Token(attackTypeCooldowns[i], i));
                }
            }

            /*
            //Each token is an object which tracks its own availability and cooldown
            //"tokens" is a 2D array of tokens, where tokens[i][j] represents the jth token of attack type i.
            tokens = new Token[attackTypeCooldowns.Length][];
            for (int i = 0; i < attackTypeCooldowns.Length; i++)
            {
                tokens[i] = new Token[maxTokensPerAttackType[i]];
                for (int j = 0; j < tokens[i].Length; j++)
                {
                    tokens[i][j] = new Token(attackTypeCooldowns[i]);
                }
            }
            */
        }

        public void Update(float difficulty)
        {
            List<Token>[] cooledTokens = new List<Token>[initialTokensPerAttackType.Length];
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < coolingTokens.Length; i++)
            {
                cooledTokens[i] = new List<Token>();
                foreach(Token token in coolingTokens[i])
                {
                    if (token.UpdateCooldown(deltaTime))
                    {
                        cooledTokens[i].Add(token);
                    }
                }
            }

            for (int i = 0; i < cooledTokens.Length; i++)
            {
                foreach(Token token in cooledTokens[i])
                {
                    coolingTokens[i].Remove(token);
                    availableTokens[i].Add(token);
                }
            }
            UpdateMaxTokens(difficulty);
        }

        void UpdateMaxTokens(float difficulty)
        {
            for (int i = 0; i < initialTokensPerAttackType.Length; i++)
            {
                if (totalTokens(i) < initialTokensPerAttackType[i] + Mathf.FloorToInt(tokenGrowthPerAttackType[i] * difficulty))
                {
                    availableTokens[i].Add(new Token(attackTypeCooldowns[i], i));
                }
            }
        }

        public bool isAttackAvailable(int attackType)
        {
            return (availableTokens[attackType].Count > 0);
        }

        public void ReturnToken(Token token)
        {
            takenTokens[token.Type].Remove(token);
            coolingTokens[token.Type].Add(token);
            token.ReturnToken();
        }

        public Token RemoveToken(GameObject taker, int attackType)
        {
            Token token = availableTokens[attackType][0];
            availableTokens[attackType].Remove(token);
            takenTokens[attackType].Add(token);
            token.TakeToken(taker);
            return token;
        }

        public int totalTokens(int type)
        {
            return availableTokens[type].Count + coolingTokens[type].Count + takenTokens[type].Count;
        }
    }

    public class Token : object
    {
        GameObject owner;
        float attackCooldown;
        float currentCooldown;
        int type;
        bool available;
        public Token(float cooldown, int tokenType)
        {
            attackCooldown = cooldown;
            currentCooldown = attackCooldown;
            type = tokenType;
            available = true;
        }

        public bool UpdateCooldown(float time)
        {
            currentCooldown += time;
            if (currentCooldown >= attackCooldown)
            {
                available = true;
                return true;
            }
            return false;
        }

        public void TakeToken(GameObject taker)
        {
            owner = taker;
            available = false;
        }

        public void ReturnToken()
        {
            owner = null;
            currentCooldown = 0;
        }

        public bool Available
        {
            get
            {
                return available;
            }
        }

        public int Type
        {
            get
            {
                return type;
            }
        }

        public bool Cooling
        {
            get
            {
                return (currentCooldown < attackCooldown);
            }
        }

        public GameObject Owner
        {
            get
            {
                return owner;
            }
        }
    }
}
