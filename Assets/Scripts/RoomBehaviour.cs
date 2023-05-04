using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class RoomPrefabs
{
    public GameObject fightPrefab;
    public GameObject itemPrefab;
    public GameObject bossPrefab;
    public GameObject specialPrefab;
    public GameObject shopPrefab;
}

[System.Serializable]
public class RoomIcons
{
    public Sprite fightIcon;
    public Sprite itemIcon;
    public Sprite bossIcon;
    public Sprite specialIcon;
    public Sprite shopIcon;
}

public class RoomBehaviour : MonoBehaviour
{
    private GameManager gameManager;
    public float roomSceneYOffset;

    [SerializeField] public RoomPrefabs roomPrefabs;
    [SerializeField] public RoomIcons roomIcons;

    private GameObject roomToInitiate;
    private GameObject thisRoom;

    private SpriteRenderer iconRenderer;
    [System.NonSerialized] public GameObject playerIcon;

    [System.NonSerialized] public bool canSpawnRoom;
    [System.NonSerialized] public bool canReenter;

    private void Awake()
    {
        playerIcon = FindObjectOfType<PlayerMovement>().gameObject;
        canSpawnRoom = true;

        gameManager = FindObjectOfType<GameManager>();

        iconRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        switch (gameObject.tag) // room type tags are: shopRoom, itemRoom, enemyRoom, bossRoom, specialRoom
        {
            case "shopRoom":
                iconRenderer.sprite = roomIcons.shopIcon;
                roomToInitiate = gameManager.roomReferences.shopRoom;
                break;
            case "itemRoom":
                iconRenderer.sprite = roomIcons.itemIcon;
                roomToInitiate = gameManager.roomReferences.itemRoom;
                break;
            case "enemyRoom":
                iconRenderer.sprite = roomIcons.fightIcon;
                roomToInitiate = gameManager.roomReferences.enemyRoom;
                break;
            case "bossRoom":
                iconRenderer.sprite = roomIcons.bossIcon;
                roomToInitiate = gameManager.roomReferences.bossRoom;
                break;
            case "specialRoom":
                iconRenderer.sprite = roomIcons.specialIcon;
                roomToInitiate = gameManager.roomReferences.specialRoom;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {      
        if (other.gameObject.tag == "Player" && roomToInitiate != null) //If there is an assigned room to initiate and the player was detected
        {
            if (thisRoom == null && canSpawnRoom) //If no room has yet been generated, instantiate a new one of the selected type
            {
                thisRoom = Instantiate(roomToInitiate, gameObject.transform);
                thisRoom.transform.localPosition += Vector3.up * roomSceneYOffset;
                playerIcon.SetActive(false);
            }
            else if (canReenter) //If the room exists and it can be re-entered, simply reactivate it. This is because fights cannot be re-entered
            {
                thisRoom.SetActive(true);
                playerIcon.SetActive(false);
            }
            playerIcon.GetComponent<Collider2D>().enabled = false;         
        }
    }
}
