using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UpdatedNetworkManager : NetworkManager
{

    public List<GameObject> playerModels;

    public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
    {

        if (LogFilter.Debug) Debug.Log("NetworkManager.OnServerAddPlayer");

        if (playerPrefab == null)
        {
            Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object.");
            return;
        }

        if (playerPrefab.GetComponent<NetworkIdentity>() == null)
        {
            Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab.");
            return;
        }

        if (conn.playerController != null)
        {
            Debug.LogError("There is already a player for this connections.");
            return;
        }

        if (playerModels == null) playerModels = new List<GameObject>();
        //change the below to pick a prefab to spawn in
        int modelPick = Random.Range(0, playerModels.Count);
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerModels[modelPick], startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        NetworkServer.AddPlayerForConnection(conn, player);
        playerModels.RemoveAt(modelPick);

    }

}
