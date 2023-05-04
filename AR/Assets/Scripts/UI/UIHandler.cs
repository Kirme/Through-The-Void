using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour {
    [SerializeField] TextMeshProUGUI _textTimeHeld;
    [SerializeField] TextMeshProUGUI _textInfo;

    static class Mistakes {
        public const int WrongPanel = 0;
        public const int NotLongEnough = 1;
        public const int TooLong = 2;
        public const int Error = -1;
    }

    private void Start() {
        _textTimeHeld.gameObject.SetActive(false);
        _textInfo.gameObject.SetActive(false);
    }

    public void SetTimeHeld(float timeHeld) {
        float margin = 0.2f;

        if (timeHeld < margin) {
            _textTimeHeld.gameObject.SetActive(false);
        }
        else if (!_textTimeHeld.IsActive()) {
            _textTimeHeld.gameObject.SetActive(true);
        }

        _textTimeHeld.text = timeHeld.ToString("0.00");
    }

    public void SetMistake() {
        string txt = "You used the wrong panel,\nthe ship took damage...";

        Show(txt);
    }

    public void SetMistake(Panel panel, float timeHeld) {
        int type = GetMistakeType(panel, timeHeld);
        string txt;
        switch (type) {
            case Mistakes.WrongPanel:
                txt = "You pressed the wrong panel,\nthe ship took damage...";
                break;
            case Mistakes.NotLongEnough:
                txt = "You didn\'t hold long enough,\nthe ship took damage...";
                break;
            case Mistakes.TooLong:
                txt = "You held too long,\nthe ship took damage...";
                break;
            default:
                txt = "An unknown mistake occurred";
                break;
        }

        Show(txt);
    }

    public void Show(string txt) {
        _textInfo.text = txt;

        StartCoroutine(ShowText(3));
    }

    IEnumerator ShowText(int secondsToShow) {
        _textInfo.gameObject.SetActive(true);
        yield return new WaitForSeconds(secondsToShow);
        _textInfo.gameObject.SetActive(false);
    }

    /*
     * Get mistake type
     * 0 - Wrong panel
     * 1 - didn't hold long enough
     * 2 - held too long
     */
    private int GetMistakeType(Panel panel, float timeHeld) {
        if (!panel.canRepair) {
            return Mistakes.WrongPanel;
        }
        float tth = panel.timeToHold;

        if (timeHeld < tth) {
            return Mistakes.NotLongEnough;
        }

        if (timeHeld > tth) {
            return Mistakes.TooLong;
        }

        return Mistakes.Error;
    }
}
