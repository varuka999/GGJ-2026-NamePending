using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    static public UIManager Instance { get { return instance; } }

    public event Action<string, Transform> DisplayClueTextEvent;
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject uiControlsParent;
    [SerializeField] private TMP_Text displayText = null;
    [SerializeField] private TMP_Text abilityText = null;

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

        //DisplayClueTextEvent += DisplayClueText;
    }

    private void OnDisable()
    {
        //DisplayClueTextEvent -= DisplayClueText;
    }

    public void RequestDisplayClueText(string clueString, Transform clue)
    {
        //DisplayClueTextEvent?.Invoke(clueString, clue);
        DisplayClueText(clueString, clue);
    }

    public void DisplayClueText(string clueString, Transform clue)
    {
        TMP_Text textToDisplay = Instantiate(displayText, clue.transform);
        //textToDisplay.transform.position = Camera.main.WorldToScreenPoint(clue.position) + new Vector3(50.0f, 50.0f, 0f);
        textToDisplay.text = clueString;
    }

    public void ToggleUIControls(int maskIndex)
    {
        uiControlsParent.SetActive(!uiControlsParent.activeSelf);
    }

    public void UpdateControlsText(int maskIndex)
    {
        Debug.Log("updated controls text");
        abilityText.text = "(Space) ";
        switch (maskIndex)
        {
            case 0:
                abilityText.text += "Detective Vision";
                break;
            case 1:
                abilityText.text += "Ghost Dash";
                break;
            default:
                break;
        }
    }
}