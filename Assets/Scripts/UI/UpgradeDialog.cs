using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeDialog : MonoBehaviour
{
    // Make singleton
    public static UpgradeDialog Instance;

    private UpgradeModalWindow upgradeModalWindow;
    private UpgradeManager upgradeManager;

    private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button turretsTop;
    [SerializeField] private Button turretsRight;
    [SerializeField] private Button turretsLeft;
    [SerializeField] private Button health;
    [SerializeField] private Button phobos;
    [SerializeField] private Button deimos;
    [SerializeField] private Button shieldGen;
    [SerializeField] private Button shieldCap;

    [SerializeField] Image turretsTopImage;
    [SerializeField] Image turretsRightImage;
    [SerializeField] Image turretsLeftImage;
    [SerializeField] Image healthImage;
    [SerializeField] Image phobosImage;
    [SerializeField] Image deimosImage;
    [SerializeField] Image shieldGenImage;
    [SerializeField] Image shieldCapImage;

    [SerializeField] Image TurretImage;

    [SerializeField] string turretsTopText;
    [SerializeField] string turretsRightText;
    [SerializeField] string turretsLeftText;
    [SerializeField] string healthText;
    [SerializeField] string phobosText;
    [SerializeField] string deimosText;
    [SerializeField] string shieldGenText;
    [SerializeField] string shieldCapText;

    [SerializeField] string turretText;

    [SerializeField] string turretsTopTitle;
    [SerializeField] string turretsRightTitle;
    [SerializeField] string turretsLeftTitle;
    [SerializeField] string healthTitle;
    [SerializeField] string phobosTitle;
    [SerializeField] string deimosTitle;
    [SerializeField] string shieldGenTitle;
    [SerializeField] string shieldCapTitle;

    [SerializeField] string turretTitle;

    


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
        turretsTop.onClick.AddListener(OnTurretsTopClick);
        turretsRight.onClick.AddListener(OnTurretsRightClick);
        turretsLeft.onClick.AddListener(OnTurretsLeftClick);
        health.onClick.AddListener(OnHealthClick);
        phobos.onClick.AddListener(OnPhobosClick);
        deimos.onClick.AddListener(OnDeimosClick);
        shieldGen.onClick.AddListener(OnShieldGenClick);
        shieldCap.onClick.AddListener(OnShieldCapClick);

    }

    public void OnTurretsTopClick()
    {
        int id_add = 0; // To identify which of the 9 turrets belong to this array
        UpgradeModalWindow.Instance.ChooseTurretModal(turretsTopTitle, turretsTopText, turretsTopImage, id_add);
    }
    public void OnTurretsRightClick()
    {
        int id_add = 3; // To identify which of the 9 turrets belong to this array
        UpgradeModalWindow.Instance.ChooseTurretModal(turretsRightTitle, turretsRightText, turretsRightImage, id_add);
    }

    private void OnTurretsLeftClick()
    {
        int id_add = 6; // To identify which of the 9 turrets belong to this array
        UpgradeModalWindow.Instance.ChooseTurretModal(turretsLeftTitle, turretsLeftText, turretsLeftImage, id_add);

    }

    public void OnHealthClick()
    {
        UpgradeModalWindow.Instance.ShowUpgradeModal(healthTitle, healthText, healthImage);
    }

    private void OnPhobosClick()
    {
        UpgradeModalWindow.Instance.ShowUpgradeModal(phobosTitle, phobosText, phobosImage);
    }

    private void OnDeimosClick()
    {
        UpgradeModalWindow.Instance.ShowUpgradeModal(deimosTitle, deimosText, deimosImage);
    }

    private void OnShieldGenClick()
    {
        UpgradeModalWindow.Instance.ShowUpgradeModal(shieldGenTitle, shieldGenText, shieldGenImage);
    }

    private void OnShieldCapClick()
    {
        UpgradeModalWindow.Instance.ShowUpgradeModal(shieldCapTitle, shieldCapText, shieldGenImage);
    }

}
