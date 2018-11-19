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
    }

    //Request a token for a specific attack type from a specific enemy type, remove token from pool if available
    public bool RequestToken(SpawnManager.EnemyType type, int attackType)
    {
        if (enemyTypes[(int)type].attackAvailable(attackType))
        {
            enemyTypes[(int)type].removeToken(attackType);
            return true;
        }
        return false;
    }

    //Return a token for a specific attack type for a specific enemy type to the pool
    public void ReturnToken(SpawnManager.EnemyType type, int attackType)
    {
        enemyTypes[(int)type].returnToken(attackType);
    }


    //Structured this way since you need to have a serializable class and an array of the serializable class to have
    //nested arrays in Unity that are editor-visible. This allows us to have a top-level array of enemy types which 
    //internally have an array of attack types which are all tracked individually.

        //Should this be implemented as an array of lists?
    [System.Serializable]
    class EnemyTypeTokens : MonoBehaviour
    {
        [SerializeField] SpawnManager.EnemyType enemyType;
        [SerializeField] int[] maxTokensPerAttackType;
        [SerializeField] float[] attackTypeCooldowns;

        Token[][] tokens;
        private int[] tokensPerAttackType;
        List<Token>[] availableTokens;
        List<Token>[] coolingTokens;
        List<Token>[] takenTokens;

        private void Start()
        {
            availableTokens = new List<Token>[maxTokensPerAttackType.Length];
            coolingTokens = new List<Token>[maxTokensPerAttackType.Length];
            takenTokens = new List<Token>[maxTokensPerAttackType.Length];
            for (int i = 0; i < maxTokensPerAttackType.Length; i++)
            {
                while (availableTokens[i].Count < maxTokensPerAttackType[i])
                {
                    availableTokens[i].Add(new Token(attackTypeCooldowns[i]));
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

        private void Update()
        {
            List<Token>[] cooledTokens = new List<Token>[maxTokensPerAttackType.Length];
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < coolingTokens.Length; i++)
            {
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

        public void returnToken(int attackType)
        {

        }

        public void removeToken(int attackType)
        {
            
        }
    }

    class Token : object
    {
        GameObject owner;
        float attackCooldown;
        float currentCooldown;
        bool available;
        public Token(float cooldown)
        {
            attackCooldown = cooldown;
            currentCooldown = attackCooldown;
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

        public bool Cooling
        {
            get
            {
                return (currentCooldown < attackCooldown);
            }
        }
    }
}
