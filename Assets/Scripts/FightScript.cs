using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


[System.Serializable]
public class NotificationColors
{
    public Color missCol;
    public Color damageCol;
    public Color critCol;
    public Color defenseColor;
}

public class FightScript : MonoBehaviour
{
    //UI variables
    public Image dialogueSprite;
    private Canvas canvas;
    [SerializeField] private EventSystem eventSystem;
    private TextMeshProUGUI displayText;

    [SerializeField] private GameObject attackBar;
    private Transform attackSlider;
    private LineRenderer attackGradient;

    [SerializeField] public NotificationColors notificationColors;

    //Player references
    private GameManager gameManager;
    private PlayerManager playerManager;
    private PlayerFight player;

    //Fight variables
    [SerializeField] private float blockActionTemporaryArmor;
    private float baseArmor;

    [SerializeField] private float distPercentageForCrit;

    [System.NonSerialized] public RoomBehaviour roomTypeObj;
    
    [System.NonSerialized] public int playerTurns;
    [System.NonSerialized] public int enemyTurns;
    
    [System.NonSerialized] public string fightText;

    [SerializeField] private Vector2Int MinMaxEnemyTime; //use x and y as min/max vaulues, serialization of vectors is handy
    public GameObject[] spawnEnemies;
    [System.NonSerialized] public List<GameObject> enemiesInThisFight;

    private int fightReward;
    private int enemySpawnCount;
    private float endFightDelay = 3f;

    [SerializeField] private bool isBossFight;
    //Common delegates
    public event System.Action OnEndPlayerTurn;
    public event System.Action OnBeginPlayerTurn;

