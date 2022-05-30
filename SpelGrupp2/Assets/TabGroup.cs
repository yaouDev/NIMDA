using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private Button[] tabMain, tabLaser, tabRevolver, tabColor;
    private Button[] currentArray;
    private Button selectedButton;
    private int selectedButtonIndex = 0;
    private CallbackSystem.FadingTextEvent fadingtextEvent;

    private void Awake()
    {
        fadingtextEvent = new CallbackSystem.FadingTextEvent();
        currentArray = tabMain;
        selectedButton = tabMain[0];
        selectedButton.image.color = Color.red;
    }
    public void NextButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectedButtonIndex++;
            if (selectedButtonIndex == currentArray.Length)
                selectedButtonIndex = 0;

            ChangeSelectedButton();
        }
    }

    public void PreviousButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectedButtonIndex--;
            if (selectedButtonIndex < 0)
                selectedButtonIndex = currentArray.Length - 1;

            ChangeSelectedButton();
        }
    }

    private void ChangeSelectedButton()
    {
        selectedButton.image.color = Color.white;
        selectedButton = currentArray[selectedButtonIndex];
        selectedButton.image.color = Color.red;
    }

    public void SelectButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (selectedButton.interactable)
                selectedButton.onClick.Invoke();
            else
            {
                fadingtextEvent.text = "Unavailable Purchase";
                CallbackSystem.EventSystem.Current.FireEvent(fadingtextEvent);
            }
        }
    }

    public void ReturnToMain(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentArray.Equals(tabMain);
            SwitchArray(tabMain);
        }
    }

    public void SwitchArray(Button[] switchToArray)
    {
        if (switchToArray == null)
            Debug.Log("switchToArray is null");
        for(int i = 0; i < currentArray.Length; i++)
            currentArray[i].gameObject.SetActive(false);
        
        currentArray = switchToArray;
        for(int j = 0; j < currentArray.Length; j++)     
            currentArray[j].gameObject.SetActive(true);          
        
        selectedButtonIndex = 0;
        ChangeSelectedButton();
    }

    public void SetLaserPage() => SwitchArray(tabLaser);
    public void SetRevolvePage() => SwitchArray(tabRevolver);
    public void SetColorPage() => SwitchArray(tabColor);
}
