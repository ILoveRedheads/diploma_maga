using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class MenuUi : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _uiObject;
    [SerializeField] private Transform _head;
    [SerializeField] private InputActionProperty _showUi;
    [SerializeField] private float _spawnDistance = 2.0f;

    [Header("Ray interactor")]
    [SerializeField] private XRRayInteractor _rayInteractor;

    private bool _isActive = false;

    private void Start() {
        _showUi.action.Enable();
        _showUi.action.started += _ => {
            if (_uiObject == null) return;

            _uiObject.SetActive(!_uiObject.activeSelf);
            _uiObject.transform.position = _head.position + new Vector3(_head.forward.x, 0, _head.forward.z).normalized * _spawnDistance;

            _isActive = !_isActive;

            _rayInteractor.enabled = _isActive;
        };

        _uiObject.SetActive(false);
    }

    private void Update() {
        if (!_uiObject.activeSelf) return;

        _uiObject.transform.LookAt(new Vector3(_head.position.x, _uiObject.transform.position.y, _head.position.z));
        _uiObject.transform.forward *= -1;
    }
}
