using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private float audioTime = 0f;

    private RaycastHit2D roomCheck;

    private GameManager gameManager;
    private Collider2D playerCol;

    private float verDist; //distance at which new rooms are created at the North and South
    private float horDist; //distance at which new rooms are created at the East and West

    public float roomSwapSpeed;

    //Make song duration be stored when entering/exiting rooms
    private void OnEnable() { audioSource.time = audioTime; }
    private void OnDisable() { audioTime = audioSource.time; }


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerCol = gameObject.GetComponent<Collider2D>();

        verDist = gameManager.verDist;
        horDist = gameManager.horDist;
    }

    public bool lockMovement = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) ^ Input.GetKeyDown(KeyCode.W) && !lockMovement) //Check for rooms north when pressing UP
        {
            roomCheck = Physics2D.Raycast(transform.position, Vector2.up, verDist);
            if (roomCheck.collider != null && roomCheck.collider.tag == "room")
            {
                StartCoroutine(SwitchRoom(transform.position + Vector3.up * verDist, verDist));
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) ^ Input.GetKeyDown(KeyCode.S) && !lockMovement) //check for rooms south when pressing DOWN
        {
            roomCheck = Physics2D.Raycast(transform.position, -Vector2.up, verDist);
            if (roomCheck.collider != null && roomCheck.collider.tag == "room")
            {
                StartCoroutine(SwitchRoom(transform.position - Vector3.up * verDist, verDist));
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) ^ Input.GetKeyDown(KeyCode.A) && !lockMovement) //check for rooms west when pressing LEFT
        {
            roomCheck = Physics2D.Raycast(transform.position, -Vector2.right, horDist);
            if (roomCheck.collider != null && roomCheck.collider.tag == "room")
            {
                StartCoroutine(SwitchRoom(transform.position - Vector3.right * horDist, horDist));
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) ^ Input.GetKeyDown(KeyCode.D) && !lockMovement) //check for rooms east when pressing RIGHT
        {
            roomCheck = Physics2D.Raycast(transform.position, Vector2.right, horDist);
            if (roomCheck.collider != null && roomCheck.collider.tag == "room")
            {
                StartCoroutine(SwitchRoom(transform.position + Vector3.right * horDist, horDist));
            }
        }
               
    }

    IEnumerator SwitchRoom(Vector3 target, float speedMultiplier)
    {
        lockMovement = true;
        playerCol.enabled = false;
        yield return StartCoroutine(GameManager.MoveTowardsPoint(gameObject, target, speedMultiplier * roomSwapSpeed));
        playerCol.enabled = true;
        lockMovement = false;        
    }

}
