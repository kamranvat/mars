using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    //for now, just spawn based on a delay. 
    //later, add delay = 1/rate and an amount 
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private float _delay = 1f;
    [SerializeField]
    private bool _dead = false;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnSystem());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator SpawnSystem()
    {
        //generate six spawnpoints where enemies can come from, pass which spawnpoint 
        //i want the current group to come from into here.
        //also make the wave spawn in groups and keep count of the total number
        //also have different types of groups later to spawn different swarms

        while (!_dead)
        {
            //spawn enemies
            //instantiation with prefab, pos, rot and parent transform 
            Instantiate(_enemyPrefab, new Vector3(Random.Range(-3f, 3f), 10), Quaternion.identity);
            yield return new WaitForSeconds(_delay);

        }

        yield return null;

    }

}

