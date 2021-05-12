using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
public class NetworkManagerPokemon : NetworkManager
{

    public Transform leftRacketSpawn;
    public Transform rightRacketSpawn;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // add player at correct spawn position
        Transform start = numPlayers % 2 == 0 ? leftRacketSpawn : rightRacketSpawn;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public void Respawn(GameObject player, NetworkConnection conn)
    {
        Destroy(player);
        Transform start = numPlayers % 2 == 0 ? leftRacketSpawn : rightRacketSpawn;
        GameObject player2 = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player2);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }

}
