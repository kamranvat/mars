using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    // There can be only one
    public static GameControl control;

    public float maxPlayerHp;
    public float playerHp;
    public float maxShielpHp;
    public float shieldHp;
    public float shieldRechargeDelay;
    public float shieldRechargeRate;
    public float resources;
    public int intel;

    public bool isPlayerAlive = true;

    // Level stats:
    public int currentLevel = 1;
    public int enemiesRemaining;

    private float shieldRechargeTimer;


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

    private void Start()
    {
        shieldRechargeTimer = shieldRechargeDelay;
    }

    private void Update()
    {
        if (shieldRechargeTimer > 0f)
        {
            shieldRechargeTimer -= Time.deltaTime;
        }
        else if (!IsShieldFullyCharged())
        {
            ChargeShield();
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 30), "Health: " + playerHp);

        if(GUI.Button(new Rect(10,100, 100, 30), "Save"))
        {
            GameControl.control.Save();
        }

        if (GUI.Button(new Rect(10, 150, 100, 30), "Load"))
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
        data.hp = playerHp;
        data.shield = shieldHp;
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

            playerHp = data.hp;
            shieldHp = data.shield;
            resources = data.resources; 
            intel = data.intel;
        }
    }

    public float Shield(Tuple<float, float, bool> damageTuple)
    {

        float damage = damageTuple.Item1;
        float bypass = damageTuple.Item2;
        bool emp = damageTuple.Item3;

        // Takes the damage and bypass, updates public playerShield
        // Returns how much playerHp damage the player takes after shielding
        float bypassAmount = damage * bypass;
        float shieldDamage = damage - bypassAmount;
        float playerDamage = bypassAmount;

        // TODO reset playerShield damage timer here?

        if (emp)
        {
            // EMP only damages shields
            shieldHp -= damage;

            if (shieldHp <= 0)
            {
                shieldHp = 0;
            }

            return 0;

        }
        else
        {
            if (shieldDamage >= shieldHp)
            {
                shieldHp = 0;
                // TODO reset recharge timer
                playerDamage += shieldDamage - shieldHp;
            }
            else if (shieldDamage < shieldHp)
            {
                shieldHp = shieldHp - shieldDamage;
            }

            return playerDamage;
        }
        
    }

    public void ChargeShield()
    {
        shieldHp += shieldRechargeRate * Time.deltaTime;

        // Clamp shieldHp to maximum value
        shieldHp = Mathf.Clamp(shieldHp, 0f, maxShielpHp);
    }

    public bool IsShieldFullyCharged()
    {
        return shieldHp >= maxShielpHp;
    }

    public void DamagePlayer(Tuple<float,float,bool> statsTuple)
    {
        playerHp -= Shield(statsTuple);
        if (playerHp < 0)
        {
            playerHp = 0;
            OnPlayerDeath();
        }
    }

    public void StartLevel()
    {
        // All the things that are needed at start:
        // reset playerHp
        // reset playerShield
        // call upgrade menu thing maybe?
        // maybe just make that another scene
        // placeholder for now
        Debug.Log("UPGRADE MENU PLACEHOLDER ");
        // CHANGE SCENE HERE (maybe separate fct)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        playerHp = maxPlayerHp;
        shieldHp = maxShielpHp;

    }

    public void CollectResource()
    {
        StartCoroutine(IncrementResourceWithDelay(10));
    }

    private IEnumerator IncrementResourceWithDelay(int amount)
    {
        float delay = 0.02f;
     
        for (int i = 0; i < amount; i++)
        {
            resources++; 
            yield return new WaitForSeconds(delay);
        }
    }

    public void CollectIntel()
    {
        intel++;
    }
    public void OnLevelWin()
    {
        // TODO implement "level won screen"
        // with a START LEVEL N+1 button
        Debug.Log("On Level Win called.");
        currentLevel++;
        StartLevel();
    }

    public void RestartLevel()
    {
        // TODO implement as follows:
        // show a screen with two options
        // START LEVEL N / RETURN TO MAIN MENU
        // Note: this gets called a bunch of times

        // Reset stuff:
        isPlayerAlive = true;
    }

    public void OnPlayerDeath() 
        //TODO Implement
    {
        //Debug.Log("Skill issue");
        isPlayerAlive = false;
        RestartLevel();
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