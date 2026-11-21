using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;

namespace Oculus.Platform.EntitlementCheck
{
    public class AppEntitlementCheck : MonoBehaviour
    {
        public string oculusPlayerID;
        public static AppEntitlementCheck instance { get; private set; }

        void Awake()
        {
            try
            {
                Core.AsyncInitialize();
                Entitlements.IsUserEntitledToApplication().OnComplete(EntitlementCallback);
                Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
            }
            catch (UnityException e)
            {
                Debug.LogError("Platform failed to initialize due to exception.");
                Debug.LogException(e);
            }
        }

        void EntitlementCallback(Message msg)
        {
            if (msg.IsError)
            {
                Debug.LogError("You are NOT entitled to use this app.");
            }
            else
            {
                Debug.Log("You are entitled to use this app.");
            }
        }

        private void GetLoggedInUserCallback(Message<User> message)
        {
            if (message.IsError)
            {
                Debug.LogError("Failed to get logged-in user: " + message.GetError().Message);
            }
            else
            {
                oculusPlayerID = message.Data.ID.ToString();
                Debug.Log("Oculus Player ID: " + oculusPlayerID);

            }
        }
    }
}