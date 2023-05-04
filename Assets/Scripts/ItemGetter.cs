using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class DisplayColors
{
    public Color itemGet;
    public Color notGet;
}

[RequireComponent(typeof(Button))]
public class ItemGetter : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    private Image image;
    private PlayerManager playerManager;
    private GameManager gameManager;

    [SerializeField] private ItemBase item;

    [SerializeField] private DisplayColors displayColors;

    public bool isPaid;
    public int price;

    public static event System.Action<string> OnItemSelected;

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        image = gameObject.GetComponent<Image>();

        gameManager = FindObjectOfType<GameManager>();

        playerManager = gameManager.playerManager;

        SetItem(item);       
    }

    public void SetItem(ItemBase it)
    {
        item = it;

        image.sprite = item.itemImage;
        price = (int)item.type * isPaid.GetHashCode();
    }

    public void ApplyItem() //There are ways to get items that are not through the item getter, which is why this class is not the one that calls the OnItemGetEvent
    {


        if (playerManager.currentGold >= price) //Paywall
        {
            playerManager.SetGold(playerManager.currentGold - price);
            playerManager.DisplayNotification(item.itemTitle, displayColors.itemGet);

            if (item.type == ItemBase.itemType.Weapon) //If the player picks up a weapon, the weapon they were holding stays on the itemgetter
            {
                ItemBase replaceItem;
                replaceItem = playerManager.currentWeapon;

                playerManager.EquipItem(item);

                isPaid = false;
                SetItem(replaceItem);
            }
            else
            {
                playerManager.EquipItem(item);
                eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
                Destroy(gameObject);
            }           
        }
        else
        {
            playerManager.DisplayNotification("No funds", displayColors.notGet);
        }

    }

    public void ItemSelected()
    {
        string header = item.itemName + (isPaid ? " - " + price + "g\n" : "\n"); //Only display price if the item is not free
        
        OnItemSelected?.Invoke(header + item.itemDescription);
    }
}
