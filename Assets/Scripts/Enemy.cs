using System.Linq;
using System.Collections;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UnityEngine.UI;


public class Enemy : MonoBehaviour
{
    //Enemy properties
    public float attackDelay;
    public int enemyMaxHP;
    [SerializeField] private Vector2Int MinMaxGoldReward; //use x and y as min/max vaulues, serialization of vectors is handy

    public Sprite textSprite;

    public GameObject[] enemyAttacks;
    private int attacksPerTurn = 1;
    public int turnsForExtraAttack; //until how many turns does the enemy get to perform an extra attack
    public int maxAttacksPerTurn;

    //Lines for the fight text, followed by at which turn number they appear
    [SerializeField, Header("Lines spoken by the enemy on its turn")]
    public FightDialogues fightDialogues;
    [SerializeField, Header("Lines that narrate the fight on the player's turn")]
    public FightNarrations fightNarrations;

    //variables
    [System.NonSerialized] public int goldBounty;
    [System.NonSerialized] public int enemyCurrentHP;

    [System.NonSerialized] public FightScript fight;

    [System.NonSerialized] public bool isTurnToTalk;
    private bool isDead;

    private Animator enemyAnimator;

    private GameObject hpBarParent;
    private TextMeshProUGUI hpCounter;
    private Image hpBar;

    //subscribe methods to delegate
    private void OnEnable() { fight.OnBeginPlayerTurn += OnBeginPlayerTurn; fight.OnEndPlayerTurn += OnEndPlayerTurn; }
    private void OnDisable() { fight.OnBeginPlayerTurn -= OnBeginPlayerTurn; fight.OnEndPlayerTurn -= OnEndPlayerTurn; }

    private void Awake()
    {       
        fight = transform.parent.GetComponent<FightScript>();

        enemyAnimator = GetComponent<Animator>();

        isTurnToTalk = false;
        isDead = false;

        hpBarParent = gameObject.GetComponentInChildren<Canvas>().gameObject;
        hpCounter = hpBarParent.GetComponentInChildren<TextMeshProUGUI>();
        hpBar = hpBarParent.transform.GetChild(1).GetComponent<Image>();

        UpdateHP(enemyMaxHP);
        goldBounty = Random.Range(MinMaxGoldReward.x, MinMaxGoldReward.y + 1);

        
    }

    private void OnTriggerEnter2D(Collider2D other) { enemyAnimator.SetBool("onTarget", true); }
    private void OnTriggerExit2D(Collider2D other) { enemyAnimator.SetBool("onTarget", false); }

    //delegate events
    private void OnEndPlayerTurn()
    {
        if (isTurnToTalk)
        {
            fight.dialogueSprite.sprite = textSprite;
            SetCurrentTurnText(false);
            isTurnToTalk = false;
        }

        if (!isDead) //If the enemy is allowed to, invoke random attacks
        {
            attacksPerTurn += fight.enemyTurns / turnsForExtraAttack; //calculate attacks per turn based on turn count
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        for (int i = 0; i < Mathf.Clamp(attacksPerTurn, 0, maxAttacksPerTurn); i++)
        {
            //After a specific delay, invoke an attack
            yield return StartCoroutine(GameManager.InvokeAfterDelay(attackDelay, () => {
                GameObject newAttack = Instantiate(enemyAttacks[Random.Range(0, enemyAttacks.Length)], fight.transform);
                newAttack.GetComponent<AttackBase>().invoker = gameObject;
            }));
        }
    }

    private void OnBeginPlayerTurn()
    {
        StopAllCoroutines();
        SetCurrentTurnText(true);
    }

    //public methods
    public void UpdateHP(int hp)
    {
        enemyCurrentHP = hp;

        hpCounter.text = Mathf.Clamp(enemyCurrentHP, 0, enemyMaxHP).ToString();
        hpBar.fillAmount = (float)enemyCurrentHP / enemyMaxHP;

        if (enemyCurrentHP <= 0 )
        {
            enemyAnimator.SetTrigger("die");
            fight.enemiesInThisFight.Remove(gameObject);
            isDead = true;
        }
    }

    public void EnemyDie()
    {
        gameObject.SetActive(false); //defeated enemies will be destroyed when the fight is over, there is no need to use destory now
    }

    private void SetCurrentTurnText(bool countPlayerTurns)
    {
        //store the keys and values from the dictionaries into arrays, since arrays have more properties than ICollection types
        int[] values;
        string[] lines;
        int turns;
        switch (countPlayerTurns)
        {
            case false: // COUNTING ENEMY TURNS, use dialogues dictionary
                lines = new string[fightDialogues.Keys.Count];
                fightDialogues.Keys.CopyTo(lines, 0);

                values = new int[fightDialogues.Values.Count];
                fightDialogues.Values.CopyTo(values, 0);

                turns = fight.enemyTurns;
                break;
            case true: // COUNTING PLAYER TURNS, use narrations dictionary
                lines = new string[fightNarrations.Keys.Count];
                fightNarrations.Keys.CopyTo(lines, 0);

                values = new int[fightNarrations.Values.Count];
                fightNarrations.Values.CopyTo(values, 0);

                turns = fight.playerTurns;
                break;
        }

        // starting at the current enemy turn, check if there is a matching value. If there is, extract it from the list and use its corresponding line. If there is not, keep going down
        if (gameObject.activeInHierarchy == true) //If the enemy is still alive, of course
        {
            for (int t = turns; t >= 0; t--)
            {
                if (values.Contains(t))
                {
                    
                    //Update the string that the game is told to draw at the end of the frame (This allows for the string to receive all enemy updates before being drawn)
                    IEnumerator rot = fight.UpdateFightText(lines[System.Array.IndexOf(values, t)] + "\n");
                    fight.StartCoroutine(rot);
                    break;
                }
            }
        }            
    }
}

//SERIALIZABLE DICTIONARIES
[System.Serializable]
public class FightDialogues : SerializableDictionaryBase<string, int> { }

[System.Serializable]
public class FightNarrations : SerializableDictionaryBase<string, int> { }
