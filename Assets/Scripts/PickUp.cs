using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUp : NetworkBehaviour
{
    public int respawnTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Server]
    void Effect()
    {
        gameObject.SetActive(true);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider co)
    {
        gameObject.SetActive(false);
        Invoke("Effect", respawnTime);
    }
}
