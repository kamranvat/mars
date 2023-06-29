using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    [SerializeField] 
    private List<TurretSlot> turretSlotList = new List<TurretSlot>(); // List of turret slots
    [SerializeField]
    private List<TurretData> turretDataList = new List<TurretData>(); // List of turret data



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

