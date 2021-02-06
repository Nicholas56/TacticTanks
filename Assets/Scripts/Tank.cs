using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class Tank : NetworkBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;

    [Header("Movement")]
    public float rotationSpeed = 100;
    [SyncVar(hook =nameof(SetSpeed))]
    public int moveSpeed = 8;

    [Header("Firing")]
    public KeyCode shootKey = KeyCode.Space;
    public GameObject projectilePrefab;
    public Transform projectileMount;

    [Header("Health and UI")]
    [SyncVar(hook = nameof(SetHealth))]
    public int healthValue = 5;
    public TextMeshPro healthTxt;
    GameObject localUI;
    TMP_Text loseTxt;

    public override void OnStartServer()
    {
        base.OnStartServer();
        SetHealth(healthValue);
        SetSpeed(moveSpeed);
    }

    void SetSpeed(int newSpeed)
    {
        agent.speed = newSpeed;
    }

    void SetHealth(int newHealth)
    {
        string healthStr = "";
        for(int i=1;i<=newHealth;i++)
        {
            healthStr += "-";
        }
        healthTxt.text = healthStr;
        if (newHealth <= 0)
        {
            healthTxt.text = "Lose";
            if(isLocalPlayer)
            {
                GetComponent<Tank>().enabled = false;
            }
        }
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            loseTxt = GameObject.Find("loseTxt").GetComponent<TMP_Text>();
            CameraFollow360.player = transform;
        }
        
    }

    void Update()
    {
        // movement for local player
        if (!isLocalPlayer) return;

        // rotate
        float horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);

        // move
        float vertical = Input.GetAxis("Vertical");
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        agent.velocity = forward * Mathf.Max(vertical, 0) * agent.speed;
        
        // shoot
        if (Input.GetKeyDown(shootKey))
        {
            CmdFire();
        }
    }

    // this is called on the server
    [Command]
    void CmdFire()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, transform.rotation);
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());
        NetworkServer.Spawn(projectile);
        RpcOnFire();
    }

    // this is called on the tank that fired for all observers
    [ClientRpc]
    void RpcOnFire()
    {
        
    }

    [Command]
    public void CmdChangeHealth(int amount)
    {
        healthValue = healthValue + amount;
        if (isLocalPlayer)
        {
            if (healthValue <= 0)
            {
            Debug.Log("You Lose");
                loseTxt.text = "You Lose";
                GetComponent<Tank>().enabled = false;
            }
        }
    }

    [Command]
    public void CmdChangeSpeed(int amount)
    {
        moveSpeed = moveSpeed + amount;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (isLocalPlayer && collision.gameObject.tag == "Bullet")
        {
            Debug.Log("hit "+gameObject);
            CmdChangeHealth(-1);
        }
        if (isLocalPlayer && collision.gameObject.tag == "Health")
        {
            Debug.Log("hit " + gameObject);
            CmdChangeHealth(1);
        }
        if (isLocalPlayer && collision.gameObject.tag == "Speed")
        {
            Debug.Log("hit " + gameObject);
            CmdChangeSpeed(2);
        }
    }
}


