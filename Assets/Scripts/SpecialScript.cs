using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SpecialScript : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private ItemBase trapItem;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private TextMeshProUGUI specialText;
    [SerializeField] private string offer;

    private PlayerManager playerManager;
    private GameManager gameManager;

    [System.NonSerialized] public RoomBehaviour roomTypeObj;

    private void OnEnable()
    {
        eventSystem.gameObject.SetActive(true);
        specialText.text = offer;
    }

    private void Awake()
    {
        canvas.worldCamera = Camera.main;
        roomTypeObj = transform.parent.gameObject.GetComponent<RoomBehaviour>();
        roomTypeObj.canReenter = true;

        gameManager = FindObjectOfType<GameManager>();
    }
    private void Start()
    {
        playerManager = gameManager.playerManager;
    }

    public void AcceptTrap()
    {
        playerManager.EquipItem(trapItem);
        playerManager.DisplayNotification(trapItem.itemTitle, Color.white);

        StartCoroutine(GameManager.InvokeAfterDelay(2f, () => {
            roomTypeObj.playerIcon.SetActive(true);
            roomTypeObj.canSpawnRoom = false;
            roomTypeObj.canReenter = false;
            Destroy(gameObject);
        }));
    }

    public void ExitSpecialRoom()
    {
        StartCoroutine(GameManager.InvokeAfterDelay(2f, () => {
            roomTypeObj.playerIcon.SetActive(true);
            gameObject.SetActive(false);
        }));
        
    }
}


