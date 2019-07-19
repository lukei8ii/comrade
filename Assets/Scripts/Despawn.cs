using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    public float lifeSpan = 5f;

    float despawnAt;

    // Start is called before the first frame update
    void Start()
    {
        despawnAt = Time.time + lifeSpan;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= despawnAt)
        {
            Destroy(this.gameObject);
        }
    }
}
