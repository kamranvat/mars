using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyBehaviour EnemyBehaviour;

    //for now, just spawn based on a delay. 
    //later, add delay = 1/rate and an amount 
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private float _delay = 1f;
    [SerializeField]
    private bool _dead = false;

    [SerializeField] // for debugging
    public int _currentLevel = 1;

    [SerializeField]
    private int _levelTotalHp = 1000;

    [SerializeField]
    private int _currentWave = 1;
     

    // Start is called before the first frame update
    void Start()
    {

        // for some reason, I cannot get values from other scripts. Using a dummy value for now
        // TODO: find out how this was done in the tutorial and where they hid the info
        //EnemyBehaviour = GameObject.Find("Enemy1").GetComponent<EnemyBehaviour>();
        //int enemyHp = EnemyBehaviour._hp;
        //Debug.Log("YOYOYO" + enemyHp);


        StartCoroutine(SpawnSystem());
    }

    // Update is called once per frame
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
        // Have another fct that decides which enemies  are "active" based on the level.
        // Pass the list of active enemies (string[]) in here, choose one at random,
        // and call spawnGroup with the right parameters 
    }

    private void spawnGroup(int memberAmount, Vector2 spawnPoint, GameObject objectToSpawn)
    {
        float dist = 5f;

        // TODO: make random velocity, assign to each member  within the for loop
        //give each group a (similar between members) initial velocity parallel to the circle defined
        //by the distance d to the center, d such that it is out of view
        for (int i = 0; i <= memberAmount; i++)
        {
            // using unityengine.random to clarify between this and system.random
            // Spawn at spawnpoint plus random offset
            Vector2 position = spawnPoint + new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * dist;
            Instantiate(objectToSpawn, position, Quaternion.identity);
        }
;
    }

    private Vector2 chooseSpawnpoint()
    {
        //TODO make it choose one spawnpoint at random

        /*Vector2[] spawnPointArray = new Vector2[4];

        spawnPointArray[0] = new Vector2(8, 4);
        spawnPointArray[1] = new Vector2(-8, 4);
        spawnPointArray[2] = new Vector2(8, -4);
        spawnPointArray[3] = new Vector2(-8, -4);*/

        return new Vector2(8, 4);
    }

    private bool isWaveAlive()
    {
        // should only be false if the last group has been spawned and all enemies are dead
        return true;
    }

    private bool isLevelActive() 
    { 
        // should return true if either the player dies or the last wave of a level is dead
        return true;
    }


    IEnumerator SpawnSystem()
    {

        int enemyHp = 20;
        int waveAmount = _currentLevel / 2 + 1;
        int[] currentLevelWaves = waveHealthDistribution(5, _levelTotalHp); // 5 for debugging, waveAmount otherwíse

        Debug.Log(currentLevelWaves[0]);
       

        //also have different types of groups later to spawn different swarms
        // TODO group selector fct that selects the spawn group and spawns it with the right vals

        // IN CONCLUSION 
        // what I want is:
        /* For each level spawn level/2 waves
         *      for each wave spawn x groups
         *              each group consists of z members and spawns together
         * x is based on total hp
         * groups are manually defined, such that they are similarly strong but feel different
         */

        while (!_dead)
        {
            int totalHp = 0;
            int maxHp = 1000; // add waveHealthDIst fct here

            // Spawn a group and track how much hp they had
            spawnGroup(5, chooseSpawnpoint(), _enemyPrefab);
            int spawned_hp = totalHp + enemyHp;
            Debug.Log(spawned_hp);

            yield return new WaitForSeconds(_delay);

        }

        yield return null;

    }

    IEnumerator LevelSystem()
    {
        // current level stats
        int enemyHp = 20;
        int waveAmount = _currentLevel / 2 + 1;
        int[] currentLevelWaves = waveHealthDistribution(waveAmount, _levelTotalHp); // 5 for debugging, waveAmount otherwíse
        int currentWaveNr = 0;
        bool levelActive = isLevelActive();

        bool levelWon = false;
        bool levelLost = false;
        
        Debug.Log(currentLevelWaves[0] + _currentLevel);


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

        // Make the waves
        while (levelActive)
        {

            if (levelWon)
            {
                // on level win:
                _currentLevel += 1;
                levelActive = false;
                Debug.Log(message: "level won");
            }

            if (levelLost)
            {
                // on level loss:
                levelActive = false;
                Debug.Log(message: "level lost");
            } // honestly i think I should check for player death in update() and stop calling the subroutines here if it happens

            while ((currentWaveNr <= currentLevelWaves.Length) && !levelLost)
            {
                
                // Spawn groups in regular intervals until the wave is over
                int spawnedHp = 0; 
                int waveHp = currentLevelWaves[currentWaveNr]; // max HP for this wave

                while ((spawnedHp < waveHp) && !levelLost)
                {
                    spawnGroup(5, chooseSpawnpoint(), _enemyPrefab); // TODO replace this with spawnGroupSelector once implemented
                    int spawned_hp = spawnedHp + enemyHp;
                    Debug.Log(spawned_hp);
                    yield return new WaitForSeconds(_delay);
                }

                // Monitor the wave - the last enemies should be on the map now
                // while(true){check if enemies alive} -> as soon as not, or 30s pass, levelWon = true
                // If the wave dies -> wait, then next wave
                // If the wave dies && it was the last wave -> levelWon
            }

        }


        
        while (!_dead)
        {


        }

        yield return null;

    }

}

