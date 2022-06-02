using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private Button[] tabMain, tabLaser, tabRevolver, tabColor;
    [SerializeField] private GameObject[] mainDescriptions, laserDescriptions, revolverDescriptions, colorDescriptions;
    [SerializeField] private Button defaultColorButton;
    [HideInInspector] public Button selectedButton;
    private GameObject descriptionGO;
    private CallbackSystem.FadingTextEvent fadingtextEvent;
    private List<Button> allButtons;
    private Button[] currentArray;
    private GameObject[] currentDescriptionArray;
    private int selectedButtonIndex;
    private bool isPlayerOne;
    public Dictionary<string, bool> buttonsDictionary;

    private void Awake()
    {
        fadingtextEvent = new CallbackSystem.FadingTextEvent();
        if (buttonsDictionary == null)
            buttonsDictionary = new Dictionary<string, bool>();
        if (allButtons == null)
            allButtons = new List<Button>();
        currentArray = tabMain;
        currentDescriptionArray = mainDescriptions;
        selectedButton = tabMain[0];
        selectedButton.image.color = Color.red;
        SwitchArray(currentArray, currentDescriptionArray);
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
        selectedButton.image.color = Color.gray;
        if(descriptionGO != null)
        descriptionGO.SetActive(false);
        descriptionGO = currentDescriptionArray[selectedButtonIndex];
        descriptionGO.SetActive(true);
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
            SwitchArray(tabMain, mainDescriptions);
        }
    }

    public void ReturnToMain()
    {
        currentArray.Equals(tabMain);
        SwitchArray(tabMain, mainDescriptions);
    }

    public void SwitchArray(Button[] switchToArray, GameObject[] desciptionArray)
    {
        if (switchToArray != null)
        {
            for (int i = 0; i < currentArray.Length; i++)
                currentArray[i].gameObject.SetActive(false);
        }

        currentArray = switchToArray;
        for (int j = 0; j < currentArray.Length; j++)
            currentArray[j].gameObject.SetActive(true);

        currentDescriptionArray = desciptionArray;
        selectedButtonIndex = 0;
        ChangeSelectedButton();
    }

    public void SetLaserPage() => SwitchArray(tabLaser, laserDescriptions);
    public void SetRevolverPage() => SwitchArray(tabRevolver, revolverDescriptions);
    public void SetColorPage() => SwitchArray(tabColor, colorDescriptions);

    public Button GetDefaultColorButton() { return defaultColorButton; }

    public void SetPlayerOne(bool argument) => isPlayerOne = argument;

    private void CombineArrays()
    {
        allButtons.AddRange(tabLaser);
        allButtons.AddRange(tabRevolver);
        allButtons.AddRange(tabColor);
    }

    public void InstantiateButtons()
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
