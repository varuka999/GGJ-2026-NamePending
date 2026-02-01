using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static UIManager instance = null;
    static public UIManager Instance { get { return instance; } }

    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject uiControlsParent;
    [SerializeField] private TMP_Text displayText = null;
    [SerializeField] private TMP_Text abilityText = null;

    private List<TMP_Text> clueTexts = new List<TMP_Text>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RequestDisplayClueText(string clueString, Transform clue)
    {
        DisplayClueText(clueString, clue);
    }

    public void DisplayClueText(string clueString, Transform clue)
    {
        TMP_Text textToDisplay = Instantiate(displayText);
        textToDisplay.transform.position = (clue.position);
        textToDisplay.transform.parent = clue;
        textToDisplay.text = clueString;

        clueTexts.Add(textToDisplay);
    }

    public void ToggleUIControls(int maskIndex)
    {
        uiControlsParent.SetActive(!uiControlsParent.activeSelf);
    }

    public void UpdateControlsText(MaskType mask)
    {
        Debug.Log("updated controls text");
        abilityText.text = "(Space) ";
        switch (mask)
        {
            case MaskType.Detective:
                abilityText.text += "Detective Vision";
                break;
            case MaskType.Ghost:
                abilityText.text += "Ghost Dash";
                break;
            default:
                break;
        }
    }

    public void ToggleClueText(bool mode)
    {
        foreach (TMP_Text clue in clueTexts)
        {
            clue.gameObject.SetActive(mode);
        }
    }
}