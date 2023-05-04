using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public Sprite itemImage;

    protected PlayerManager playerManager;

    public enum itemType { Weapon = 150, Passive = 100, OneTime = 50} //Sore prices for different weapon types on the weapon type enum
    public itemType type;

    [SerializeField] public string itemName;
    [SerializeField] public string itemTitle;
    [SerializeField, TextArea(4, 10)] public string itemDescription;

    public abstract void OnEquipThisItem(PlayerManager player);    
}
