using UnityEngine;
using Mirror.VR;

public class LBButton : MonoBehaviour
{
    [Header("Type: 0 = Mute | 1 = Report")]
    public LeaderBoard lb;
    public int type = 0;
    public int button = 0;
    public string handTag = "HandTag";

    private void FixedUpdate()
    {
        if (MirrorVRManager.GetUsername() == MirrorVRManager.PlayerList[button].PlayerName)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(handTag))
        {
            if (type == 0)
            {
                lb.Mute(button);
            }
            if (type == 1)
            {
                lb.Report(button);
            }
        }
    }
}