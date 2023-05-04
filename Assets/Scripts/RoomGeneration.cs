using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    private GameObject roomPrefab;

    private GameManager gameManager;

    private float verDist; //distance at which new rooms are created at the North and South
    private float horDist; //distance at which new rooms are created at the East and West

    private int roomsFound;

    private LineRenderer lineRenderer;

    public GameObject roomTypeTemplate;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        roomsFound = 0;

        verDist = gameManager.verDist;
        horDist = gameManager.horDist;
    }

    private void Start()
    {
        roomPrefab = gameManager.roomObject;
        //update room count
        gameManager.roomCount++;
        gameObject.name = "objRoom" + gameManager.roomCount;

        //assign type, typetags are [shopRoom, itemRoom, enemyRoom, bossRoom, specialRoom]
        GameObject newType;
        switch (gameManager.levelRooms[gameManager.roomCount - 1])
        {
            case GameManager.RoomType.Start:
                break;
            case GameManager.RoomType.Enemy:
                newType = Instantiate(roomTypeTemplate, transform);
                newType.tag = newType.name = "enemyRoom";
                break;
            case GameManager.RoomType.Item:
                newType = Instantiate(roomTypeTemplate, transform);
                newType.tag = newType.name = "itemRoom";
                break;
            case GameManager.RoomType.Special:
                newType = Instantiate(roomTypeTemplate, transform);
                newType.tag = newType.name = "specialRoom";
                break;
            case GameManager.RoomType.Shop:
                newType = Instantiate(roomTypeTemplate, transform);
                newType.tag = newType.name = "shopRoom";
                break;
            case GameManager.RoomType.Boss:
                newType = Instantiate(roomTypeTemplate, transform);
                newType.tag = newType.name = "bossRoom";
                break;
        }
        
        //look for adjacent rooms  and draw line if a connection is found
        RaycastHit2D entryCheck;
        {
            //check below
            entryCheck = Physics2D.Raycast(transform.position, -Vector3.up, verDist);
            if (entryCheck.collider != null && entryCheck.collider.tag == "room")
            {
                lineRenderer.positionCount += 2;
                LinkRooms(transform.position - Vector3.up * verDist);
                roomsFound++;
            }

            //check left            
            entryCheck = Physics2D.Raycast(transform.position, -Vector3.right, horDist);
            if (entryCheck.collider != null && entryCheck.collider.tag == "room")
            {
                lineRenderer.positionCount += 2;
                LinkRooms(transform.position - Vector3.right * horDist);
                roomsFound++;
            }

            //check right
            entryCheck = Physics2D.Raycast(transform.position, Vector3.right, horDist);
            if (entryCheck.collider != null && entryCheck.collider.tag == "room")
            {
                lineRenderer.positionCount += 2;
                LinkRooms(transform.position + Vector3.right * horDist);
                roomsFound++;
            }

            //check above
            entryCheck = Physics2D.Raycast(transform.position, Vector3.up, verDist);
            if (entryCheck.collider != null && entryCheck.collider.tag == "room")
            {
                lineRenderer.positionCount += 2;
                LinkRooms(transform.position + Vector3.up * verDist);
                roomsFound++;
            }
        }
     
        //Generate rooms, 1 = North, 2 = East, 3 = West, 4 = South
        if (gameManager.roomCount < gameManager.maxRooms) { TryGenerateRoom(); }
    }

    void TryGenerateRoom()
    {
        RaycastHit2D rc;

        switch (Random.Range(1, 5))
        {
            //try spawn south
            case 4:
                rc = Physics2D.Raycast(transform.position, -Vector3.up, verDist);

                if (rc.collider != null && rc.collider.tag == "room")
                {
                    TryGenerateRoom();
                }
                else { GenerateRoom(4); }
                break;

            //spawn left
            case 3:
                rc = Physics2D.Raycast(transform.position, -Vector3.right, horDist);

                if (rc.collider != null && rc.collider.tag == "room")
                {
                    TryGenerateRoom();
                }
                else { GenerateRoom(3); }
                break;

            //spawn right
            case 2:
                rc = Physics2D.Raycast(transform.position, Vector3.right, horDist);

                if (rc.collider != null && rc.collider.tag == "room")
                {
                    TryGenerateRoom();
                }
                else { GenerateRoom(2); }
                break;

            //spawn above
            case 1:
                rc = Physics2D.Raycast(transform.position, Vector3.up, verDist);

                if (rc.collider != null && rc.collider.tag == "room")
                {
                    TryGenerateRoom();
                }
                else { GenerateRoom(1); }
                break;
        }

    }

    void GenerateRoom(int direction)
    {
        switch (direction)
        {
            //spawn south
            case 4:
                Instantiate(roomPrefab, this.transform.position - Vector3.up * verDist, transform.rotation);
                break;
            //spawn west
            case 3:
                Instantiate(roomPrefab, this.transform.position - Vector3.right * horDist, transform.rotation);
                break;
            //spawn east
            case 2:
                Instantiate(roomPrefab, this.transform.position + Vector3.right * horDist, transform.rotation);
                break;
            //spawn north
            case 1:
                Instantiate(roomPrefab, this.transform.position + Vector3.up * verDist, transform.rotation);
                break;
        }
    }

    void LinkRooms(Vector3 destination)
    {
        lineRenderer.SetPosition(2 * roomsFound, transform.position);
        lineRenderer.SetPosition(2 * roomsFound + 1, destination);
    }
}
