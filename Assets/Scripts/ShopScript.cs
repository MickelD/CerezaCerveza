using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopScript : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI shopTxt;
    [SerializeField] private GameObject itemCatalog;
    [SerializeField] private GameObject itemGetterPrefab;

    [SerializeField] private Vector2 itemDisplayDistance;
    [SerializeField] private Vector2Int minMaxCatalog;
    [SerializeField] private List<ItemBase> itemPool;
    private int itemCount;

    [System.NonSerialized] public RoomBehaviour roomTypeObj;

    private void OnEnable() { ItemGetter.OnItemSelected += UpdateShopDescription; }
    private void OnDisable() { ItemGetter.OnItemSelected -= UpdateShopDescription; }
        
    
    private void Awake()
    {
        canvas.worldCamera = Camera.main;
        roomTypeObj = transform.parent.gameObject.GetComponent<RoomBehaviour>();
        roomTypeObj.canReenter = true;

        itemCount = Random.Range(minMaxCatalog.x, minMaxCatalog.y);
    }

    private void Start()
    {
        for (int i = 0; i < itemCount + 1; i++) //generate shop items on the shelves
        {
            ItemGetter thisItem = Instantiate(itemGetterPrefab, itemCatalog.transform).GetComponent<ItemGetter>() ;

            thisItem.gameObject.transform.localPosition = new Vector3(itemDisplayDistance.x * (i / 2) , itemDisplayDistance.y * (i % 2));
            thisItem.isPaid = true;

            //When an item is regenerated, remove it from the pool so it does not repeat (except the first item)
            int rndItem = Random.Range(0, itemPool.Count);
            thisItem.SetItem(itemPool[rndItem]);

            if (rndItem != 0)
            {
                itemPool.RemoveAt(rndItem);
            }
        }
    }

    public void ExitShop()
    {
        roomTypeObj.playerIcon.SetActive(true);
        gameObject.SetActive(false);
    }

    private void UpdateShopDescription(string desc)
    {
        shopTxt.text = desc;
    }
}

