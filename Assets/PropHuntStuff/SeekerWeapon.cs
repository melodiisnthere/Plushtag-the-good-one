using UnityEngine;
using Mirror;

public class SeekerWeapon : NetworkBehaviour
{
    [Header("Weapon Configuration")]
    public Transform muzzlePoint;
    public float range = 50f;
    public int damage = 100;
    public LayerMask shootableLayers;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource fireAudio;

    public void Shoot()
    {
        if (!isLocalPlayer) return;

        // BLOCK SHOOTING outside of Seeking Phase
        if (PropHuntManager.Instance != null && PropHuntManager.Instance.currentPhase != PropHuntPhase.SeekingPhase)
        {
            Debug.Log("[Weapon] Cannot shoot outside of the Seeking Phase!");
            return;
        }

        CmdFireWeapon(muzzlePoint.position, muzzlePoint.forward);
    }

    [Command]
    private void CmdFireWeapon(Vector3 origin, Vector3 direction)
    {
        if (PropHuntManager.Instance != null && PropHuntManager.Instance.currentPhase != PropHuntPhase.SeekingPhase)
        {
            return;
        }

        RpcPlayShootEffects();

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, shootableLayers))
        {
            PropHuntPlayer hitPlayer = hit.collider.GetComponentInParent<PropHuntPlayer>();
            if (hitPlayer != null && !hitPlayer.isSeeker)
            {
                hitPlayer.ServerTakeDamage(damage, netIdentity);
            }
        }
    }

    [ClientRpc]
    private void RpcPlayShootEffects()
    {
        if (muzzleFlash != null) muzzleFlash.Play();
        if (fireAudio != null) fireAudio.Play();
    }
}