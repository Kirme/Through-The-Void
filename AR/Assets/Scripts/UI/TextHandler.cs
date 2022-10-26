using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextHandler : MonoBehaviour
{
    private TextMeshPro _text;
    private Information _info;

    struct Information {
        public bool showDesc;
        public string partName;
        public string desc;

        public string GetText() {
            List<string> strList = new List<string>();
            strList.Add(partName);

            if (showDesc) {
                strList.Add("\n\n");
                strList.Add(desc);
            }

            return string.Concat(strList);
        }
    }

    void Awake() {
        _info.partName = gameObject.transform.parent.name;
        _info.showDesc = false;

        _text = GetComponentInChildren<TextMeshPro>();

        _text.text = _info.GetText();
        _text.gameObject.SetActive(false);
    }

    public void SetDescription(string txt) {
        _info.desc = txt;
    }

    public void ShowDescription(bool setTo) {
        _info.showDesc = setTo;
        _text.text = _info.GetText();
    }

    public void Show()
    {
        _text.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _text.gameObject.SetActive(false);
    }
}
