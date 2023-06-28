using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyBehaviour EnemyBehaviour;
    private IEnumerator spawner;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private float _delay = 5f;
    [SerializeField]
    private int _groupSize = 5;

    [SerializeField] // for debugging, read these values out of a list later
    private int levelTotalHp = 1000;
    [SerializeField]
    private float levelSpawnedHp = 0;

    [SerializeField]
    private int _currentWave = 0;

    [SerializeField]
    private int[] _levelDifficulties = new int[] { 1000, 2000, 3000, 4000, 5000, 6000, 7000 };



    // To get playerHp
    private EnemyBehaviour enemyBehaviour;

    void Start()
    {       
        enemyBehaviour = _enemyPrefab.GetComponent<EnemyBehaviour>();
    }


    private int[] waveHealthDistribution(int length, int level_hp)
    {
        // Returns an array[length] of values that sum up to level_hp, representing the total playerHp of each wave in this level.
        // Values are taken from a linear function and normalised such that each wave is stronger than the previous

        int[] distr = new int[length];
        float step = 1f / (length - 1); // Difference between each element

        float t = 0f;

        for (int i = 0; i < length; i++)
        {
            float value = Mathf.Lerp(0f, level_hp, t); // interpolate value between 0 and total
            distr[i] = Mathf.RoundToInt(value); // round to integer
            level_hp -= distr[i]; // subtract from total
            t += step; // increment interpolation value
        }

        // handle rounding error by adding remaining value to last element
        distr[length - 1] += level_hp;

        return distr;

    }

    private void spawnGroupSelector()
    {
        // Have another fct that decides which enemy types are "active" (e.g. can attack) based on the level.
        // Pass the list of active enemies (string[]) in here, choose one at random,
        // and call spawnGroup with the right parameters 
    }

    private void spawnGroup(int memberAmount, float memberHp, Vector2 spawnPoint, GameObject objectToSpawn)
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
            levelSpawnedHp += memberHp;
            GameControl.control.enemiesRemaining++;
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
        // returns false if there are no living enemies on the map
        // remember that this can be false during a level (in between waves)

        bool isAlive = true;
        return isAlive;
    }
    

    public void StartSpawning()
    {
        levelSpawnedHp = 0f;
        levelTotalHp = _levelDifficulties[GameControl.control.currentLevel];
        spawner = SpawnWaves();
        StartCoroutine(spawner);
    }

    public void StopSpawning()
    {
        StopCoroutine(spawner);
        Debug.Log("Stop spawning called.");
    }

    // Spawns waves of enemies based only on current level:
    private IEnumerator SpawnWaves()
    {
        // current level stats
        float enemyHp = enemyBehaviour.maxHp; //should be fine up here for one enemy type - check once others are added
        int waveAmount = GameControl.control.currentLevel / 2 + 1;
        int[] currentLevelWaves = waveHealthDistribution(waveAmount, levelTotalHp);
        int currentWaveNr = 0;
        
        //Debug.Log("currentLevelWaves[0] and currentLevel:");
        //Debug.Log(currentLevelWaves[0]);
        //Debug.Log(GameControl.control.currentLevel);


        //also have different types of groups later to spawn different swarms
        // TODO group selector fct that selects the spawn group and spawns it with the right vals

        // IN CONCLUSION 
        // what I want is:
        /* For each level spawn waveAmount waves
         *      for each wave spawn x groups
         *              each group consists of z members and spawns together
         * x is based on total playerHp
         * groups are manually defined, such that they are similarly strong but feel different
         */

        // Make the waves.
        while (levelSpawnedHp < levelTotalHp)
        {

            while (currentWaveNr < currentLevelWaves.Length)
            {
                
                // Spawn groups in regular intervals until the wave is over
                float spawnedHp = 0; 
                int waveHp = currentLevelWaves[currentWaveNr]; // max HP for this wave

                while ((spawnedHp < waveHp))
                {

                    //spawnedEnemies.Add(spawnGroup(5, chooseSpawnpoint(), _enemyPrefab)); // TODO replace this with spawnGroupSelector once implemented
                    spawnGroup(memberAmount:_groupSize, memberHp:enemyHp, spawnPoint:chooseSpawnpoint(), objectToSpawn:_enemyPrefab);
                    spawnedHp += enemyHp * _groupSize;
                    Debug.Log("spawned hp:" + spawnedHp);
                    Debug.Log("wave" + currentWaveNr + "of " + currentLevelWaves.Length);
                    Debug.Log("Enemies Remaining: " + GameControl.control.enemiesRemaining);

                    yield return new WaitForSeconds(_delay); // delay between groups
                }

                Debug.Log("wave over");
                currentWaveNr++;
                yield return new WaitForSeconds(_delay*2); // delay between waves
                
            }

        }

        while (GameControl.control.enemiesRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        GameControl.control.OnFightWin();
        yield return null;
    }

}

