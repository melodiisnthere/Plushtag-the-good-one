using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class delayedEnableDisable : MonoBehaviour
{
    [Header("THIS WAS MADE BY M12 DONT NEED CREDITS :)")]
    [Space]
    [Space]
    [Space]
    [Header("THE OBJECTS THAT WILL BE ENABLE OR DISABLE")]
    public GameObject[] TheObjects;
    [Space]
    [Header("THE TIME TO ENABLE OR DISABLE")]
    public float TimeForDisableAndEnable;
    [Space]
    [Header("THE TAG YOU WANT")]
    public string Tag = "HandTag";
    [Space]
    [Header("SETTING")]
    public bool Enable;
    public bool Disable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag))
        {
            StartCoroutine(EnableObject());
            StartCoroutine(DisableObject());
        }

    }


    IEnumerator DisableObject()
    {
        if (Disable == true)
        {
                yield return new WaitForSeconds(TimeForDisableAndEnable);

                foreach (GameObject obj in TheObjects)
                {
                    obj.SetActive(false);

                }
            }
        }

    IEnumerator EnableObject()
    {
        if (Enable == true)
        {
               yield return new WaitForSeconds(TimeForDisableAndEnable);

                foreach (GameObject Obj in TheObjects)
                {
                    Obj.SetActive(true);
                }
            }
        }
    }
