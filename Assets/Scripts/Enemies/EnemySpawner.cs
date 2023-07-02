using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public EnemyBehaviour EnemyBehaviour;
    private IEnumerator spawner;

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _groupSize = 2;
    [SerializeField] private float _groupDelay = 5f;
    [SerializeField] private float _waveDelay = 10f;

    [SerializeField] private int currentWave = 0;

    [SerializeField] private int[] _levelDifficulties = new int[] {};
    [SerializeField] private int levelTotalHp;
    [SerializeField] private int levelSpawnedHp;


    // To get enemy hp
    private EnemyBehaviour enemyBehaviour;

    void Start()
    {
        enemyBehaviour = _enemyPrefab.GetComponent<EnemyBehaviour>();
    }

    private int[] WaveHealthDistribution(int length, int levelHp)
    {
        // Returns an array[length] of values that sums up to levelHp
        // Values are taken from a linear function and normalised such that each wave is stronger than the previous

        int[] distr = new int[length];
        int currentValue = 100;
        int increment = currentValue / 2; 

        for (int i = 0; i < length; i++)
        {
            distr[i] = currentValue;
            currentValue += increment;
        }

        // Scale such that the sum is levelHp
        int currentSum = distr.Sum();
        float scaleFactor = (float)levelHp / currentSum;

        for (int i = 0; i < distr.Length; i++)
        {
            distr[i] = (int)(distr[i] * scaleFactor);
        }

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
        int memberHp = enemyBehaviour.maxHp;

        for (int i = 0; i <= memberAmount; i++)
        {
            // Spawn at spawnpoint plus random offset
            Vector2 position = spawnPoint + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * dist;
            Instantiate(objectToSpawn, position, Quaternion.identity);
            levelSpawnedHp += memberHp;
            GameControl.Instance.enemiesRemaining++;
        }
    }



    private Vector2 ChooseSpawnpoint()
    {
        // Make sure the spawnpoints are off screen
        float xScreenSize = 40;
        float yScreenSize = 25;
        float x, y;
        bool isXInRange = false;
        bool isYInRange = false;

        // Generate random coordinates until the conditions are met
        do
        {
            x = Random.Range(-45f, 45);
            y = Random.Range(-30f, 30);

            if (x > xScreenSize || x < -xScreenSize)
            {
                isXInRange = true;
            }

            if (y > yScreenSize || y < -yScreenSize)
            {
                isYInRange = true;
            }
        } while (!(isXInRange || isYInRange));

        return new Vector2(x, y);
    }

    
    public void StartSpawning()
    {
        levelSpawnedHp = 0;
        levelTotalHp = _levelDifficulties[GameControl.Instance.currentLevel];
        spawner = SpawnWaves();
        StartCoroutine(spawner);

        if (GameControl.Instance.currentLevel == _levelDifficulties.Length-1)
        {
            GameControl.Instance.lastLevel = true;
        }
    }

    public void StopSpawning()
    {
        StopCoroutine(spawner);
    }

    private IEnumerator SpawnWaves()
    {
        int waveAmount = GameControl.Instance.currentLevel / 2 + 1;
        int[] currentLevelWaves = WaveHealthDistribution(length:waveAmount, levelHp:levelTotalHp);
        currentWave = 0;


        while (currentWave < waveAmount)
        {
            int waveHp = currentLevelWaves[currentWave];
            StartCoroutine(SpawnSingleWave(waveHp));
            currentWave++;

            yield return new WaitForSeconds(_waveDelay);
        }      

        while (GameControl.Instance.enemiesRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        if (currentWave == waveAmount)
        {
            GameControl.Instance.OnFightWin();
        }
        
        yield return null;
    }

    private IEnumerator SpawnSingleWave(int waveHp)
    {
        // Spawn groups in regular intervals until the wave is over
        float waveSpawnedHp = 0;
        int memberHp = enemyBehaviour.maxHp;

        while ((waveSpawnedHp < waveHp))
        {
            SpawnGroup(memberAmount: _groupSize, spawnPoint: ChooseSpawnpoint(), objectToSpawn: _enemyPrefab);
            waveSpawnedHp += memberHp * _groupSize;

            yield return new WaitForSeconds(_groupDelay);
        }


        Debug.Log("Single wave, spawned: " + waveSpawnedHp + " of " + waveHp);        
        yield return new WaitForSeconds(_waveDelay); 
    }
}