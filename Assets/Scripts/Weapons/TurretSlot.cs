using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSlot : MonoBehaviour
{
    public Transform attachmentSlot; // Reference to the attachment slot
    public TurretType? turretType; // Type of the turret may be null if empty
    public int upgradeLevel; // Level of turret upgrade
    public float turretValue; // Value of the turret

    [SerializeField]
    private GameObject normalTurretPrefab;
    [SerializeField]
    private GameObject specialTurretPrefab;


    public void InstantiateTurret()
    {
        // Instantiate the turret GameObject and get the Turret component
        GameObject turretPrefab = GetTurretPrefab();
        GameObject turretObject = Instantiate(turretPrefab, attachmentSlot.position, Quaternion.identity, attachmentSlot);
        Turret turret = turretObject.GetComponent<Turret>();

        if (turret != null)
        {
            turret.SetTurretLevel(upgradeLevel);
        }
        
    }


    public TurretData GetTurretData()
    {
        TurretData turretData = new TurretData();
        turretData.turretType = turretType;
        turretData.upgradeLevel = upgradeLevel;
        turretData.turretValue = turretValue;
        return turretData;
    }

    public void SetTurretData(TurretData turretData)
    {
        turretType = turretData.turretType;
        upgradeLevel = turretData.upgradeLevel;
        turretValue = turretData.turretValue;
    }

    public void ClearTurretData()
    {
        turretType = null;
        upgradeLevel = 0;
        turretValue = 0;
    }


    private GameObject GetTurretPrefab()
    {
        // Based on the turret type, return the corresponding turret prefab
        switch (turretType)
        {
            case TurretType.Normal:
                return normalTurretPrefab;
            case TurretType.Special:
                return specialTurretPrefab;
            default:
                return null;
        }
    }
}