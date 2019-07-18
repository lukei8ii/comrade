using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNumber == 1)
        {
            if (Input.GetButton("Fire1"))
            {
                Debug.Log("Player 1 Slap");
            }

            if (Input.GetButton("Fire2"))
            {
                Debug.Log("Player 1 Vodka");
            }

            if (Input.GetButton("Fire3"))
            {
                Debug.Log("Player 1 Potato");
            }
        } else
        {
            if(Input.GetButton("Fire4"))
            {
                Debug.Log("Player 2 Slap");
            }

            if (Input.GetButton("Fire5"))
            {
                Debug.Log("Player 2 Vodka");
            }

            if (Input.GetButton("Fire6"))
            {
                Debug.Log("Player 2 Potato");
            }
        }
        
    }
}
