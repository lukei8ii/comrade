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
        Stunned
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

    Animator m_Animator;
    float m_NextActionTime;
    int m_Health;
    State m_State;
    ThrownPotato m_ThrownPotato;

    IEnumerator m_Slapping;
    IEnumerator m_Drinking;
    IEnumerator m_Throwing;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        Messenger.AddListener<PlayerController>(Events.OnSlapHit, SlapHit);
        Messenger.AddListener<PlayerController>(Events.OnStunned, Stunned);

        SetBlushOpacity(0);
        m_Health = initialHealth;
        m_State = State.Idle;
        m_NextActionTime = Time.time;
    }

    void SlapHit(PlayerController controller)
    {
        if (controller == this)
        {
            switch (m_State)
            {
                case State.Drinking:
                    // Dodge the slap and cause slapper to be stunned
                    Messenger.Broadcast<PlayerController>(Events.OnStunned, enemyPlayer);
                    break;
                case State.Throwing:
                    // Have potato deflected back at you
                    EnemyDeflectedPotato();
                    Messenger.Broadcast<PlayerController>(Events.OnPotatoDeflected, enemyPlayer);
                    break;
                case State.Slapping:
                    StopActions();
                    // Take damage
                    m_Animator.SetTrigger("Slapped");
                    ScreenShake();
                    TakeDamage(slapDamage);
                    break;
                case State.Idle:
                    // Take damage
                    m_Animator.SetTrigger("Slapped");
                    ScreenShake();
                    TakeDamage(slapDamage);
                    break;
            }
        }
    }

    void Stunned(PlayerController controller)
    {
        if (controller == this)
        {
            StopActions();
            StartCoroutine(Stun(stunTime));
        }
    }

    // Update is called once per frame
    void Update()
    {
        var blushOpacity = 1f - (float)m_Health / (float)initialHealth;
        SetBlushOpacity(blushOpacity);

        if (playerNumber == 1 && Input.GetButtonUp("Fire1") || playerNumber == 2 && Input.GetButtonUp("Fire4"))
        {
            if (Time.time > m_NextActionTime)
            {
                m_Slapping = TrySlap();
                StartCoroutine(m_Slapping);
            }
        }

        if (playerNumber == 1 && Input.GetButtonUp("Fire2") || playerNumber == 2 && Input.GetButtonUp("Fire5"))
        {
            if (Time.time > m_NextActionTime)
            {
                m_Drinking = TryVodka();
                StartCoroutine(m_Drinking);
            }
        }

        if (playerNumber == 1 && Input.GetButtonUp("Fire3") || playerNumber == 2 && Input.GetButtonUp("Fire6"))
        {
            if (Time.time > m_NextActionTime)
            {
                m_Throwing = TryPotato();
                StartCoroutine(m_Throwing);
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
        m_State = State.Slapping;
        m_Animator.SetTrigger("Slap");
        Messenger.Broadcast<PlayerController>(Events.OnSlap, this);
        yield return new WaitForSeconds(slapHitDelay);
        Messenger.Broadcast<PlayerController>(Events.OnSlapHit, enemyPlayer);

        // Reset state
        m_State = State.Idle;
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
        m_Animator.SetTrigger("Vodka");
        m_State = State.Drinking;
        Messenger.Broadcast<PlayerController>(Events.OnDrinkVodka, this);
        yield return new WaitForSeconds(drinkHitDelay);
        Messenger.Broadcast<PlayerController>(Events.OnVodkaHit, this);
        Heal(vodkaHealth);

        // Reset state
        m_State = State.Idle;
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
        m_Animator.SetTrigger("Potato");
        yield return new WaitForSeconds(potatoThrowDelay);
        Messenger.Broadcast<PlayerController>(Events.OnThrowPotato, this);
        SpawnPotato();

        m_State = State.Throwing;
        yield return new WaitForSeconds(potatoHitDelay);
        m_State = State.Idle;
    }

    IEnumerator Stun(float seconds)
    {
        m_Animator.SetTrigger("Stunned");
        m_State = State.Stunned;
        yield return new WaitForSeconds(seconds);
        m_State = State.Idle;
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
        StopCoroutine(m_Throwing);
        m_State = State.Idle;

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
    }

    IEnumerator SetTriggerDelay(string trigger, float delay)
    {
        yield return new WaitForSeconds(delay);
        m_Animator.SetTrigger(trigger);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Potato") && collision.rigidbody != m_ThrownPotato.Potato || m_ThrownPotato.Owner == enemyPlayer)
        {
            Messenger.Broadcast<PlayerController>(Events.OnPotatoHit, this);
            m_Animator.SetTrigger("Slapped");
            ScreenShake();
            TakeDamage(potatoDamage);

            StopActions();
        }
    }

    void StopActions()
    {
        if (m_Slapping != null)
            StopCoroutine(m_Slapping);

        if (m_Drinking != null)
            StopCoroutine(m_Drinking);

        if (m_Throwing != null)
            StopCoroutine(m_Throwing);

        m_State = State.Idle;
    }

    void Heal(int amount)
    {
        m_Health += amount;

        if (m_Health > initialHealth)
        {
            m_Health = initialHealth;
        }
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
            Messenger.Broadcast<PlayerController>(Events.OnGameOver, enemyPlayer);
        }
    }

    void SetBlushOpacity(float opacity)
    {
        blush.color = new Color(blush.color.r, blush.color.g, blush.color.b, opacity);
        blush2.color = new Color(blush2.color.r, blush2.color.g, blush2.color.b, opacity);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<PlayerController>(Events.OnSlapHit, SlapHit);
        Messenger.RemoveListener<PlayerController>(Events.OnStunned, Stunned);
    }
}
