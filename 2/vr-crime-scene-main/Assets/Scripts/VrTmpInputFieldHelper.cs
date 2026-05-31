using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Поле ввода в VR: raycast на фон поля и вызов системной клавиатуры при фокусе.
/// </summary>
[RequireComponent(typeof(TMP_InputField))]
public class VrTmpInputFieldHelper : MonoBehaviour
{
    private TMP_InputField _inputField;

    private void Awake() {
        _inputField = GetComponent<TMP_InputField>();
        ApplyRaycastSettings();
    }

    private void OnEnable() {
        if (_inputField == null) {
            return;
        }

        _inputField.onSelect.AddListener(OnSelected);
    }

    private void OnDisable() {
        if (_inputField == null) {
            return;
        }

        _inputField.onSelect.RemoveListener(OnSelected);
    }

    private void ApplyRaycastSettings() {
        if (_inputField.targetGraphic != null) {
            _inputField.targetGraphic.raycastTarget = true;
        }

        if (_inputField.textComponent != null) {
            _inputField.textComponent.raycastTarget = false;
        }

        if (_inputField.placeholder is Graphic placeholderGraphic) {
            placeholderGraphic.raycastTarget = false;
        }
    }

    private void OnSelected(string _) {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!TouchScreenKeyboard.isSupported || TouchScreenKeyboard.visible) {
            return;
        }

        var placeholder = _inputField.placeholder as TMP_Text;
        TouchScreenKeyboard.Open(
            _inputField.text,
            TouchScreenKeyboardType.Default,
            false,
            false,
            false,
            false,
            placeholder != null ? placeholder.text : string.Empty);
#endif
    }
}
