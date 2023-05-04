using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class RoomReferences
{
    public GameObject enemyRoom;
    public GameObject itemRoom;
    public GameObject bossRoom;
    public GameObject specialRoom;
    public GameObject shopRoom;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerManagerPrefab;
    public PlayerManager playerManager;

    public int maxRooms;
    [SerializeField] private int extraItemChance;

    [System.NonSerialized] public int roomCount;

    public GameObject roomObject;

    public float verDist = 1.1f;
    public float horDist = 1.6f;

    public enum RoomType
    {
        Start,
        Enemy,
        Item,
        Special,
        Shop,
        Boss
    }
    public RoomType[] levelRooms;

    public RoomReferences roomReferences;

    private void OnEnable() //If for any reason no player manager has been generated, simply create a new one
    {
        if (FindObjectOfType<PlayerManager>() == null) 
        {
            playerManager = Instantiate(playerManagerPrefab).GetComponent<PlayerManager>();
        }
        else { playerManager = FindObjectOfType<PlayerManager>(); }
    }

    private void Awake()
    {
        levelRooms = new RoomType[maxRooms];

        //make the first room the Start room and last room the Boss room
        levelRooms[0] = RoomType.Start;
        levelRooms[levelRooms.Length - 1] = RoomType.Boss;

        //place the shop, item, and special rooms randomly in the sequence, excluding previosuly picked positions
        int shopIndex = RandomRoomIndex(new int[0]);
        levelRooms[shopIndex] = RoomType.Shop;
        int itemIndex = RandomRoomIndex(new int[] { shopIndex});
        levelRooms[itemIndex] = RoomType.Item;
        int specialIndex = RandomRoomIndex(new int[] { shopIndex, itemIndex});
        levelRooms[specialIndex] = RoomType.Special;
        int enemyIndex = RandomRoomIndex(new int[] { shopIndex, itemIndex, specialIndex});
        levelRooms[enemyIndex] = RoomType.Enemy;

        for (int i = 1; i < levelRooms.Length - 1; i++)
        {
            if(i == shopIndex || i == itemIndex || i == specialIndex || i == enemyIndex)
            {
                continue;
            }
            else
            {
                switch (Random.Range(1, extraItemChance + 1))
                {
                    case 1:
                        levelRooms[i] = RoomType.Item;
                        break;
                    default:
                        levelRooms[i] = RoomType.Enemy;                       
                        break;
                }
            }
        }
    }

    private int RandomRoomIndex(int[] toExclude) //This function returns a random index from the array of rooms, reinvoking itself if it choses an undesirable value
    {
        int i = Random.Range(1, levelRooms.Length - 1);
        if (toExclude.Contains(i)) { return RandomRoomIndex(toExclude); }
        else { return i; }
    }

    public static IEnumerator MoveTowardsPoint(GameObject objToMove, Vector3 target, float speed, bool interruptable = false)
    {
        while (objToMove.transform.localPosition != target)
        {        
            objToMove.transform.localPosition = Vector3.MoveTowards(objToMove.transform.localPosition, target, speed * Time.deltaTime);
            yield return null;

            if (interruptable && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                yield break;
            }
        }           
    }

    public static IEnumerator InvokeAfterDelay(float delay, System.Action func)
    {
        yield return new WaitForSeconds(delay);
        func.Invoke();
    }


}
