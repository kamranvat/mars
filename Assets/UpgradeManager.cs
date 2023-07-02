using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private int[] turretLevels = new int[9];
    private int shieldHpLevel;
    private int shieldRechargeLevel;
    private int playerHpLevel;
    private int phobosSiloLevel;
    private int deimosSiloLevel;

    private int _maxLevel = 2;

    [SerializeField]
    private List<TurretSlot> turretSlotList = new List<TurretSlot>(); // List of turret slots
    [SerializeField]
    private Planet planet;


    // should contain for each upgradable thing a getlevel funtion, an upgrade function and a ismaxed function
    // should also contain a "setAllUpgrades" function that handles the calls to TurretManager and gameControl

    // note on terminology: turret levels are referred to as such, whereas all other upgrades are just called upgrades 
    // (e.g. SetPlayerUpgrades does not set turrets)


    void Awake()
    {
        // Create this only if it does not exist yet
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        planet = planet.GetComponent<Planet>();
        Debug.Log(planet);
    }


    public void SetPlayerUpgrades()
    {

        // Set turrets:
        planet.SetTurretLevels(turretLevels);

        // Set shieldHp:
        GameControl.Instance.maxShielpHp = GetStatLevel("ShieldCap") * 100;
        // Set shield recharge stats:
        GameControl.Instance.maxShielpHp = GetStatLevel("ShieldGen") * 10;
        // Set player hp:
        GameControl.Instance.maxShielpHp = GetStatLevel("Health") * 100;
        // Set moon turrets:
        // removed
    }

    public void GetPlayerUpgrades()
    {

    }
    public int[] GetTurretLevels()
    {
        return turretLevels;
    }

    public int GetStatLevel(string id)
    {
        return id switch
        {
            "ShieldCap" => shieldHpLevel,
            "ShieldGen" => shieldRechargeLevel,
            "Health" => playerHpLevel,
            "Phobos" => phobosSiloLevel,
            "Deimos" => deimosSiloLevel,
            _ => 0,
        };
    }

    public bool CanStatUpgrade(string id) 
    {
        return id switch
        {
            "ShieldCap" => (shieldHpLevel < _maxLevel && GameControl.Instance.resources >= 200),
            "ShieldGen" => (shieldRechargeLevel < _maxLevel && GameControl.Instance.resources >= 200),
            "Health" => (playerHpLevel < _maxLevel && GameControl.Instance.resources >= 200),
            //"Phobos" => (phobosSiloLevel < _maxLevel && GameControl.Instance.resources >= 200),
            //"Deimos" => (deimosSiloLevel < _maxLevel && GameControl.Instance.resources >= 200),
            "Phobos" => (false),
            "Deimos" => (false), // removed
            _ => false,
        };
    }

    public bool CanStatUpgrade(int id)
    {
        // For turrets
        return (turretLevels[id] < _maxLevel && GameControl.Instance.resources >= 200);
    }


    public void UpgradeTurret(int id)
    {
        turretLevels[id]++;
        GameControl.Instance.resources -= 200;
    }
       

    public void UpgradeStat(string turretTitle)
    {
        switch (GetTurretId(turretTitle))
        {
            case "ShieldCap":
                shieldHpLevel++;
                GameControl.Instance.resources -= 200;
                break;

            case "ShieldGen":
                shieldRechargeLevel++;
                GameControl.Instance.resources -= 200;
                break;

            case "Health":
                playerHpLevel++;
                GameControl.Instance.resources -= 200;
                break;

            case "Phobos":
                phobosSiloLevel++;
                GameControl.Instance.resources -= 200;
                break;

            case "Deimos":
                deimosSiloLevel++;
                GameControl.Instance.resources -= 200;
                break;
        }
    }

    public string GetTurretId(string turretTitle)
    {
        // Convert from title to id 
        switch (turretTitle)
        {
            case "Maintenance Network":
                return "Health";
            case "Shield Generator":
                return "ShieldGen";
            case "Shield Capacitor":
                return "ShieldCap";
            default:
                return string.Empty;
        }
    }



    // Get the list of turrets for UI or saving game state
    public List<TurretData> GetTurrets()
    {
        List<TurretData> turrets = new List<TurretData>();

        foreach (TurretSlot slot in turretSlotList)
        {
            turrets.Add(slot.GetTurretData());
        }

        return turrets;
    }

    // Set all turrets according to a list 
    public void SetTurrets(List<TurretData> turretDataList)
    {
        for (int i = 0; i < turretDataList.Count; i++)
        {
            if (turretDataList[i] != null)
            {
                turretSlotList[i].SetTurretData(turretDataList[i]);
                turretSlotList[i].InstantiateTurret();
            }
            else
            {
                turretSlotList[i].ClearTurretData();
                turretSlotList[i].InstantiateTurret(); // TODO test if this actually despawns the turret
            }
        }
    }
}

[System.Serializable]
public class TurretData
{
    public TurretType? turretType;
    public int upgradeLevel;
    public float turretValue;
}

public enum TurretType
{
    Normal,
    Special
}

