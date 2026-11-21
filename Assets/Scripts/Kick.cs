using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Kick : NetworkBehaviour
{
    public NetworkIdentity playerIdentity;

    void OnTriggerEnter(Collider other)
    {
        // Only the server should handle the detection and disconnection logic
        if (!isServer) return;

        NetworkIdentity otherIdentity = other.GetComponent<NetworkIdentity>();

        if (otherIdentity == null) return;

        // Prevent the owner of this object from kicking themselves (optional)
        if (otherIdentity == playerIdentity) return;

        // Ensure the target actually has a valid client connection
        if (otherIdentity.connectionToClient == null) return;

        StartCoroutine(KickWithDelay(otherIdentity.connectionToClient));
    }

    IEnumerator KickWithDelay(NetworkConnectionToClient conn)
    {
        yield return new WaitForSeconds(0.1f);

        // Send the RPC to the specific target client
        TargetQuitGame(conn);

        // Give the client a brief moment to process the quit command before cutting the connection
        yield return new WaitForSeconds(0.5f);
        conn.Disconnect();
    }

    // Removed the [Server] attribute so the client can execute this code
    [TargetRpc]
    void TargetQuitGame(NetworkConnectionToClient conn)
    {
        // This will close the application on the target player's machine
        // Note: This only works in a standalone build, not inside the Unity Editor play mode.
        Application.Quit();
    }
}