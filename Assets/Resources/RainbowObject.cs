using UnityEngine;

public class RainbowObject : MonoBehaviour
{
    [Header("Give credits if you want")]
    [Header("Script made by Golonka")]

    public float Speed = 1.0f;

    public Material material;
    private float hueValue = 0.0f;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            material = renderer.material;
        }
        else
        {
            Debug.LogError("No material or renderer component lol");
        }
    }

    void Update()
    {
        if (material == null)
            return;

        hueValue += Speed * Time.deltaTime;

        if (hueValue >= 1.0f)
            hueValue -= 1.0f;

        Color newColor = Color.HSVToRGB(hueValue, 1.0f, 1.0f);
        material.color = newColor;
    }
}
