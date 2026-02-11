using System.Linq;
using TMPro;
using UnityEngine;

public class RecordingService : MonoBehaviour
{
    [SerializeField] TMP_Dropdown _dropdown;
    [Header("Microphone settings")]
    [SerializeField] private int frequency = 16000;

    public string[] Microphones => _devices;

    public int ActiveMicrophone { 
        set {
            _deviceName = value >= 0 && value < _devices.Length ? _devices[value] : string.Empty;
        }
    }

    private bool _isRecording;
    private float _recordStart;
    private string[] _devices;
    private string _deviceName;
    private int _maxLengthSec;
    private AudioClip _audioClip;

    private void Start() {
        _dropdown.options = Microphone.devices.Select(device => new TMP_Dropdown.OptionData() { text = device }).ToList();
        _dropdown.onValueChanged.AddListener(index => {
            _deviceName = _dropdown.options[index].text;
        });

        _devices = Microphone.devices;
        Debug.Log(string.Join(" ", _devices));
        _deviceName = _devices[1];
    }

    private void Update() {
        if (!_isRecording) return;

        var timePassed = Time.realtimeSinceStartup - _recordStart;
        if (timePassed > _maxLengthSec) {
            StopRecord();
            return;
        }
    }

    public void StartRecord(int maxLengthSec) {
        _recordStart = Time.realtimeSinceStartup;
        _maxLengthSec = maxLengthSec;
        _audioClip = Microphone.Start(_deviceName, false, _maxLengthSec, frequency);
        _isRecording = true;
    }

    public (AudioClip, int seconds) StopRecord() {
        Microphone.End(_deviceName);
        _isRecording = false;

        return (_audioClip, (int)(Time.realtimeSinceStartup - _recordStart + 0.9f));
    }
}
