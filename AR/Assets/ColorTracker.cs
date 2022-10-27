using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTracker : MonoBehaviour
{
    List<Color> _colors = new List<Color>();
    Renderer _renderer;

    private void Start() {
        _renderer = GetComponent<Renderer>();

        foreach (Material mat in _renderer.materials) {
            _colors.Add(mat.color);
        }
    }

    public void ResetColors() {
        int colIndex = 0;
        foreach (Material mat in _renderer.materials) {
            mat.color = _colors[colIndex];

            colIndex++;
        }
    }

    public void SetColor(Color c) {
        foreach (Material mat in _renderer.materials) {
            mat.color = c;
        }
    }
}
