using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Переключение нескольких 360° панорам одной сцены в VR.
/// Левый контроллер: Y (Secondary) — предыдущее фото, Grip — следующее.
/// </summary>
public class PhotoSwitcher : MonoBehaviour
{
    [SerializeField] private TMP_Text _photoCounterText;
    [SerializeField] private float _stickDeadZone = 0.75f;
    [SerializeField] private float _switchCooldownSec = 0.35f;

    private InputAction _previousPhotoAction;
    private InputAction _nextPhotoAction;
    private float _lastSwitchTime;

    private void Awake() {
        _previousPhotoAction = new InputAction(
            name: "PreviousPhoto",
            type: InputActionType.Button,
            binding: "<XRController>{LeftHand}/secondaryButton");

        _nextPhotoAction = new InputAction(
            name: "NextPhoto",
            type: InputActionType.Button,
            binding: "<XRController>{LeftHand}/gripButton");
    }

    private void OnEnable() {
        _previousPhotoAction.Enable();
        _nextPhotoAction.Enable();
        _previousPhotoAction.performed += OnPreviousPerformed;
        _nextPhotoAction.performed += OnNextPerformed;
    }

    private void OnDisable() {
        _previousPhotoAction.performed -= OnPreviousPerformed;
        _nextPhotoAction.performed -= OnNextPerformed;
        _previousPhotoAction.Disable();
        _nextPhotoAction.Disable();
    }

    private void OnDestroy() {
        _previousPhotoAction?.Dispose();
        _nextPhotoAction?.Dispose();
    }

    private void Start() {
        if (_photoCounterText == null) {
            _photoCounterText = FindObjectsOfType<TMP_Text>(true)
                .FirstOrDefault(text => text.name.Contains("PhotoCounter", StringComparison.OrdinalIgnoreCase));
        }

        UpdatePhotoCounter();
        UpdateSkybox();
    }

    private void Update() {
        if (CurrentState.ScenePhotos.Count <= 1) return;
        if (Time.unscaledTime - _lastSwitchTime < _switchCooldownSec) return;

        var leftStick = ReadLeftThumbstick();
        if (leftStick.x <= -_stickDeadZone) {
            PreviousPhoto();
            _lastSwitchTime = Time.unscaledTime;
        } else if (leftStick.x >= _stickDeadZone) {
            NextPhoto();
            _lastSwitchTime = Time.unscaledTime;
        }
    }

    private void OnPreviousPerformed(InputAction.CallbackContext _) => PreviousPhoto();

    private void OnNextPerformed(InputAction.CallbackContext _) => NextPhoto();

    public void NextPhoto() {
        if (CurrentState.ScenePhotos.Count == 0) return;

        CurrentState.CurrentPhotoIndex++;
        if (CurrentState.CurrentPhotoIndex >= CurrentState.ScenePhotos.Count) {
            CurrentState.CurrentPhotoIndex = 0;
        }

        UpdateSkybox();
        UpdatePhotoCounter();
    }

    public void PreviousPhoto() {
        if (CurrentState.ScenePhotos.Count == 0) return;

        CurrentState.CurrentPhotoIndex--;
        if (CurrentState.CurrentPhotoIndex < 0) {
            CurrentState.CurrentPhotoIndex = CurrentState.ScenePhotos.Count - 1;
        }

        UpdateSkybox();
        UpdatePhotoCounter();
    }

    private static Vector2 ReadLeftThumbstick() {
        var device = InputSystem.devices
            .FirstOrDefault(d => d.usages.Any(u => u.Equals("LeftHand")));

        if (device == null) return Vector2.zero;

        var stickControl = device.TryGetChildControl<UnityEngine.InputSystem.Controls.Vector2Control>("thumbstick");
        return stickControl?.ReadValue() ?? Vector2.zero;
    }

    private void UpdateSkybox() {
        if (CurrentState.ScenePhotos.Count == 0) return;

        var currentTexture = CurrentState.ScenePhotos[CurrentState.CurrentPhotoIndex];

        RenderSettings.skybox = new Material(Shader.Find("Skybox/Panoramic")) {
            mainTexture = currentTexture
        };

        DynamicGI.UpdateEnvironment();
    }

    private void UpdatePhotoCounter() {
        if (_photoCounterText == null) return;

        if (CurrentState.TotalPhotos <= 1) {
            _photoCounterText.text = "Панорама 1 из 1";
            return;
        }

        _photoCounterText.text = $"Панорама {CurrentState.CurrentPhotoIndex + 1} из {CurrentState.TotalPhotos}";
    }
}
