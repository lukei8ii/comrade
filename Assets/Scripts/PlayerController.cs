using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Slapping,
        Drinking,
        Throwing,
        Thrown,
        Stunned,
        GameOver
    }

    struct ThrownPotato
    {
        public Rigidbody2D Potato;
        public PlayerController Owner;
    }

    public int playerNumber;
    public float slapCooldownTime = 1f;
    public float vodkaCooldownTime = 1f;
    public float potatoCooldownTime = 1f;
    public PlayerController enemyPlayer;
    public GameObject potatoPrefab;
    public Transform potatoSpawn;
    public Vector2 potatoDirection;
    public float potatoVerticalVariance = 45f;
    public float potatoForce;
    public SpriteRenderer blush;
    public SpriteRenderer blush2;
    public int initialHealth = 10;
    public int slapDamage = 2;
    public int potatoDamage = 1;
    public int vodkaHealth = 1;

    public float slapHitDelay = 0.5f;
    public float drinkHitDelay = 0.5f;
    public float potatoThrowDelay = 0.5f;
    public float potatoHitDelay = 0.5f;
    public float stunTime = 0.5f;

    public int drinksToVomit = 2;
    public SpriteRenderer background;

    public GameObject eyes;
    public Sprite eyesNormal;
    public Sprite eyesDamaged;
    public Sprite eyesDead;

    Animator m_Animator;
    float m_NextActionTime;
    int m_Health;
    State m_State;
    ThrownPotato m_ThrownPotato;
    float m_InitialBackgroundAlpha;

    int m_CurrentDrinks = 0;

    Coroutine m_Slapping;
    Coroutine m_Drinking;
    Coroutine m_Throwing;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        Messenger.AddListener<PlayerController>(Events.OnSlapTry, HandleEnemySlapping);
        //Messenger.AddListener<PlayerController>(Events.OnVodkaTry, HandleEnemyDrinking);
        //Messenger.AddListener<PlayerController>(Events.OnVodkaDeflected, DrinkSpilled);
        Messenger.AddListener<PlayerController>(Events.OnStunned, SlapMissed);
        Messenger.AddListener<PlayerController>(Events.OnGameOver, GameOver);

        SetBlushOpacity(0);
        m_Health = initialHealth;
        SetState(State.Idle);
        m_NextActionTime = Time.time;
        m_InitialBackgroundAlpha = background.color.a;
    }

    void HandleEnemySlapping(PlayerController controller)
    {
        if (controller == this)
        {
            switch (m_State)
            {
                case State.Drinking:
                    // Dodge the slap and cause slapper to be stunned
                    Messenger.Broadcast<PlayerController>(Events.OnStunned, enemyPlayer);
                    break;
                case State.Thrown:
                    // Have potato deflected back at you
                    EnemyDeflectedPotato();
                    Messenger.Broadcast<PlayerController>(Events.OnPotatoDeflected, enemyPlayer);
                    break;
                case State.Slapping:
                    // Prevent damaging enemy, play thud sound
                    Messenger.Broadcast<PlayerController>(Events.OnSlapSlapped, this);
                    SetState(State.Idle);
                    break;
                case State.Throwing:
                case State.Idle:
                    // Take damage
                    Messenger.Broadcast<PlayerController>(Events.OnSlapHit, this);
                    m_Animator.SetTrigger("Slapped");
                    ScreenShake();
                    TakeDamage(slapDamage);
                    SetState(State.Idle);
                    break;
            }
        }
    }

    void SlapMissed(PlayerController controller)
    {
        if (controller == this)
        {
            SetState(State.Stunned);
            StartCoroutine(SlappingStun(stunTime));
        }
    }

    void GameOver(PlayerController controller)
    {
        SetState(State.GameOver);

        if (controller == this)
        {
            m_Animator.SetTrigger("Win");
        }
    }

    // Update is called once per frame
    void Update()
    {
        var healthPercentage = (float)m_Health / (float)initialHealth;
        var blushOpacity = 1f - healthPercentage;
        SetBlushOpacity(blushOpacity);

        //var eyesRenderer = eyes.GetComponent<SpriteRenderer>();

        //if (healthPercentage < .4f)
        //{
        //eyesRenderer.sprite = eyesDamaged;
        //}

        if (m_State == State.Stunned || m_State == State.GameOver)
            return;

        if (playerNumber == 1 && Input.GetButtonUp("Fire1") || playerNumber == 2 && Input.GetButtonUp("Fire4"))
        {
            if (Time.time > m_NextActionTime)
            {
                m_Slapping = StartCoroutine(TrySlap());
            }
        }

        if (playerNumber == 1 && Input.GetButtonUp("Fire2") || playerNumber == 2 && Input.GetButtonUp("Fire5"))
        {
            if (Time.time > m_NextActionTime)
            {
                m_Drinking = StartCoroutine(TryVodka());
            }
        }

        if (playerNumber == 1 && Input.GetButtonUp("Fire3") || playerNumber == 2 && Input.GetButtonUp("Fire6"))
        {
            if (Time.time > m_NextActionTime)
            {
                m_Throwing = StartCoroutine(TryPotato());
            }
        }      
    }

    // Broadcast that we are slapping
    // Start the ability cooldown
    // Start the slapping animation
    // Wait for an amount of time
    // (Coroutine can be stopped during this time)
    // Broadcast that the slap hit
    IEnumerator TrySlap()
    {
        m_NextActionTime = Time.time + slapCooldownTime;
        SetState(State.Slapping);

        yield return new WaitForSeconds(slapHitDelay);

        if (m_State != State.Idle)
        {
            SetState(State.Idle);
            Messenger.Broadcast<PlayerController>(Events.OnSlapTry, enemyPlayer);
        }
        
        yield return null;
    }

    // Broadcast that we are drinking
    // Start the ability cooldown
    // Start the drinking animation
    // Wait for an amount of time
    // (Coroutine can be stopped during this time)
    // Broadcast that the drink was drunk
    IEnumerator TryVodka()
    {
        m_NextActionTime = Time.time + vodkaCooldownTime;
        
        SetState(State.Drinking);
        Messenger.Broadcast<PlayerController>(Events.OnDrinkVodka, this);
        yield return new WaitForSeconds(drinkHitDelay);

        if (m_State != State.Stunned)
        {
            SetState(State.Idle);
            m_CurrentDrinks++;

            if (m_CurrentDrinks >= drinksToVomit)
            {
                Messenger.Broadcast<PlayerController>(Events.OnVomit, this);
                m_Animator.SetTrigger("Vomit");
                m_CurrentDrinks = 0;
            } else
            {
                Messenger.Broadcast<PlayerController>(Events.OnVodkaHit, this);
                Heal(vodkaHealth);
                
            }
        }

        yield return null;
    }

    // Start the ability cooldown
    // Start the throwing animation
    // Wait for an amount of time
    // Broadcast that the potato was thrown
    // Spawn potato
    // (Potato can be deflected on collision with enemy arm)
    IEnumerator TryPotato()
    {
        m_NextActionTime = Time.time + potatoCooldownTime;
        SetState(State.Throwing);

        yield return new WaitForSeconds(potatoThrowDelay);

        if (m_State != State.Idle)
        {
            SetState(State.Thrown);
            Messenger.Broadcast<PlayerController>(Events.OnThrowPotato, this);
            SpawnPotato();

            yield return new WaitForSeconds(potatoHitDelay);

            if (m_State != State.Stunned)
                SetState(State.Idle);
        }

        yield return null;
    }

    IEnumerator SlappingStun(float seconds)
    {
        m_Animator.SetBool("Stunned", true);
        Messenger.Broadcast<PlayerController>(Events.OnSlapDeflected, this);

        yield return new WaitForSeconds(seconds);

        SetState(State.Idle);
        m_Animator.SetBool("Stunned", false);
        yield return null;
    }

    IEnumerator DrinkingStun(float seconds)
    {
        m_Animator.SetTrigger("Vodkaed");

        yield return new WaitForSeconds(seconds);

        SetState(State.Idle);
        
        yield return null;
    }

    public void EndStun()
    {
        SetState(State.Idle);
    }

    void SpawnPotato()
    {
        var verticalVariance = UnityEngine.Random.Range(potatoVerticalVariance * -1, potatoVerticalVariance);
        var direction = new Vector2(potatoDirection.x * potatoForce, potatoDirection.y + verticalVariance);
        var thrownPotato = Instantiate(potatoPrefab, potatoSpawn.position, potatoSpawn.rotation);
        m_ThrownPotato.Potato = thrownPotato.GetComponent<Rigidbody2D>();
        m_ThrownPotato.Owner = this;

        m_ThrownPotato.Potato.AddForce(direction);
        m_ThrownPotato.Potato.AddTorque(UnityEngine.Random.value * 45);
    }

    void EnemyDeflectedPotato()
    {
        //StopCoroutine(m_Throwing);
        SetState(State.Idle);

        var verticalVariance = UnityEngine.Random.Range(potatoVerticalVariance * -1, potatoVerticalVariance);
        var direction = new Vector2(potatoDirection.x * potatoForce * -2, potatoDirection.y + verticalVariance);

        m_ThrownPotato.Owner = enemyPlayer;
        m_ThrownPotato.Potato.AddForce(direction);
        m_ThrownPotato.Potato.AddTorque(UnityEngine.Random.value * 45);
    }

    IEnumerator ScreenShakeDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ScreenShake();
    }

    void ScreenShake()
    {
        Camera.main.DOShakePosition(0.5f, 0.25f, 10);
        background.DOFade(0.25f, 0.075f).OnComplete(()=> background.DOFade(m_InitialBackgroundAlpha, 0.075f));
    }

    IEnumerator SetTriggerDelay(string trigger, float delay)
    {
        yield return new WaitForSeconds(delay);
        m_Animator.SetTrigger(trigger);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Potato") &&
            collision.rigidbody != m_ThrownPotato.Potato ||
            m_ThrownPotato.Owner == enemyPlayer)
        {
            if (m_State == State.Drinking)
            {
                SetState(State.Stunned);
                Messenger.Broadcast<PlayerController>(Events.OnVodkaDeflected, this);
                StartCoroutine(DrinkingStun(stunTime));
            } else
            {
                m_Animator.SetTrigger("Slapped");
                TakeDamage(potatoDamage);
                SetState(State.Idle);
            }

            ScreenShake();
            Messenger.Broadcast<PlayerController>(Events.OnPotatoHit, this);
        }
    }

    void SetState(State state)
    {
        m_State = state;
        //Debug.Log($"{playerNumber} - {m_State}");

        switch (state)
        {
            case State.Slapping:
                m_Animator.SetTrigger("Slap");
                break;
            case State.Drinking:
                m_Animator.SetTrigger("Vodka");
                break;
            case State.Throwing:
                m_Animator.SetTrigger("Potato");
                break;
            case State.Stunned:
            case State.Idle:
                break;
        }
    }

    void Heal(int amount)
    {
        m_Health += amount;

        if (m_Health > initialHealth)
        {
            m_Health = initialHealth;
        }

        UpdateEyes();
    }

    IEnumerator TakeDamageDelay(int amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        TakeDamage(amount);
    }

    void TakeDamage(int amount)
    {
        m_Health -= amount;

        if (m_Health < 0)
        {
            m_Health = 0;
        }

        if (m_Health == 0)
        {
            m_Animator.SetTrigger("Dead");
            Messenger.Broadcast<PlayerController>(Events.OnGameOver, enemyPlayer);
        }

        UpdateEyes();
    }

    void UpdateEyes()
    {
        var healthPercentage = (float)m_Health / (float)initialHealth;
        var eyesRenderer = eyes.GetComponent<SpriteRenderer>();

        if (healthPercentage >= .4f)
        {
            eyesRenderer.sprite = eyesNormal;
        }
        else if (healthPercentage > 0)
        {
            eyesRenderer.sprite = eyesDamaged;
        }
        else
        {
            eyesRenderer.sprite = eyesDead;
        }
    }

    void SetBlushOpacity(float opacity)
    {
        blush.color = new Color(blush.color.r, blush.color.g, blush.color.b, opacity);
        blush2.color = new Color(blush2.color.r, blush2.color.g, blush2.color.b, opacity);
    }

    public void BroadcastGlassDown()
    {
        Messenger.Broadcast<PlayerController>(Events.OnGlassDown, this);
    }

    public void BroadcastSlap()
    {
        Messenger.Broadcast<PlayerController>(Events.OnSlap, this);
    }

    public void BroadcastHitTable()
    {
        Messenger.Broadcast<PlayerController>(Events.OnHitTable, this);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<PlayerController>(Events.OnSlapTry, HandleEnemySlapping);
        Messenger.RemoveListener<PlayerController>(Events.OnStunned, SlapMissed);
    }
}
