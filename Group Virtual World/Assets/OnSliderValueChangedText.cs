using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class OnSliderValueChangedText : MonoBehaviour {

    private InputField valueText;

    private void OnValidate() {
        valueText = GetComponent<InputField>();
    }

    private void Start() {
        valueText = GetComponent<InputField>();
    }

    public void callback(float value) {
        if (value > 100) value = 100;
        else if (value < 0) value = 0;

        valueText.text = value.ToString("0");

    }

}
