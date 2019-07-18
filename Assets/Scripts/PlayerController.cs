using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
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

    private Animator m_Animator;
    private float nextSlapTime = 0;
    private float nextVodkaTime = 0;
    private float nextPotatoTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        Messenger.AddListener<PlayerController>(Events.OnSlap, Slapped);
        Messenger.AddListener<PlayerController>(Events.OnDrinkVodka, Vodkaed);
        Messenger.AddListener<PlayerController>(Events.OnThrowPotato, Potatoed);
    }

    void Slapped(PlayerController controller)
    {
        if (controller == this)
        {
            StartCoroutine(SetTriggerDelay("Slapped", 0.25f));
            StartCoroutine(ScreenShake(0.25f));
        }
    }

    void Vodkaed(PlayerController controller)
    {
        if (controller == this)
        {
            // TODO: see if anything should happen
            //m_Animator.SetTrigger("Vodkaed");
        }
    }

    void Potatoed(PlayerController controller)
    {
        if (controller == this)
        {
            // TODO: see if anything should happen
            //m_Animator.SetTrigger("Potatoed");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNumber == 1 && Input.GetButtonUp("Fire1") || playerNumber == 2 && Input.GetButtonUp("Fire4"))
        {
            if (Time.time > nextSlapTime)
            {
                m_Animator.SetTrigger("Slap");
                Messenger.Broadcast<PlayerController>(Events.OnSlap, enemyPlayer);
                nextSlapTime = Time.time + slapCooldownTime;
            }
            
        }

        if (playerNumber == 1 && Input.GetButtonUp("Fire2") || playerNumber == 2 && Input.GetButtonUp("Fire5"))
        {
            if (Time.time > nextVodkaTime)
            {
                m_Animator.SetTrigger("Vodka");
                Messenger.Broadcast<PlayerController>(Events.OnDrinkVodka, enemyPlayer);
                nextVodkaTime = Time.time + vodkaCooldownTime;
            }
        }

        if (playerNumber == 1 && Input.GetButtonUp("Fire3") || playerNumber == 2 && Input.GetButtonUp("Fire6"))
        {
            if (Time.time > nextPotatoTime)
            {
                m_Animator.SetTrigger("Potato");
                StartCoroutine(SpawnPotato());
                Messenger.Broadcast<PlayerController>(Events.OnThrowPotato, enemyPlayer);
                nextPotatoTime = Time.time + potatoCooldownTime;
            }
        }      
    }

    IEnumerator SpawnPotato()
    {
        yield return new WaitForSeconds(1.2f);
        var verticalVariance = UnityEngine.Random.Range(potatoVerticalVariance * -1, potatoVerticalVariance);
        var direction = new Vector2(potatoDirection.x * potatoForce, potatoDirection.y + verticalVariance);
        var thrownPotato = Instantiate(potatoPrefab, potatoSpawn.position, potatoSpawn.rotation);

        thrownPotato.GetComponent<Rigidbody2D>().AddForce(direction);
        thrownPotato.GetComponent<Rigidbody2D>().AddTorque(UnityEngine.Random.value * 5);
    }

    IEnumerator ScreenShake(float delay)
    {
        yield return new WaitForSeconds(delay);
        Camera.main.DOShakePosition(0.5f, 0.25f, 10);
    }

    IEnumerator SetTriggerDelay(string trigger, float delay)
    {
        yield return new WaitForSeconds(delay);
        m_Animator.SetTrigger(trigger);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<PlayerController>(Events.OnSlap, Slapped);
        Messenger.RemoveListener<PlayerController>(Events.OnDrinkVodka, Vodkaed);
        Messenger.RemoveListener<PlayerController>(Events.OnThrowPotato, Potatoed);
    }
}
