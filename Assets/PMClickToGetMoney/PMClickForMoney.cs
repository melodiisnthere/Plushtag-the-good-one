using System.Collections;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class AddCurrency : MonoBehaviour
{
    [Header("this script is made by primal monke do not steal if you do you will be sued.")]

    [Header("playfab stuff")]
    public int CurrencyAmount;
    public string CurrencyCode;
    [Space]
    [Space]
    public string HandTag;

    
    private int amountOfClicks;
    private bool canClick = true;
    [Header("Important")]
    public float clickDelay = 0.4f;
    [Header("text")]
    public TextMeshPro Text;

    [Header("this is how many clicks itll take to get the money")]
    public int HowManyClicksTillMoney;

    private Color oldColor;
    
    
   
    private IEnumerator color()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        yield return new WaitForSeconds(.5f);
        gameObject.GetComponent<Renderer>().material.color = oldColor;
    }

    private void Start()
    {
        oldColor = gameObject.GetComponent<Renderer>().material.color;
        Text.text = amountOfClicks.ToString();
    }

    private void Update()
    {
        Text.text = amountOfClicks.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HandTag) && canClick)
        {
            StartCoroutine(color());
            if (amountOfClicks == HowManyClicksTillMoney)
            {
                GrantPlayerMoney();
                amountOfClicks = 0;
                StartCoroutine(ClickDelayCoroutine());
            }
            else
            {
                amountOfClicks++;
                StartCoroutine(ClickDelayCoroutine());
            }
        }
    }

    IEnumerator ClickDelayCoroutine()
    {
        canClick = false;

        yield return new WaitForSeconds(clickDelay);

        canClick = true;
    }

    public void GrantPlayerMoney()
    {
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = CurrencyCode,
            Amount = CurrencyAmount
        };
        PlayFabClientAPI.AddUserVirtualCurrency(request, OnGrantMoneySuccess, OnError);
    }

    void OnGrantMoneySuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log("Granted Money Successfully");
    }

    public void OnError(PlayFabError error)
    {
        Debug.Log("Granting Money Unsuccessful");
    }
}
