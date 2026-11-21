using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Mirror.VR;

public class LeaveLobbyOnExit : MonoBehaviour
{
    private bool _hasDisconnected = false;

    private void OnApplicationQuit()
    {
        DisconnectIg();
    }

    private void OnDestroy()
    {
        DisconnectIg();
    }
#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            DisconnectIg();
        }
    }
#endif

    private void DisconnectIg()
    {
        if (!_hasDisconnected)
        {
            MirrorVRManager.Disconnect();
            _hasDisconnected = true;
        }
    }
}
