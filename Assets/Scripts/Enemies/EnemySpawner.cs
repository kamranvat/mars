using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public EnemyBehaviour EnemyBehaviour;
    private IEnumerator spawner;

    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private int _groupSize = 5;
    [SerializeField]
    private float _groupDelay = 5f;
    [SerializeField]
    private float _waveDelay = 10f;


    [SerializeField] // for debugging, read these values out of a list later
    private int levelTotalHp = 1000;
    [SerializeField]
    private float levelSpawnedHp = 0;

    [SerializeField]
    private int currentWave = 0;

    [SerializeField]
    private int[] _levelDifficulties = new int[] { 1000, 2000, 3000, 4000, 5000, 6000, 7000 };



    // To get playerHp
    private EnemyBehaviour enemyBehaviour;

    void Start()
    {       
        enemyBehaviour = _enemyPrefab.GetComponent<EnemyBehaviour>();
    }


    private int[] WaveHealthDistribution(int length, int levelHp)
    {
        // Returns an array[length] of values that sum up to levelHp, representing the total playerHp of each wave in this level.
        // Values are taken from a linear function and normalised such that each wave is stronger than the previous

        int[] distr = new int[length];
        float step = 1f / (length - 1); // Difference between each element

        float t = 0f;

        for (int i = 0; i < length; i++)
        {
            float value = Mathf.Lerp(0f, levelHp, t); // interpolate value between 0 and total
            distr[i] = Mathf.RoundToInt(value); // round to integer
            levelHp -= distr[i]; // subtract from total
            t += step; // increment interpolation value
        }

        // handle rounding error by adding remaining value to last element
        distr[length - 1] += levelHp;

        return distr;

    }

    private void SpawnGroupSelector()
    {
        // Have another fct that decides which enemy types are "active" (e.g. can attack) based on the level.
        // Pass the list of active enemies (string[]) in here, choose one at random,
        // and call SpawnGroup with the right parameters 
    }

    private void SpawnGroup(int memberAmount, Vector2 spawnPoint, GameObject objectToSpawn)
    {
        float dist = 5f;
        float memberHp = enemyBehaviour.maxHp;

        // TODO: make random velocity, assign to each member  within the for loop
        //give each group a (similar between members) initial velocity parallel to the circle defined
        //by the distance d to the center, d such that it is out of view
        for (int i = 0; i <= memberAmount; i++)
        {
            // Spawn at spawnpoint plus random offset
            Vector2 position = spawnPoint + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * dist;
            Instantiate(objectToSpawn, position, Quaternion.identity);
            levelSpawnedHp += memberHp;
            GameControl.control.enemiesRemaining++;
        }
    }


    // TODO: build functionality
    private Vector2 ChooseSpawnpoint()
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
    private bool IsWaveAlive()
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
        int[] currentLevelWaves = WaveHealthDistribution(length:waveAmount, levelHp:levelTotalHp);
        currentWave = 0;
        //also have different types of groups later to spawn different swarms
        // TODO group selector fct that selects the spawn group and spawns it with the right vals

        // what I want is:
        /* For each level spawn waveAmount waves
         *      for each wave spawn x groups
         *              each group consists of z members and spawns together
         * x is based on total playerHp
         * groups are manually defined, such that they are similarly strong but feel different
         */

        while (currentWave < waveAmount)
        {
            int waveHp = currentLevelWaves[currentWave];
            StartCoroutine(SpawnSingleWave(waveHp));
            currentWave++;

            yield return new WaitForSeconds(_waveDelay);
            /* Spawn groups in regular intervals until the wave is over
            float waveSpawnedHp = 0; 
            int waveHp = currentLevelWaves[currentWaveNr]; // max HP for this wave

            while ((waveSpawnedHp < waveHp))
            {

                //spawnedEnemies.Add(SpawnGroup(5, ChooseSpawnpoint(), _enemyPrefab)); // TODO replace this with SpawnGroupSelector once implemented
                SpawnGroup(memberAmount:_groupSize, memberHp:enemyHp, spawnPoint:ChooseSpawnpoint(), objectToSpawn:_enemyPrefab);
                waveSpawnedHp += enemyHp * _groupSize;
                Debug.Log("spawned hp:" + waveSpawnedHp);
                Debug.Log("wave" + currentWaveNr + "of " + currentLevelWaves.Length);
                Debug.Log("Enemies Remaining: " + GameControl.control.enemiesRemaining);

                yield return new WaitForSeconds(_groupDelay); // delay between groups
            }

            Debug.Log("wave over");
            currentWaveNr++;
            yield return new WaitForSeconds(_groupDelay*2); // delay between waves
            */
        }

        

        while (GameControl.control.enemiesRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        GameControl.control.OnFightWin();
        yield return null;
    }

    private IEnumerator SpawnSingleWave(int waveHp)
    {
        // Spawn groups in regular intervals until the wave is over
        float waveSpawnedHp = 0;
        float memberHp = enemyBehaviour.maxHp;

        while ((waveSpawnedHp < waveHp))
        {
            SpawnGroup(memberAmount: _groupSize, spawnPoint: ChooseSpawnpoint(), objectToSpawn: _enemyPrefab);
            waveSpawnedHp += memberHp * _groupSize;

            yield return new WaitForSeconds(_groupDelay);
        }

        Debug.Log("wave over");        
        yield return new WaitForSeconds(_waveDelay); 
    }
}