using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.U2D.Path.GUIFramework;

public class UpgradeModalWindow : MonoBehaviour
{
    // Make singleton
    public static UpgradeModalWindow Instance;
    private UpgradeManager upgradeManager;

    [SerializeField] private TextMeshProUGUI titleObject;
    [SerializeField] private TextMeshProUGUI descriptionObject;
    [SerializeField] private Image imageObject;

    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private Button turret1;
    [SerializeField] private Button turret2;
    [SerializeField] private Button turret3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        Hide();
    }

    public void ChooseTurretModal(string title, string description, Image image, int id_add)
    {
        // Let the player choose which of the 3 turrets of an array they want to inspect.

        titleObject.text = title;
        descriptionObject.text = description;
        imageObject.sprite = image.sprite;

        UnhideTurretButtons();
        HideUpgradeButton();
        Unhide();

        turret1.onClick.RemoveAllListeners();
        turret1.onClick.AddListener(() =>
        {
            Hide();
            ShowUpgradeModal(id_add);
        });

        turret2.onClick.RemoveAllListeners();
        turret2.onClick.AddListener(() =>
        {
            Hide();
            ShowUpgradeModal(id_add + 1);
        });

        turret3.onClick.RemoveAllListeners();
        turret3.onClick.AddListener(() =>
        {
            Hide();
            ShowUpgradeModal(id_add + 2);
        });
    }

    public void ShowUpgradeModal(string title, string description, Image image)
    {
        // Let the player choose to upgrade one of the player stats.

        titleObject.text = title;
        descriptionObject.text = description;
        imageObject.sprite = image.sprite;
        
        UnhideUpgradeButton();
        HideTurretButtons();
        Unhide();

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() =>
        {
            Hide();
            // UPGRADE ACTION
            UpgradeManager.Instance.SetPlayerUpgrades();
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public void ShowUpgradeModal(int id)
    {
        // Let the player choose whether to upgrade this specific turret.
        if (UpgradeManager.Instance.CanStatUpgrade(id))
        {
            UnhideUpgradeButton();
        }
        else
        {
            HideUpgradeButton();
        }
        HideTurretButtons();
        Unhide();

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() =>
        {
            Hide();
            UpgradeManager.Instance.UpgradeTurret(id);
            UpgradeManager.Instance.SetPlayerUpgrades();
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            Hide();
        });

    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    void HideTurretButtons()
    {
        turret1.gameObject.SetActive(false);
        turret2.gameObject.SetActive(false);
        turret3.gameObject.SetActive(false);
    }

    void HideUpgradeButton()
    {
        upgradeButton.gameObject.SetActive(false);
    }

    void UnhideTurretButtons()
    {
        turret1.gameObject.SetActive(true);
        turret2.gameObject.SetActive(true);
        turret3.gameObject.SetActive(true);
    }

    void UnhideUpgradeButton()
    {
        upgradeButton.gameObject.SetActive(true);
    }

    private void Unhide()
    {
        gameObject.SetActive(true);
    }
}
