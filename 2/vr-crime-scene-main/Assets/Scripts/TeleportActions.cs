using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(TeleportationAnchor))]
public class TeleportActions : MonoBehaviour
{
    [SerializeField] private GameObject _objectOnSelect;
    [SerializeField] private GameObject _defaultObject;
    [SerializeField] private Material _materialForSkybox;

    private TeleportationAnchor _teleportationAnchor;

    private void Start() {
        _defaultObject.SetActive(true);
        _objectOnSelect.SetActive(false);

        _teleportationAnchor = GetComponent<TeleportationAnchor>();
        _teleportationAnchor.hoverEntered.AddListener(_ => {
            _defaultObject.SetActive(false);
            _objectOnSelect.SetActive(true);
        });

        _teleportationAnchor.hoverExited.AddListener(_ => {
            _objectOnSelect.SetActive(false);
            _defaultObject.SetActive(true);
        });

        _teleportationAnchor.teleporting.AddListener(_ => {
            _objectOnSelect.SetActive(false);
            _defaultObject.SetActive(false);

            RenderSettings.skybox = _materialForSkybox;
        });
    }
}
