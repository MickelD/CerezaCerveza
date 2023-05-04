using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TreasureScript : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI itemTxt;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject itemGetterPrefab;
    [SerializeField] private Transform itemPlace;
    [SerializeField] private ItemBase[] itemPool;

    [System.NonSerialized] public RoomBehaviour roomTypeObj;

    private void OnEnable() { ItemGetter.OnItemSelected += UpdateItemDescription; }
    private void OnDisable() { ItemGetter.OnItemSelected -= UpdateItemDescription; }

    private void Awake()
    {
        canvas.worldCamera = Camera.main;
        roomTypeObj = transform.parent.gameObject.GetComponent<RoomBehaviour>();
        roomTypeObj.canReenter = true;
    }

    private void Start()
    {
        ItemGetter thisItem = Instantiate(itemGetterPrefab, itemPlace.transform).GetComponent<ItemGetter>();
        thisItem.isPaid = false;

        thisItem.SetItem(itemPool[Random.Range(0, itemPool.Length)]);

        eventSystem.SetSelectedGameObject(thisItem.gameObject);
    }

    public void ExitTreasureRoom()
    {
        roomTypeObj.playerIcon.SetActive(true);
        gameObject.SetActive(false);
    }

    private void UpdateItemDescription(string desc)
    {
        itemTxt.text = desc;
    }
}

