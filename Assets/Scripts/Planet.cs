using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    // VARIABLES


    // -- lives --
    private int _lives = 5;




    void Start()
    {

    }

    void Update()
    {
        //PlayerMovement();
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
}