using UnityEngine;
using Mirror;
using Mirror.VR;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

public class LeaderBoard : NetworkBehaviour
{
    public TMP_Text names;
    public List<MeshRenderer> colorSlots;
    public string WebHookURL;
    public Playfablogin pfl;

    private void FixedUpdate()
    {
        for (int i = 0; i < MirrorVRManager.PlayerList.Count; i++)
        {
            if (i <= MirrorVRManager.PlayerList.Count)
            {
                names.text = "";
                if (i != 0)
                {
                    names.text += "\n" + MirrorVRManager.PlayerList[i].PlayerName;
                }
                else
                {
                    names.text += MirrorVRManager.PlayerList[i].PlayerName;
                }
                colorSlots[i].material.color = MirrorVRManager.PlayerList[i].PlayerColor;
            }
        }
    }

    public void Mute(int i)
    {
        AudioSource mic = MirrorVRManager.PlayerList[i].GetComponentInChildren<AudioSource>();
        if (mic.volume == 1f)
        {
            mic.volume = 0f;
        }
        else
        {
            mic.volume = 1f;
        }
    }

    public void Report(int i)
    {
        string message = $"Player: {MirrorVRManager.PlayerList[i].PlayerName} was reported by {MirrorVRManager.GetUsername()} (PLAYFAB: {pfl.MyPlayFabID}) in room code: {MirrorVRManager.GetLobbyCode()}";
        StartCoroutine(PostToDiscord(message));
    }

    IEnumerator PostToDiscord(string message)
    {
        string jsonPayload = "{\"content\": \"" + message + "\"}";
        using (UnityWebRequest www = new UnityWebRequest(WebHookURL, "POST"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
        }
    }
}