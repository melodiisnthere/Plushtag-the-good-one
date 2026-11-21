using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromocodeButton : MonoBehaviour
{
    public char codeCharacter;
    public PromocodesManager promoCodeManager;

    public bool isSpecialCharacter = true;

    private bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag"))
        {
            isPressed = true;
            PressButton();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HandTag"))
        {
            isPressed = false;
        }
    }

    private void Update()
    {
    }

    private void PressButton()
    {
        if (isSpecialCharacter)
        {
            if (codeCharacter == 'B')
            {
                string currentText = promoCodeManager.promoCodeText.text;
                if (currentText.Length > 0)
                {
                    currentText = currentText.Substring(0, currentText.Length - 1);
                    promoCodeManager.UpdatePromoCodeText(currentText);
                }
            }
        }
        else
        {
            promoCodeManager.UpdatePromoCodeText(promoCodeManager.promoCodeText.text + codeCharacter);
        }
    }
}
