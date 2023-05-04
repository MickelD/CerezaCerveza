using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject deathScreen;

    private Canvas canv;
    private PlayerManager playerManager;
    private GameManager gameManager;

    [SerializeField] private Image playerHPBar;
    [SerializeField] private TextMeshProUGUI hpCounter;

    [SerializeField] private Image weaponSprite;
    [SerializeField] private TextMeshProUGUI goldCounter;
    [SerializeField] private GameObject announcer;

    private void Awake()
    {
        canv = gameObject.GetComponent<Canvas>();
        canv.worldCamera = Camera.main;

        gameManager = FindObjectOfType<GameManager>();      
    }

    private void Start()
    {
        playerManager = gameManager.playerManager;

        playerManager.OnUpdateGold += UpdateGold;
        playerManager.OnUpdateCurrentHP += UpdateCurrentHP;
        playerManager.OnDisplayNotification += Announcement;
        playerManager.OnItemEquip += DisplayEquipItem;
        playerManager.OnPlayerDeath += PlayerDeath;

        UpdateCurrentHP(playerManager.currentPlayerHP, playerManager.maxPlayerHP);
        UpdateGold(playerManager.currentGold);
        DisplayEquipItem(playerManager.currentWeapon);
    }

    public void DisplayEquipItem(ItemBase item)
    {
        if (weaponSprite != null && item.type == ItemBase.itemType.Weapon)
        {
            weaponSprite.sprite = item.itemImage;
        }
    }

    private void UpdateGold(int gold)
    {
        if(goldCounter != null)
        {
            goldCounter.text = gold.ToString() + "g";
        }
    }

    private void UpdateCurrentHP(int hp, int maxHP)
    {
        if(playerHPBar != null)
        {
            hpCounter.text = hp.ToString();
            playerHPBar.fillAmount = (float)hp / maxHP;
        }

    }

    private void Announcement(string text, Color col)
    {
        if(announcer != null)
        {
            announcer.GetComponent<TextMeshProUGUI>().text = text;
            announcer.GetComponent<TextMeshProUGUI>().color = col;

            announcer.GetComponent<Animator>().SetTrigger("notify");
        }
    }

    private void PlayerDeath()
    {
        if(deathScreen != null)
        {
            deathScreen.GetComponent<Animator>().SetTrigger("death");
        }       
    }
}
