using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public int maxPlayerHP;
    [System.NonSerialized] public int currentPlayerHP;
    public event System.Action<int, int> OnUpdateCurrentHP;

    public int playerDamage;
    public Weapon currentWeapon;

    public Vector2 minMaxArmor;
    public float playerArmor;

    public Vector2 minMaxSpeed;
    public float playerSpeed;

    public event System.Action<ItemBase> OnItemEquip;
    public List<ItemBase> playerItems;

    public event System.Action<int> OnUpdateGold;
    [System.NonSerialized] public int currentGold;

    public event System.Action<string, Color> OnDisplayNotification;
    public event System.Action OnPlayerDeath;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        SetGold(0);

        SetCurrentHealth(maxPlayerHP, maxPlayerHP);
        EquipItem(currentWeapon);
    }

    public void EquipItem(ItemBase item)
    {
        if(item.type == ItemBase.itemType.Passive)
        {
            playerItems.Add(item);
        }

        item.OnEquipThisItem(this);
        OnItemEquip?.Invoke(item);
    }

    //use functions to set values that have natural limits
    public void SetArmor(float armor)
    {
        playerArmor = Mathf.Clamp(armor, minMaxArmor.x, minMaxArmor.y);
    }

    public void SetSpeed(float spd)
    {
        playerSpeed = Mathf.Clamp(spd, minMaxSpeed.x, minMaxSpeed.y);
    }

    public void SetGold(int gold)
    {
        currentGold = gold;
        OnUpdateGold?.Invoke(currentGold);
    }

    public void SetCurrentHealth(int hp, int maxHP)
    {
        maxPlayerHP = maxHP;
        currentPlayerHP = Mathf.Clamp(hp, 0, maxPlayerHP);

        if (currentPlayerHP == 0)
        {
            OnPlayerDeath?.Invoke();
            StartCoroutine(GameManager.InvokeAfterDelay(3f, () => SceneManager.LoadScene(0)));
            
        }
        OnUpdateCurrentHP?.Invoke(currentPlayerHP, maxHP);
    }

    public void DisplayNotification(string text, Color col)
    {
        OnDisplayNotification?.Invoke(text, col);
    }
}
