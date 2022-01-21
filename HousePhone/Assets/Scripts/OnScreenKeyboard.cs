using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenKeyboard : MonoBehaviour
{
    [SerializeField] GameObject keyboard;
    [SerializeField] TMPro.TMP_Text shownText;
    [SerializeField] TMPro.TMP_InputField inputField;

    public void Open() => keyboard.SetActive(true);
    public void Close() => keyboard.SetActive(false);

    public void AddCharacter(string c)
    {
        inputField.text += c[0];
        shownText.text = inputField.text;
    }
    public void BackSpace()
    {
        inputField.text = inputField.text.Remove(inputField.text.Length - 1);
        shownText.text = inputField.text;
    }

    public void UpdateTextShown(string str)
    {
        shownText.text = str;
    }
}
