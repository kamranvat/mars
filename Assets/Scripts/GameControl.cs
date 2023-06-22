using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Android;

public class GameControl : MonoBehaviour
{
    // There can be only one
    public static GameControl control;

    public float hp;
    public float shield;
    public float resources;
    public int intel;

    public bool alive = true;

    // Level stats:
    public int currentLevel = 1;
    public int enemyHp; //needs to be set in the spawner
    public int waveAmount = GameControl.control.currentLevel / 2 + 1;
    public int[] currentLevelWaves = waveHealthDistribution(waveAmount, _levelTotalHp);
    public int currentWaveNr = 0;

    void Awake()
    {
        // Create this only if it does not exist
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control == this)
        {
            Destroy(gameObject);
        }
        
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 30), "Health: " + hp);

        if(GUI.Button(new Rect(10,100, 100, 30), "Save"))
        {
            GameControl.control.Save();
        }

        if (GUI.Button(new Rect(10, 150, 100, 30), "´Load"))
        {
            GameControl.control.Load();
        }
    }

    public void Save()
    {
        // TODO modify to save at end of each level
        // TODO maybe add different saves and a main menu
        // Save the information in this script to the device
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.hp = hp;
        data.shield = shield;
        data.resources = resources;
        data.intel = intel;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
         if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            hp = data.hp;
            shield = data.shield;
            resources = data.resources; 
            intel = data.intel;
        }
    }

    public float Shield(float damage, float bypass, float shieldHp)
    {
        // Takes the damage and bypass, updates public shield hp
        // Returns how much hp damage the player takes after shielding

        float shieldBypass = damage * bypass;
        float shieldDamage = damage - shieldBypass;
        float playerDamage = shieldBypass;
        // TODO reset shield damage timer here?

        if (shieldDamage >= shieldHp) 
        {
            shield = 0;
            // TODO reset recharge timer
            playerDamage += shieldDamage - shieldHp;
        }
        else if (shieldDamage < shieldHp) 
        { 
            shield = shieldHp - shieldDamage;
        }

        return playerDamage;
    }

    public void DamagePlayer(float damage, float bypass)
    {
        hp -= Shield(damage, bypass, shield);
        if (hp < 0)
        {
            hp = 0;
            PlayerDeath();
            Debug.Log("Player death");
        }
    }

    public void StartLevel()
    {
        // All the things that are needed at start:
        // reset hp
        // reset shield
        // call upgrade menu thing maybe?
        // maybe just make that another scene
        // placeholder for now
        Debug.Log("UPGRADE MENU PLACEHOLDER ");
        // CHANGE SCENE HERE (maybe separate fct)

    }
    public void OnLevelWin()
    {
        // TODO implement "level won screen"
        // with a START LEVEL N+1 button
        currentLevel++;
        StartLevel();
    }

    public void RestartLevel()
    {
        // TODO implement as follows:
        // show a screen with two options
        // START LEVEL N / RETURN TO MAIN MENU
        Debug.Log("Restart Level called.")
    }

    public void PlayerDeath() 
        //TODO Implement
    {
        Debug.Log("Skill issue");
        RestartLevel();
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
}


[Serializable]
class PlayerData
{
    // TODO .get this stuff and such, data security wise (look up)
    public float hp;
    public float shield;
    public float resources;
    public int intel;
    public int currentLevel;
}