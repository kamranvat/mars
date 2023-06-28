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

    // Camera zoom for upgrade phase
    public CameraZoomController zoomController;
    private Vector2 _upgradeZoomPosition = new Vector2(5f, 0f);
    private float _upgradeZoomLevel = 5f;

    // Turrets to save game state
    private TurretManager turretManager;

    // Player stats
    public float maxPlayerHp;
    public float playerHp;
    public float maxShielpHp;
    public float shieldHp;
    public float shieldRechargeDelay;
    public float shieldRechargeRate;

    // Inventory
    public float resources;
    public int intel;


    // Each level has three phases
    public enum LevelPhase
    {
        Upgrade,
        Fight,
        Outro
    }

    // Level info
    public int currentLevel = 1;
    public int enemiesRemaining;
    private LevelPhase currentLevelPhase;
    //private bool isPhaseDone = false;

    // Functional
    public bool isPlayerAlive = true;
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

        zoomController = FindObjectOfType<CameraZoomController>();
    }

    private void Update()
    {
        // Shield recharge:
        if (shieldRechargeTimer > 0f)
        {
            shieldRechargeTimer -= Time.deltaTime;
        }
        else if (!IsShieldFullyCharged() && isPlayerAlive)
        {
            ChargeShield();
        }

        // Camera zoom:
        if (Input.GetKeyDown(KeyCode.Space))
        {
            zoomController.ZoomIn(_upgradeZoomLevel, _upgradeZoomPosition);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            zoomController.ZoomOut();
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 30), "Health: " + playerHp);

        if(GUI.Button(new Rect(10,100, 100, 30), "Save"))
        {
            Save();
        }

        if (GUI.Button(new Rect(10, 150, 100, 30), "Load"))
        {
            Load();
        }
    }

    public void Save()
    {
        // Save the information in this script to the device
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.Turrets = turretManager.GetTurrets();
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

            turretManager.SetTurrets(data.Turrets);
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

        // Taking damage resets the timer
        shieldRechargeTimer = shieldRechargeDelay;

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
        if (isPlayerAlive) 
        {
            playerHp -= Shield(statsTuple);
        }
           
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
        
        // CHANGE SCENE HERE (maybe separate fct)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // call upgrade menu thing maybe?
        // maybe just make that another scene
        // placeholder for now
        currentLevelPhase = LevelPhase.Upgrade;
        Debug.Log("UPGRADE MENU PLACEHOLDER ");

        playerHp = maxPlayerHp;
        shieldHp = maxShielpHp;
        shieldRechargeTimer = shieldRechargeDelay;

        StartPhase(currentLevelPhase);

    }

    private void StartPhase(LevelPhase phase)
    {
        // Perform actions specific to the given phase
        switch (phase)
        {
            case LevelPhase.Upgrade:
                // LOAD TURRET LIST HERE, SET TURRETS TO LIST
                Load();
                Debug.Log("Loaded");
                zoomController.ZoomIn(_upgradeZoomLevel, _upgradeZoomPosition);
                break;
            case LevelPhase.Fight:
                // Start the fight phase
                break;
            case LevelPhase.Outro:
                // Start the outro phase
                // set resourcegravity to high
                break;
        }
    }

    private void EndPhase(LevelPhase phase)
    {
        // Perform actions specific to the given phase at its end
        switch (phase)
        {
            case LevelPhase.Upgrade:
                // GRAB UPDATED TURRET LIST; SET TURRETS TO LIST
                zoomController.ZoomOut();
                break;
            case LevelPhase.Fight:
                // Fade out fight HUD
                break;
            case LevelPhase.Outro:
                // Show level won screen
                // set resourcegravity back to normal, destroy all remaining resources
                Save();
                Debug.Log("Saved game");
                break;
        }
    }

    private void AdvanceToNextPhase()
    {
        // End the current phase
        EndPhase(currentLevelPhase);

        // Determine the next phase
        switch (currentLevelPhase)
        {
            case LevelPhase.Upgrade:
                currentLevelPhase = LevelPhase.Fight;
                break;
            case LevelPhase.Fight:
                currentLevelPhase = LevelPhase.Outro;
                break;
            case LevelPhase.Outro:
                Debug.Log("Level " + currentLevel + " done.");
                currentLevel++;
                break;
        }
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

    public void OnFightWin()
    {
        // TODO implement "level won screen"
        // with a START LEVEL N+1 button
        Debug.Log("On Fight Win called.");
        AdvanceToNextPhase();
    }

    public void RestartLevel()
    {
        // TODO implement as follows:
        // show a screen with two options
        // START LEVEL N / RETURN TO MAIN MENU
        // Note: this gets called a bunch of times
        Debug.Log("Restarting");
        // Reset stuff:
        isPlayerAlive = true;
        StartLevel();
    }

    public void OnPlayerDeath() 
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
    public List<TurretData> Turrets;
    public float resources;
    public int intel;
    public int currentLevel;
}