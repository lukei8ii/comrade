using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerNumber;

    public PlayerController enemyPlayer;

    private Animator m_Animator;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        Messenger.AddListener<PlayerController>(Events.OnSlap, Slapped);
    }

    void Slapped(PlayerController controller)
    {
        if (controller == this)
        {
            m_Animator.SetTrigger("Slapped");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNumber == 1)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                m_Animator.SetTrigger("Slap");
                Messenger.Broadcast<PlayerController>(Events.OnSlap, enemyPlayer);
            }

            if (Input.GetButtonUp("Fire2"))
            {
                Debug.Log("Player 1 Vodka");
            }

            if (Input.GetButtonUp("Fire3"))
            {
                Debug.Log("Player 1 Potato");
            }
        } else
        {
            if(Input.GetButtonUp("Fire4"))
            {
                Debug.Log("Player 2 Slap");
            }

            if (Input.GetButtonUp("Fire5"))
            {
                Debug.Log("Player 2 Vodka");
            }

            if (Input.GetButtonUp("Fire6"))
            {
                Debug.Log("Player 2 Potato");
            }
        }
        
    }
}
