using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour
{
    // Make singleton
    public static StatsPanel Instance;


    private UpgradeManager upgradeManager;
    private TextMeshProUGUI textMeshPro;

    //[SerializeField] private Button phobosAttack;
    //[SerializeField] private Button deimosAttack;

    [SerializeField] private TextMeshProUGUI turretsTopTextObject;
    [SerializeField] private TextMeshProUGUI turretsRightTextObject;
    [SerializeField] private TextMeshProUGUI turretsLeftTextObject;

    [SerializeField] private TextMeshProUGUI healthTextObject;
    [SerializeField] private TextMeshProUGUI shieldGenTextObject;
    [SerializeField] private TextMeshProUGUI shieldCapTextObject;

    //[SerializeField] private TextMeshProUGUI phobosTextObject;
    //[SerializeField] private TextMeshProUGUI deimosTextObject;

    [SerializeField] private TextMeshProUGUI currentLevelTextObject;
    [SerializeField] private TextMeshProUGUI currentResourcesTextObject;


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

    }

    private void Update()
    {
        // removed moons
        //phobosAttack.onClick.AddListener(OnPhobosAttack);
        //deimosAttack.onClick.AddListener(OnDeimosAttack);

        turretsTopTextObject.text = ("Elysium Array: " + TurretLevelsToString(0));
        turretsRightTextObject.text = ("Cassini Array: " + TurretLevelsToString(3));
        turretsLeftTextObject.text = ("Tharsis Array: " + TurretLevelsToString(6));

        healthTextObject.text = ("Maintenance Network: Level " + UpgradeManager.Instance.GetStatLevel("Health") + " / 3");
        shieldGenTextObject.text = ("Shield Generator: Level " + UpgradeManager.Instance.GetStatLevel("ShieldGen") + " / 3");
        shieldCapTextObject.text = ("Maintenance Network: Level " + UpgradeManager.Instance.GetStatLevel("ShieldCap") + " / 3");

        //phobosTextObject.text = ("Launch from Phobos - Level " + UpgradeManager.Instance.GetStatLevel("Phobos") + " / 3");
        //deimosTextObject.text = ("Launch from Deimos - Level " + UpgradeManager.Instance.GetStatLevel("Deimos") + " / 3");

        currentLevelTextObject.text = ("Playing Level " + GameControl.Instance.currentLevel);
        currentResourcesTextObject.text = ("Resources: " + GameControl.Instance.resources);
    }

    private string TurretLevelsToString(int id_add)
    {
        // Formats three turret levels (beginning at id_add) into a string for displaying
        int[] turretLevels = UpgradeManager.Instance.GetTurretLevels();
        return ((turretLevels[id_add] + 1) + "/3 - " + (turretLevels[id_add + 1] + 1) + "/3 - " + (turretLevels[id_add + 2] + 1) + "/3");
    }

    // removed OnPhobosAttack / OnDeimosAttack

}
