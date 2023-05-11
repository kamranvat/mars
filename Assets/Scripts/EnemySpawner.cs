using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyBehaviour EnemyBehaviour;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private float _delay = 1f;

    [SerializeField]
    private int _groupSize = 5;

    [SerializeField]
    private bool _dead = false;

    [SerializeField] // for debugging
    public int _currentLevel = 1;

    [SerializeField] // for debugging, read these values out of a list later
    private int _levelTotalHp = 1000;
    [SerializeField]
    private int _levelSpawnedHp = 0;

    [SerializeField]
    private int _currentWave = 1;

    [SerializeField]
    private int[] _levelDifficulties = new int[] { 100, 200, 300, 400 };

    // To get hp
    private EnemyBehaviour enemyBehaviour;

    void Start()
    {       
        enemyBehaviour = _enemyPrefab.GetComponent<EnemyBehaviour>();

        // Todo: on player death, stop coroutine (in update function)
        StartCoroutine(LevelSystem());
    }

    void Update()
    {

    }

    private int[] waveHealthDistribution(int length, int level_hp)
    {
        // Returns an array[length] of values that sum up to level_hp, representing the total hp of each wave in this level.
        // Values are taken from a linear function and normalised such that each wave is stronger than the previous

        int[] distr = new int[length];
        float step = 1f / (length - 1); // Difference between each element

        float t = 0f;

        Debug.Log(message: "Wave health init...");

        for (int i = 0; i < length; i++)
        {
            float value = Mathf.Lerp(0f, level_hp, t); // interpolate value between 0 and total
            distr[i] = Mathf.RoundToInt(value); // round to integer
            level_hp -= distr[i]; // subtract from total
            t += step; // increment interpolation value
        }

        // handle rounding error by adding remaining value to last element
        distr[length - 1] += level_hp;

        Debug.Log(message: "Wave health initialized. Values: " + distr);
        return distr;

    }

    private void spawnGroupSelector()
    {
        // Have another fct that decides which enemy types are "active" (e.g. can attack) based on the level.
        // Pass the list of active enemies (string[]) in here, choose one at random,
        // and call spawnGroup with the right parameters 
    }

    private void spawnGroup(int memberAmount, int memberHp, Vector2 spawnPoint, GameObject objectToSpawn)
    {
        float dist = 5f;

        // TODO: make random velocity, assign to each member  within the for loop
        //give each group a (similar between members) initial velocity parallel to the circle defined
        //by the distance d to the center, d such that it is out of view
        for (int i = 0; i <= memberAmount; i++)
        {
            // Spawn at spawnpoint plus random offset
            Vector2 position = spawnPoint + new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * dist;
            Instantiate(objectToSpawn, position, Quaternion.identity);
            _levelSpawnedHp += memberHp; 
        }
    }


    // TODO: build functionality
    private Vector2 chooseSpawnpoint()
    {
        //TODO make it choose one spawnpoint at random
        //TODO define spawn points as game corners plus random spread range plus one

        /*Vector2[] spawnPointArray = new Vector2[4];

        spawnPointArray[0] = new Vector2(8, 4);
        spawnPointArray[1] = new Vector2(-8, 4);
        spawnPointArray[2] = new Vector2(8, -4);
        spawnPointArray[3] = new Vector2(-8, -4);*/

        return new Vector2(20, 10);
    }

    // TODO: build functionality
    private bool isWaveAlive()
    {
        // should only be false if the last group has been spawned and all enemies are dead
        // handle this by making "waveOver" a public variable
        return true;
    }

    // TODO: build functionality
    private bool isLevelActive() 
    { 
        // should return true if either the player dies or the last wave of a level is dead
        return true;
    }


    // Spawns waves of enemies based on what the current level is:
    IEnumerator LevelSystem()
    {
        // current level stats
        int enemyHp = enemyBehaviour._hp; //should be fine up here for one enemy type - check once others are added
        int waveAmount = _currentLevel / 2 + 1;
        int[] currentLevelWaves = waveHealthDistribution(waveAmount, _levelTotalHp);
        int currentWaveNr = 0;
        bool levelActive = isLevelActive();

        bool levelWon = false;
        bool levelLost = false;
        
        Debug.Log("currentLevelWaves[0] and currentLevel:");
        Debug.Log(currentLevelWaves[0]);
        Debug.Log(_currentLevel);


        //also have different types of groups later to spawn different swarms
        // TODO group selector fct that selects the spawn group and spawns it with the right vals

        // IN CONCLUSION 
        // what I want is:
        /* For each level spawn level/2 + 1 waves
         *      for each wave spawn x groups
         *              each group consists of z members and spawns together
         * x is based on total hp
         * groups are manually defined, such that they are similarly strong but feel different
         */

        // Make the waves. add player death somehow - levelLost/levelWon are not implemented properly (and might not be a good approach)
        while (_levelSpawnedHp < _levelTotalHp)
        {

            if (levelWon)
            {
                // on level win:
                _currentLevel++;
                levelActive = false;
                Debug.Log(message: "level won");
            }

            if (levelLost)
            {
                // on level loss:
                levelActive = false;
                Debug.Log(message: "level lost");
            } // honestly i think I should check for player death in update() and stop calling the subroutines here if it happens

            while ((currentWaveNr < currentLevelWaves.Length) && !levelLost)
            {
                

                // Spawn groups in regular intervals until the wave is over
                int spawnedHp = 0; 
                int waveHp = currentLevelWaves[currentWaveNr]; // max HP for this wave
                // List<GameObject> spawnedEnemies = new List<GameObject>();

                while ((spawnedHp < waveHp) && !levelLost)
                {
                    //spawnedEnemies.Add(spawnGroup(5, chooseSpawnpoint(), _enemyPrefab)); // TODO replace this with spawnGroupSelector once implemented
                    spawnGroup(memberAmount:_groupSize, memberHp:enemyHp, spawnPoint:chooseSpawnpoint(), objectToSpawn:_enemyPrefab);
                    spawnedHp += enemyHp * _groupSize;
                    Debug.Log("spawned hp:" + spawnedHp);
                    Debug.Log("wave" + currentWaveNr + "of " + currentLevelWaves.Length);

                    yield return new WaitForSeconds(_delay);
                }

                // Monitor the wave - the last enemies should be on the map now
                // while(true){check if enemies alive} -> as soon as not, or 30s pass, levelWon = true
                // If the wave dies -> wait, then next wave
                // If the wave dies && it was the last wave -> levelWon
                Debug.Log("wave over");
                currentWaveNr++;
                yield return new WaitForSeconds(_delay*2); // delay between waves
                
            }

        }

        _currentLevel++; //TODO replace with proper "levelWon" structure or something like it
        Debug.Log(message: "level increased to " + _currentLevel);
        yield return null;

    }

}