    private void Awake()
    {
        //assign components
        canvas = gameObject.GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        
        attackSlider = attackBar.transform.GetChild(1).GetComponent<Transform>();
        attackGradient = attackBar.GetComponentInChildren<LineRenderer>();
        attackBar.SetActive(false);
        
        //get player references
        player = gameObject.GetComponentInChildren<PlayerFight>();
        player.fight = this;
        gameManager = FindObjectOfType<GameManager>();

        //reference to the room that started the fight
        roomTypeObj = transform.parent.gameObject.GetComponent<RoomBehaviour>();
        roomTypeObj.canReenter = false;

        //reference to the fight text
        displayText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        fightText = "";

        //reference to fight variables
        enemiesInThisFight = new List<GameObject>();
    }
    private void Start()
    {
        playerManager = gameManager.playerManager;
        baseArmor = playerManager.playerArmor;

        enemySpawnCount = isBossFight? 1 : Random.Range(1, 5);

        //Positions that enemies can occupy on fight grounds
        Vector3[][] enemyPositions = {  
            //Position for 1 enemy
            new Vector3[] { new Vector3(6f, -0.5f)}, 
            //Positions for 2 enemies
            new Vector3[] { new Vector3(6f, 1f), new Vector3(6f, -2f)}, 
            //Positions for 3 enemies
            new Vector3[] { new Vector3(5f, -0.5f), new Vector3(7f, 1f), new Vector3(7f, -2f)},  
            //Positions for 4 enemies
            new Vector3[] { new Vector3(7f, -2f), new Vector3(7f, 1f), new Vector3(5f, -2f), new Vector3(5f, 1f)} }; 

        for (int i = 0; i < enemySpawnCount; i++) //Instantiate enemies in pre-defined positions
        {
            GameObject enemyToSpawn = spawnEnemies[Random.Range(0, spawnEnemies.Length)];//get a random enemy from all possible enemies
            Vector3 enemyPosition = enemyPositions[enemySpawnCount - 1] [i]; //access the correct individual position for this number of enemies from the array

            GameObject thisEnemy = Instantiate(enemyToSpawn, gameObject.transform);
            thisEnemy.transform.localPosition = enemyPosition;
            enemiesInThisFight.Add(thisEnemy);
        }

        foreach (GameObject enemy in enemiesInThisFight) //Calculate amount of gold to give the player based on how many enemies this fight has
        {
            fightReward += enemy.GetComponent<Enemy>().goldBounty;
        }

        //This prevents variables from updating before the room entry animation finishes
        StartCoroutine(GameManager.InvokeAfterDelay(0.2f, BeginPlayerTurn));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(FinishThisFight());
        }
    }

    //delegate events
    private void EndPlayerTurn() // END PLAYER TURN
    {        
        //Change UI to dialogue mode if there are still enemies alive
        if (enemiesInThisFight.Count > 0)
        {
            //player.weaponInUse.weaponHitBox.SetActive(false);
            DisplayWeaponHitbox(false);

            GetRandomEnemy().isTurnToTalk = true; //Give dialogue opportunity to a random enemy

            enemyTurns++;
            fightText = "";
            displayText.text = fightText; //clear fight text

            dialogueSprite.gameObject.SetActive(true); //Change UI to dialogue mode
            eventSystem.SetSelectedGameObject(null);
            eventSystem.gameObject.SetActive(false);

            OnEndPlayerTurn?.Invoke();
            StartCoroutine(GameManager.InvokeAfterDelay(Random.Range(MinMaxEnemyTime.x, MinMaxEnemyTime.y + 1), BeginPlayerTurn));
        }
        else //If not, display reward and end fight
        {
            fightText = "";
            StartCoroutine(UpdateFightText("You won! You received " + fightReward + " gold."));
            StartCoroutine(FinishThisFight(endFightDelay));
        }     
    }
    private void BeginPlayerTurn() //BEGIN PLAYER TURN
    {
        playerManager.SetArmor(baseArmor);

        playerTurns++;
        fightText = "";
        displayText.text = fightText; //clear fight text

        dialogueSprite.gameObject.SetActive(false);

        eventSystem.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);

        OnBeginPlayerTurn?.Invoke();
    }

    //player actions
    public void Attack() { StartCoroutine(AttackCoroutine()); } //Buttons cannot start coroutines, translate method into IEnumerator
    private IEnumerator AttackCoroutine()
    {
        bool isCritical = false;
        eventSystem.gameObject.SetActive(false);
        displayText.text = "";
        attackBar.SetActive(true); //Clear UI and prepare attack bar

        //Send attack slider to rightmost extreme of the bar and invoke it to move to the other side 
        attackSlider.localPosition = attackGradient.GetPosition(1); 
        yield return GameManager.MoveTowardsPoint(attackSlider.gameObject, attackGradient.GetPosition(0) - Vector3.right, player.weaponInUse.weaponSpeed, true);

        //Calculate a percentage based on how close to the left of the bar the attack was pressed
        float distPercent = (attackSlider.localPosition.x - attackGradient.GetPosition(0).x) / (attackGradient.GetPosition(1).x - attackGradient.GetPosition(0).x);
        
        //apply that percentage to the player's damage
        float dmg = (playerManager.playerDamage + player.weaponInUse.weaponDamage) * (1 - distPercent * 0.5f);

        //nullify damage if slider surpases the bar, and also give critial damage if close enough to the edge
        if (distPercent <= 0) 
        { 
            dmg = 0;
        }
        else if (distPercent <= distPercentageForCrit) { dmg *= player.weaponInUse.critMultiplier; isCritical = true; }
        
        //perform attack and end turn
        attackBar.SetActive(false);
        player.weaponInUse.weaponAnimator.SetTrigger("attack");

        //deal damage, notify HUD accordingly
        if(!player.weaponInUse.DealDamage(Mathf.RoundToInt(dmg)) || (dmg == 0)) //MISS
        {
            playerManager.DisplayNotification("Miss", notificationColors.missCol);
        }
        else if (isCritical) //CRITICAL HIT
        {
            playerManager.DisplayNotification(Mathf.RoundToInt(dmg) + " CRITICAL", notificationColors.critCol);
        }
        else //REGULAR DAMAGE
        {
            playerManager.DisplayNotification(Mathf.RoundToInt(dmg).ToString(), notificationColors.damageCol);
        }

        EndPlayerTurn(); 
    }
    public void Defend()
    {
        playerManager.DisplayNotification("Defense Up!", notificationColors.defenseColor);

        //Defend action raises the player's armor a specific percentage for that turn
        baseArmor = playerManager.playerArmor;
        playerManager.SetArmor(playerManager.playerArmor + blockActionTemporaryArmor);
        EndPlayerTurn();
    }

    //methods
    public void DisplayWeaponHitbox(bool display)
    {
        player.weaponInUse.weaponHitBox.SetActive(display);
    }

    public IEnumerator UpdateFightText(string updateText)
    {
        if(!fightText.Contains(updateText)) //Make sure that text is not repeated
        {
            fightText += updateText;
        }     
        yield return new WaitForEndOfFrame(); //Ensure the text is properly updated before rendering it
        displayText.text = fightText;
    }
    private IEnumerator FinishThisFight(float delay = 0f)
    {
        playerManager.SetGold(playerManager.currentGold + fightReward);
        DisplayWeaponHitbox(false);

        yield return new WaitForSeconds(delay);

        if (!isBossFight) //If it is just a regular fight you go back to the map, but it is the boss fight the games moves to the next level
        {
            roomTypeObj.playerIcon.SetActive(true);
            roomTypeObj.canSpawnRoom = false;
            Destroy(gameObject);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public Enemy GetRandomEnemy()
    {
        return enemiesInThisFight[Random.Range(0, enemiesInThisFight.Count)].GetComponent<Enemy>();
    }
}
