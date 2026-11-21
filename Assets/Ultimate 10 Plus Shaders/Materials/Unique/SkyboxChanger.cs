using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public Material[] skyboxes; // Assign multiple skyboxes in the inspector
    private int currentSkyboxIndex = 0;
    private bool skyboxActive = false;

    void Start()
    {
        if (skyboxes.Length > 0)
            RenderSettings.skybox = skyboxes[0]; // Set an initial skybox
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag"))
        {
            ActivateAndChangeSkybox();
        }
    }

    void ActivateAndChangeSkybox()
    {
        skyboxActive = true;

        if (skyboxes.Length == 0)
        {
            Debug.LogWarning("No skyboxes assigned!");
            return;
        }

        // Cycle to the next skybox
        currentSkyboxIndex = (currentSkyboxIndex + 1) % skyboxes.Length;
        RenderSettings.skybox = skyboxes[currentSkyboxIndex];

        // Optional: Refresh lighting if needed
        DynamicGI.UpdateEnvironment();
    }
}
