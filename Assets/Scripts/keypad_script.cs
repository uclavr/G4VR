using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq.Expressions;

public class keypad_script : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField inputField;
    private string text = "";
    public GameObject KeypadWindow;
    
    public void getDigit(string digit)
    {
        text = null;
        text+=digit;
        UpdateDisplay(text);
    }

    private void UpdateDisplay(string T)
    {
        inputField.text += T;
    }

    public void Delete()
    {
        string str = inputField.GetComponent<TMP_InputField>().text;
        if (str.Length>0)
        {
            string newStr = str.Substring(0, str.Length - 1);
            inputField.text = newStr;
        }
    }

    public void ShowWindow()
    {
        if (KeypadWindow.activeSelf == false)
            KeypadWindow.SetActive(true);
        else
            KeypadWindow.SetActive(false);
    }

    public void HideWindow()
    
    {
        if (KeypadWindow.activeSelf == true)
            KeypadWindow.SetActive(false);
    }

    public void setIF(TMP_InputField newIF)
     {
        inputField = newIF;
        }
}
   