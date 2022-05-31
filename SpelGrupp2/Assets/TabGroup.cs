using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private Button[] tabMain, tabLaser, tabRevolver, tabColor;
    private Button[] currentArray;
    [SerializeField] private Button defaultColorButton;
    [HideInInspector] public Button selectedButton;
    private int selectedButtonIndex = 0;
    private CallbackSystem.FadingTextEvent fadingtextEvent;
    private bool isPlayerOne;
    public Dictionary<string, bool> buttonsDictionary;
    private List<Button> allButtons;

    private void Awake()
    {
        fadingtextEvent = new CallbackSystem.FadingTextEvent();
        if (buttonsDictionary == null)
            buttonsDictionary = new Dictionary<string, bool>();
        if (allButtons == null)
            allButtons = new List<Button>();
        currentArray = tabMain;
        selectedButton = tabMain[0];
        selectedButton.image.color = Color.red;
        SwitchArray(currentArray);
        CombineArrays();
        InstantiateButtons();
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
                fadingtextEvent.isPlayerOne = isPlayerOne;
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

    public void ReturnToMain()
    {
        currentArray.Equals(tabMain);
        SwitchArray(tabMain);
    }

    public void SwitchArray(Button[] switchToArray)
    {
        if (switchToArray != null)
        {
            for (int i = 0; i < currentArray.Length; i++)
                currentArray[i].gameObject.SetActive(false);
        }

        currentArray = switchToArray;
        for (int j = 0; j < currentArray.Length; j++)
            currentArray[j].gameObject.SetActive(true);

        selectedButtonIndex = 0;
        ChangeSelectedButton();
    }

    public void SetLaserPage() => SwitchArray(tabLaser);
    public void SetRevolverPage() => SwitchArray(tabRevolver);
    public void SetColorPage() => SwitchArray(tabColor);

    public Button GetDefaultColorButton() { return defaultColorButton; }

    public void SetPlayerOne(bool argument) => isPlayerOne = argument;

    private void CombineArrays()
    {
        allButtons.AddRange(tabLaser);
        allButtons.AddRange(tabRevolver);
        allButtons.AddRange(tabColor);
    }
    private void InstantiateButtons()
    {
        foreach (Button button in allButtons)
        {
            if (!buttonsDictionary.ContainsKey(button.name))
            {
                buttonsDictionary.Add(button.name, false);
            }
            button.interactable = !buttonsDictionary[button.name];
        }
    }
}
