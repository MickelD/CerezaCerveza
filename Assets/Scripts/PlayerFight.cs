using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFight : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerManager playerManager;
    [System.NonSerialized] public FightScript fight;

    private float movementSpeed;

    public bool canReceiveDamage;
    [System.NonSerialized] public bool allowMovement;

    [SerializeField] private Animator healthAnimator;
    [SerializeField] private Animator moveAnimator;

    private Vector3 inputDir;
    private Rigidbody2D rb;

    public Transform weaponSlot;
    public Weapon weaponInUse;

    //subscribe methods to delegate
    private void OnEnable() { fight.OnBeginPlayerTurn += OnBeginPlayerTurn; fight.OnEndPlayerTurn += OnEndPlayerTurn; }
    private void OnDisable() { fight.OnBeginPlayerTurn -= OnBeginPlayerTurn; fight.OnEndPlayerTurn -= OnEndPlayerTurn; }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        rb = gameObject.GetComponent<Rigidbody2D>();
        allowMovement = false;
    }
    private void Start()
    {
        playerManager = gameManager.playerManager;
        movementSpeed = playerManager.playerSpeed;

        weaponInUse = Instantiate(playerManager.currentWeapon, weaponSlot);
        weaponInUse.weaponHitBox.SetActive(false);
    }

    void Update()
    {
        //Read Input
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        //make player walk and face left or right
        gameObject.transform.localScale = new Vector3((xInput != 0) && allowMovement ? xInput : 1, 1f);

        //animations
        if (allowMovement && (xInput != 0 || yInput != 0))
        {
            moveAnimator.SetBool("walk", true);
        }
        else { moveAnimator.SetBool("walk", false); }

        inputDir = new Vector3(xInput * allowMovement.GetHashCode(), yInput * allowMovement.GetHashCode());
    }

    private void FixedUpdate()
    {
        //Update Position
        rb.MovePosition(transform.position + (inputDir.normalized * movementSpeed * Time.deltaTime));
    }

    //damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "bullet" && !healthAnimator.GetCurrentAnimatorStateInfo(0).IsTag("damage"))
        {
            healthAnimator.SetTrigger("getDamage");

            //get the attack base script from the attack that hit the player to calculate pertienent damage 
            AttackBase attack = other.GetComponent<AttackBase>() ? other.GetComponent<AttackBase>() : other.GetComponentInParent<AttackBase>();
            int dmg = Mathf.RoundToInt((1 - playerManager.playerArmor) * attack.attackDmg);

            playerManager.SetCurrentHealth(playerManager.currentPlayerHP - dmg, playerManager.maxPlayerHP);

            if (attack.isDestroyedOnHit)
            {
                other.gameObject.SetActive(false);
            }
        }
    }

    //delegate events
    public void OnEndPlayerTurn() //player moves and dodges attacks while it is NOT THEIR TURN.
    {
        allowMovement = true;
    }

    public void OnBeginPlayerTurn() //Player cannot move while is THEIR TURN and navigates menus instead
    {
        allowMovement = false;

    }
}
