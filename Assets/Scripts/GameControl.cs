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
    public static GameControl Instance;

    // Show start screen on first start
    private bool firstStart = true;
    public bool lastLevel = false;

    // Camera zoom for upgrade phase
    public CameraZoomController zoomController;
    private Vector2 _upgradeZoomPosition = new Vector2(5f, 0f);
    private float _upgradeZoomLevel = 5f;

    // UI elements
    public Canvas upgradeMenu;
    public Canvas welcomeMenu;
    public Canvas youDiedMenu;
    public Canvas youWonMenu;
    public Canvas gameWonMenu;

    // Turrets to save game state
    private TurretManager turretManager;

    // EnemySpawner to Instance start of fight phase
    private EnemySpawner enemySpawner;

    // Player stats
    public float maxPlayerHp;
    public float playerHp;
    public float maxShielpHp;
    public float shieldHp;
    public float shieldRechargeDelay;
    public float shieldRechargeRate;

    // Health and shield bar
    [SerializeField] private Healthbar healthbar;
    [SerializeField] private Healthbar shieldbar;

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
    public int currentLevel = 0;
    public int enemiesRemaining;
    private LevelPhase currentLevelPhase;

    // Functional
    public bool isPlayerAlive = true;
    private float shieldRechargeTimer;
    public float gravityStrength = 1; // For collectables


    private void Awake()
    {
        // Create this only if it does not exist
        if (Instance != null && Instance != this)  
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        zoomController = FindObjectOfType<CameraZoomController>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        turretManager = FindObjectOfType<TurretManager>();

        healthbar.UpdateHealthBar(maxPlayerHp, playerHp);
        shieldbar.UpdateHealthBar(maxShielpHp, shieldHp);
    }

    private void Start()
    {
        Debug.Log("gamecontrol started");

        HideCanvas(gameWonMenu);
        HideCanvas(youWonMenu);
        HideCanvas(youDiedMenu);
        HideCanvas(welcomeMenu);
        HideCanvas(upgradeMenu);
        StartLevel();
    }

    private void Update()
    {
        // Shield recharge
        if (shieldRechargeTimer > 0f)
        {
            shieldRechargeTimer -= Time.deltaTime;
        }
        else if (!IsShieldFullyCharged() && isPlayerAlive)
        {
            ChargeShield();
        }

        // Update UI
        healthbar.UpdateHealthBar(maxPlayerHp, playerHp);
        shieldbar.UpdateHealthBar(maxShielpHp, shieldHp);

        // Camera zoom
        if (Input.GetKeyDown(KeyCode.Space))
        {
            zoomController.ZoomIn(_upgradeZoomLevel, _upgradeZoomPosition);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            zoomController.ZoomOut();
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
        playerHp = maxPlayerHp;
        shieldHp = maxShielpHp;
        shieldRechargeTimer = shieldRechargeDelay;

        currentLevelPhase = LevelPhase.Upgrade;
        StartPhase(currentLevelPhase);

    }

    private void StartPhase(LevelPhase phase)
    {
        // Perform actions specific to the given phase
        switch (phase)
        {
            case LevelPhase.Upgrade:
                Load();
                zoomController.ZoomIn(_upgradeZoomLevel, _upgradeZoomPosition);

                if (firstStart)
                {
                    resources = 0;
                    firstStart = false;
                    ShowCanvas(welcomeMenu);
                }
                else
                {
                    ShowCanvas(upgradeMenu);
                }                         
                break;

            case LevelPhase.Fight:
                gravityStrength = 1;
                enemySpawner.StartSpawning();
                
                break;

            case LevelPhase.Outro:

                if (lastLevel)
                {
                    ShowCanvas(gameWonMenu);
                } 
                else
                {
                    gravityStrength = 20;
                    ShowCanvas(youWonMenu);
                }
                
                break;
        }
    }

    private void EndPhase(LevelPhase phase)
    {
        // Perform actions specific to the given phase at its end
        switch (phase)
        {
            case LevelPhase.Upgrade:
                HideCanvas(welcomeMenu);
                HideCanvas(upgradeMenu);
                zoomController.ZoomOut();
                break;

            case LevelPhase.Fight:
                break;

            case LevelPhase.Outro:
                Save();
                HideCanvas(youWonMenu);
                break;
        }
    }

    public void AdvanceToNextPhase()
    {
        // End the current phase
        EndPhase(currentLevelPhase);

        // Determine the next phase
        switch (currentLevelPhase)
        {
            case LevelPhase.Upgrade:
                Debug.Log("Advance to fight phase");
                currentLevelPhase = LevelPhase.Fight;
                break;
            case LevelPhase.Fight:
                Debug.Log("Advance to outro phase");
                currentLevelPhase = LevelPhase.Outro;
                break;
            case LevelPhase.Outro:
                Debug.Log("Level " + currentLevel + " done.");
                if(!lastLevel)
                {
                    currentLevel++;
                    currentLevelPhase = LevelPhase.Upgrade;
                }
                else
                {
                    Application.Quit();
                }
                
                break;
        }

        // Start the next phase
        StartPhase(currentLevelPhase);
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
        Debug.Log("On Fight Win called.");
        AdvanceToNextPhase();
    }

    public void RestartLevel()
    {
        ClearEnemies();
        HideCanvas(youDiedMenu);
        isPlayerAlive = true;
        StartLevel();
    }

    public void OnPlayerDeath() 
    {
        isPlayerAlive = false;
        enemySpawner.StopSpawning();
        youDiedMenu.enabled = true;
        ShowCanvas(youDiedMenu);
    }

    public void ClearEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
    public void ShowCanvas(Canvas canvas)
    {
        canvas.enabled = true;
    }

    public void HideCanvas(Canvas canvas)
    {
        canvas.enabled = false;
    }

    public void QuitGame()
    {
        Save();
        Application.Quit();
    }
}


[Serializable]
class PlayerData
{
    public List<TurretData> Turrets;
    public float resources;
    public int intel;
    public int currentLevel;
}