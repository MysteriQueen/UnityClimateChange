using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnUISliderValueChanged : MonoBehaviour {

    private InputField inputField;
    private Slider slider;

    private void OnValidate() {
        Start();
    }

    private void Start() {
        inputField = GetComponentInChildren<InputField>();
        slider = GetComponentInChildren<Slider>();

        inputField.onValueChanged.Invoke(inputField.text);
        
    }

    public void sliderCallback(float value) {
        inputField.text = value.ToString("0");

    }

    public void textboxCallback(string str) {
        float value = float.Parse(str);

        if (value > 100) value = 100;
        else if (value < 0) value = 0;

        slider.value = value;
        inputField.text = value.ToString("0");

    }

}
