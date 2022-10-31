using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTracker : MonoBehaviour
{
    private Dictionary<string, List<Color>> _partColors = new Dictionary<string, List<Color>>();
    private GameObject _ship;

    private void Start() {
        _ship = GetComponent<FaultHandler>().ship;

        GetChildColors();
    }

    private void GetChildColors() {
        for (int i = 0; i < _ship.transform.childCount; i++) {
            Transform child = _ship.transform.GetChild(i);
            Renderer ren = child.GetComponent<Renderer>();

            _partColors.Add(child.name, new List<Color>());

            foreach (Material mat in ren.materials) {
                _partColors[child.name].Add(mat.color);
            }
        }
    }

    public void ResetColors(Transform child) {
        int colIndex = 0;

        Renderer _renderer = child.GetComponent<Renderer>();
        foreach (Material mat in _renderer.materials) {
            mat.color = _partColors[child.name][colIndex];

            colIndex++;
        }
    }

    public void SetColor(Color c, Transform child) {
        Renderer _renderer = child.GetComponent<Renderer>();

        foreach (Material mat in _renderer.materials) {
            mat.color = c;
        }
    }
}
