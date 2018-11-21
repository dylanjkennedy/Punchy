using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacksManager : MonoBehaviour {
    SpawnManager spawnManager;
    [SerializeField] EnemyTypeTokens[] enemyTypes;

    // Use this for initialization
    void Start()
    {
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
            EnemyType.Update();
        }
    }

    //Request a token for a specific attack type from a enemy, remove token from pool if available
    public Token RequestToken(GameObject enemy, int attackType)
    {
        int enemyType = (int)enemy.GetComponent<EnemyController>().Type;
        if (enemyTypes[enemyType].attackAvailable(attackType))
        {
            return enemyTypes[enemyType].removeToken(enemy, attackType);
        }
        return null;
    }

    //Return a token for a specific attack type for a specific enemy type to the pool
    public void ReturnToken(SpawnManager.EnemyType type, Token token)
    {
        enemyTypes[(int)type].returnToken(token);
    }


    //Structured this way since you need to have a serializable class and an array of the serializable class to have
    //nested arrays in Unity that are editor-visible. This allows us to have a top-level array of enemy types which 
    //internally have an array of attack types which are all tracked individually.

        //Should this be implemented as an array of lists?
    [System.Serializable]
    class EnemyTypeTokens : object
    {
        [SerializeField] SpawnManager.EnemyType enemyType;
        [SerializeField] int[] maxTokensPerAttackType;
        [SerializeField] float[] attackTypeCooldowns;

        //Token[][] tokens;
        //private int[] tokensPerAttackType;
        List<Token>[] availableTokens;
        List<Token>[] coolingTokens;
        List<Token>[] takenTokens;

        public void Start()
        {
            availableTokens = new List<Token>[maxTokensPerAttackType.Length];
            coolingTokens = new List<Token>[maxTokensPerAttackType.Length];
            takenTokens = new List<Token>[maxTokensPerAttackType.Length];
            for (int i = 0; i < maxTokensPerAttackType.Length; i++)
            {
                availableTokens[i] = new List<Token>();
                coolingTokens[i] = new List<Token>();
                takenTokens[i] = new List<Token>();
                while (availableTokens[i].Count < maxTokensPerAttackType[i])
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

        public void Update()
        {
            List<Token>[] cooledTokens = new List<Token>[maxTokensPerAttackType.Length];
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < coolingTokens.Length; i++)
            {
                cooledTokens[i] = new List<Token>();
                foreach(Token token in coolingTokens[i])
                {
                    if (token.updateCooldown(deltaTime))
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
        }

        public bool attackAvailable(int attackType)
        {
            return (availableTokens[attackType].Count > 0);
        }

        public void returnToken(Token token)
        {
            takenTokens[token.Type].Remove(token);
            coolingTokens[token.Type].Add(token);
            token.returnToken();
        }

        public Token removeToken(GameObject taker, int attackType)
        {
            Token token = availableTokens[attackType][0];
            availableTokens[attackType].Remove(token);
            token.takeToken(taker);
            return token;
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

        public bool updateCooldown(float time)
        {
            currentCooldown += time;
            if (currentCooldown >= attackCooldown)
            {
                available = true;
                return true;
            }
            return false;
        }

        public void takeToken(GameObject taker)
        {
            owner = taker;
            available = false;
        }

        public void returnToken()
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
    }
}
