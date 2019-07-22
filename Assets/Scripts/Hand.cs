using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hand : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit()
    {
        Messenger.Broadcast<GameObject>(Events.OnSlam, this.gameObject);
        Camera.main.DOShakePosition(0.5f, 0.25f, 10);
    }
}
