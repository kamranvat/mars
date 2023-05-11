using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    // VARIABLES
    GameObject[] attachedObjects = new GameObject[3];

    private GameObject[] attachedObjectTypes; // Your pool of 6 object types to apply to the wall

    private Vector2[] slots =
        {
            new Vector2(0, 1),
            new Vector2(1, -1),
            new Vector2(-1, -1),
        };// The position the objects are placed


    // -- lives --
    private int _lives = 5;




    void Start()
    {

    }

    void Update()
    {
    }



    public void Damage()
    {
        //damage the player on enemy contact
        _lives--;
        Debug.Log(message: " D A M A G E " + _lives);

        // update UI

        // change color on damage

        // player death
        if (_lives == 0)
        {
            //foreach (Transform child in SpawnManager.transform)
            //{
            //    Destroy(child.gameObject);
            //}

            Debug.Log(message: "ded");

            //SpawnManager.GetComponent<SpawnManager>().onPlayerDeath();
        }
    }



    // Randomly populate a wall
    void randomAttach()
    {
        for (int i = 0; i < attachedObjects.Length; i++)
        {
            // Instantiate a random selection from the 6 types to predefined locations on the wall
            int randomType = Random.Range(0, attachedObjectTypes.Length);
            AttachToPlanet(i, attachedObjectTypes[randomType]);
        }
    }

    // Add a specified object to the wall at a specified position
    void AttachToPlanet(int position, GameObject objToPlace)
    {
        attachedObjects[position] = Instantiate(objToPlace, new Vector3(slots[position].x, slots[position].y, 0), Quaternion.identity);
    }
}