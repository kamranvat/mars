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
    public int _level = 1;

    [SerializeField]
    private int _level_hp = 1000;

    [SerializeField]
    private int currentWave = 1;
     

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

    private Array waveHealthDistribution(int n, int level_hp)
    {
        // Returns an array of values that sum up to 100, representing the total hp of each wave in this level.
        // Values are taken from a linear function and normalised such that each wave is stronger than the previous

        int[] distr = new int[n];
        float step = 1f / (n - 1); // Difference between each element

        float t = 0f;

        Debug.Log(message: "Wave health init...");

        for (int i = 0; i < n; i++)
        {
            float value = Mathf.Lerp(0f, level_hp, t); // interpolate value between 0 and total
            distr[i] = Mathf.RoundToInt(value); // round to integer
            level_hp -= distr[i]; // subtract from total
            t += step; // increment interpolation value
        }

        // handle rounding error by adding remaining value to last element
        distr[n - 1] += level_hp;

        Debug.Log(message: "Wave health initialized. Values: " + distr);
        return distr;

    }

    private void spawnGroup(int memberAmount, Vector2 spawnPoint, GameObject objectToSpawn)
    {
        float dist = 1f;

        // TODO: make random velocity, assign to each member  within the for loop
        //give each group a (similar between members) initial velocity parallel to the circle defined
        //by the distance d to the center, d such that it is out of view
        Debug.Log("...");
        for (int i = 0; i <= memberAmount; i++)
        {
            // using unityengine.random to clarify between this and system.random
            // Spawn at spawnpoint plus random offset
            Vector2 position = spawnPoint + new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * dist;
            Instantiate(objectToSpawn, position, Quaternion.identity);
            Debug.Log(".!!.");

            Debug.Log("!!!!");
        }
;
    }
    IEnumerator SpawnSystem()
    {
        //generate six spawnpoints where enemies can come from, pass which spawnpoint 
        //i want the current group to come from into here.
        // for now, just hardcode a few of them in here.
        //TODO make a dedicated fct that returns one random spawnpoint
        int enemyHp = 20;
        Vector2[] spawnPointArray = new Vector2[4];

        spawnPointArray[0] = new Vector2(8, 4);
        spawnPointArray[1] = new Vector2(-8, 4);
        spawnPointArray[2] = new Vector2(8, -4);
        spawnPointArray[3] = new Vector2(-8, -4);
       

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
            Debug.Log("spawning..." + _enemyPrefab);
            spawnGroup(5, spawnPointArray[0], _enemyPrefab);
            int spawned_hp = totalHp + enemyHp;
            Debug.Log(spawned_hp);

            yield return new WaitForSeconds(_delay);

        }

        yield return null;

    }

}

