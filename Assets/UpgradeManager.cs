using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private int[] turretLevels = new int[11];
    private int shieldHpLevel;
    private int shieldRechargeLevel;
    private int playerHpLevel;
    private int phobosSiloLevel;
    private int deimosSiloLevel;

    private int _maxLevel = 2;

    [SerializeField]
    private List<TurretSlot> turretSlotList = new List<TurretSlot>(); // List of turret slots
    [SerializeField]
    private List<TurretData> turretDataList = new List<TurretData>(); // List of turret data

    // should contain for each upgradable thing a getlevel funtion, an upgrade function and a ismaxed function
    // should also contain a "setAllUpgrades" function that handles the calls to TurretManager and gameControl

    // note on terminology: turret levels are referred to as such, whereas all other upgrades are just called upgrades 
    // (e.g. SetPlayerUpgrades does not set turrets)

    // Start is called before the first frame update
    void Start()
    {

    }


    public void SetPlayerUpgrades()
    {
        // Makes all the calls to instantiate all upgrades 
        // Set turrets:

        // Set shieldHp:

        // Set shield recharge stats:

        // Set player hp:

        // Set moon turrets:
    }

    public void GetPlayerUpgrades()
    {

    }
    public int[] GetTurretLevels()
    {
        return turretLevels;
    }

    public int GetStatLevel(string name)
    {
        return name switch
        {
            "shieldHpLevel" => shieldHpLevel,
            "shieldRechargeLevel" => shieldRechargeLevel,
            "playerHpLevel" => playerHpLevel,
            "phobosSiloLevel" => phobosSiloLevel,
            "deimosSiloLevel" => deimosSiloLevel,
            _ => 0,
        };
    }

    public bool CanTurretLevelUp(int id)
    {
        return (turretLevels[id] < _maxLevel);
    }

    public bool CanStatUpgrade(string name) 
    {
        return name switch
        {
            "shieldHpLevel" => (shieldHpLevel < _maxLevel),
            "shieldRechargeLevel" => (shieldRechargeLevel < _maxLevel),
            "playerHpLevel" => (playerHpLevel < _maxLevel),
            "phobosSiloLevel" => (phobosSiloLevel < _maxLevel),
            "deimosSiloLevel" => (deimosSiloLevel < _maxLevel),
            _ => false,
        };
    }


    public void UpgradeTurret(int id)
    {
        turretLevels[id]++;
    }
       

    public void UpgradeStat(string name)
    {
        switch (name)
        {
            case "shieldHpLevel":
                shieldHpLevel++;
                break;

            case "shieldRechargeLevel":
                shieldRechargeLevel++;
                break;

            case "playerHpLevel":
                playerHpLevel++;
                break;

            case "phobosSiloLevel":
                phobosSiloLevel++;
                break;

            case "deimosSiloLevel":
                deimosSiloLevel++;
                break;
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

