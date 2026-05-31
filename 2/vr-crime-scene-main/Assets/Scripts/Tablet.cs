using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RecordingService))]
public class Tablet : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _flashScreen;
    [SerializeField] private GameObject _microphoneActiveIcon;
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private TMP_Text _status;
    [SerializeField] private float _flashScreenDelay = 0.3f;
    [SerializeField] private int _maxPhotoAudioLengthSec = 60;
    [SerializeField] private int _maxCommentAudioLengthSec = 600;
    [SerializeField] private InputActionProperty _makeComment;
    [SerializeField] private InputActionProperty _makePhoto;
    [SerializeField] private InputActionProperty _zoom;
    [SerializeField] private InputActionProperty _returnToMainMenu;

    private const int RESCALE_FACTOR = 32767;
    private const int HEADER_SIZE = 44;

    private RecordingService _recordingService;
    private AudioClip _audioComment;
    private (ushort Index, ActionStatusType ActionType) _action;
    private byte[] _screenshotBytes;
    private float _defaultFov;
    private int _audioClipLength;
    private float _startRecordingTime;

    private void OnEnable() {
        _makeComment.action.Enable();
        _makePhoto.action.Enable();
        _zoom.action.Enable();
        _returnToMainMenu.action.Enable();
    }

    private void Start() {
        _defaultFov = _camera.fieldOfView;
        _recordingService = GetComponent<RecordingService>();
        _flashScreen.SetActive(false);

        EnsurePhotoSwitcher();

        _makeComment.action.started += MakeCommentAction;
        _makePhoto.action.started += MakePhotoAction;

        _returnToMainMenu.action.started += _ => {
            CurrentState.SceneId = -1;
            CurrentState.SceneName = null;
            CurrentState.SceneTexture = null;
            CurrentState.ScenePhotos.Clear();
            CurrentState.CurrentPhotoIndex = 0;
            CurrentState.TotalPhotos = 0;
            CurrentState.FirstName = null;
            CurrentState.LastName = null;
            CurrentState.GroupName = null;

            SceneManager.LoadSceneAsync("StartScene");
        };
    }

    private void EnsurePhotoSwitcher() {
        if (GetComponent<PhotoSwitcher>() == null) {
            gameObject.AddComponent<PhotoSwitcher>();
        }
    }

    private void Update() {
        var value = _zoom.action.ReadValue<Vector2>();
        if (value == Vector2.zero) return;

        value.Normalize();
        var target = Mathf.Clamp(_camera.fieldOfView + value.y * -1.3f, 20, 120);

        var angle = Mathf.Abs((_defaultFov / 2) - _defaultFov);
        _camera.fieldOfView = Mathf.MoveTowards(_camera.fieldOfView, target, angle * Time.deltaTime);
    }

    private IEnumerator FlashScreen() {
        _flashScreen.SetActive(true);
        yield return new WaitForSeconds(_flashScreenDelay);
        _flashScreen.SetActive(false);
    }

    private void FinishRecording(Func<IEnumerator> sendCoroutine) {
        _timer.text = "00::00";
        (_audioComment, _audioClipLength) = _recordingService.StopRecord();
        StartCoroutine(sendCoroutine.Invoke());

        _action.ActionType = ActionStatusType.None;
    }

    private IEnumerator ForceFinishRecording(Func<IEnumerator> sendCoroutine, int _maxLengthSec, ushort index) {
        yield return new WaitForSeconds(_maxLengthSec);

        if (_action.Index == index) FinishRecording(sendCoroutine);
    }

    private void MakePhotoAction(InputAction.CallbackContext context) {
        if (_action.ActionType == ActionStatusType.Comment) return;
        if (_action.ActionType == ActionStatusType.Photo) {
            FinishRecording(SendPhoto);
            return;
        }

        _action.Index++;
        _action.ActionType = ActionStatusType.Photo;
        _startRecordingTime = Time.realtimeSinceStartup;

        StartCoroutine(FlashScreen());
        StartCoroutine(BlinkingIcon());
        StartCoroutine(TimerUpdate());
        StartCoroutine(ForceFinishRecording(SendPhoto, _maxPhotoAudioLengthSec, _action.Index));

        _screenshotBytes = GetImageBytes();
        _recordingService.StartRecord(_maxPhotoAudioLengthSec);
    }

    private void MakeCommentAction(InputAction.CallbackContext context) {
        if (_action.ActionType == ActionStatusType.Photo) return;
        if (_action.ActionType == ActionStatusType.Comment) {
            FinishRecording(SendComment);
            return;
        }

        _action.Index++;
        _action.ActionType = ActionStatusType.Comment;
        _startRecordingTime = Time.realtimeSinceStartup;

        StartCoroutine(BlinkingIcon());
        StartCoroutine(TimerUpdate());
        StartCoroutine(ForceFinishRecording(SendComment, _maxCommentAudioLengthSec, _action.Index));

        _recordingService.StartRecord(_maxCommentAudioLengthSec);
    }
	
	private IEnumerator SendComment() {
        var currentDate = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
        var audioFilename = string.Format("{0}_user_{1}_scene_{2}_{3}.wav", "audiocomment", CurrentState.LastName, CurrentState.SceneName, currentDate);

        var form = new WWWForm();
        form.AddBinaryData("AudioFile", ConvertToWav(), audioFilename);

        using var webRequest = UnityWebRequest.Post($"http://{CurrentState.IPAddress}:5197/api/sessions/{CurrentState.SessionId}/comments", form);
        webRequest.SetRequestHeader("MacAddress", CurrentState.MacAddress);

        yield return webRequest.SendWebRequest();

        StartCoroutine(ShowStatus("Комментарий отправленн"));
    }

    private IEnumerator SendPhoto() {
        var currentDate = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
        var screenshotFilename = string.Format("{0}_user_{1}_scene_{2}_{3}.png", "screenshot", CurrentState.LastName, CurrentState.SceneName, currentDate);
        var audioFilename = string.Format("{0}_user_{1}_scene_{2}_{3}.wav", "audio", CurrentState.LastName, CurrentState.SceneName, currentDate);

        var form = new WWWForm();
        form.AddBinaryData("AudioFile", ConvertToWav(), audioFilename);
        form.AddBinaryData("ScreenshotFile", _screenshotBytes, screenshotFilename);

        using var webRequest = UnityWebRequest.Post($"http://{CurrentState.IPAddress}:5197/api/sessions/{CurrentState.SessionId}/photos", form);
        webRequest.SetRequestHeader("MacAddress", CurrentState.MacAddress);

        yield return webRequest.SendWebRequest();

        StartCoroutine(ShowStatus("Фотография отправленна"));
    }

    private IEnumerator BlinkingIcon() {
        while (_action.ActionType != ActionStatusType.None) {
            _microphoneActiveIcon.SetActive(!_microphoneActiveIcon.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }

        _microphoneActiveIcon.SetActive(false);
        yield return null;
    }

    private IEnumerator ShowStatus(string text) {
        _status.text = text;
        yield return new WaitForSeconds(1f);
        _status.text = string.Empty;
    }

    private IEnumerator TimerUpdate() {
        while (_action.ActionType != ActionStatusType.None) {
            var passedSeconds = Time.realtimeSinceStartup - _startRecordingTime;
            _timer.text = $"{(int)passedSeconds / 60:00}:{passedSeconds % 60:00}";
            yield return new WaitForSeconds(1);
        }
    }

    private byte[] GetImageBytes() {
        var image = new Texture2D(_camera.targetTexture.width, _camera.targetTexture.height, TextureFormat.RGB24, false);

        _camera.Render();
        RenderTexture.active = _camera.targetTexture;

        image.ReadPixels(new(0, 0, _camera.targetTexture.width, _camera.targetTexture.height), 0, 0);
        return image.EncodeToPNG();
    }

    private byte[] ConvertToWav() {
        var length = _audioComment.frequency * _audioComment.channels * _audioClipLength;
        var samples = new float[length];
        _audioComment.GetData(samples, 0);

        var bytes = new byte[(length * 2) + HEADER_SIZE];

        for (int i = 0; i < length; i++) {
            BitConverter.GetBytes((short)(samples[i] * RESCALE_FACTOR)).CopyTo(bytes, (i * 2) + HEADER_SIZE);
        }

        byte[] riff = new byte[] { 0x52, 0x49, 0x46, 0x46 };
        riff.CopyTo(bytes, 0);

        byte[] chunkSize = BitConverter.GetBytes(bytes.Length - 8);
        chunkSize.CopyTo(bytes, 4);

        byte[] wave = new byte[] { 0x57, 0x41, 0x56, 0x45 };
        wave.CopyTo(bytes, 8);

        byte[] fmt = new byte[] { 0x66, 0x6d, 0x74, 0x20 };
        fmt.CopyTo(bytes, 12);

        byte[] subChunk1 = BitConverter.GetBytes(16);
        subChunk1.CopyTo(bytes, 16);

        byte[] audioFormat = BitConverter.GetBytes(1);
        audioFormat.CopyTo(bytes, 20);

        byte[] numChannels = BitConverter.GetBytes(_audioComment.channels);
        numChannels.CopyTo(bytes, 22);

        byte[] sampleRate = BitConverter.GetBytes(_audioComment.frequency);
        sampleRate.CopyTo(bytes, 24);

        byte[] byteRate = BitConverter.GetBytes(_audioComment.frequency * _audioComment.channels * 2);
        byteRate.CopyTo(bytes, 28);

        BitConverter.GetBytes((ushort)(_audioComment.channels * 2)).CopyTo(bytes, 32);

        byte[] bitsPerSample = BitConverter.GetBytes(16);
        bitsPerSample.CopyTo(bytes, 34);

        byte[] datastring = new byte[] { 0x64, 0x61, 0x74, 0x61 };
        datastring.CopyTo(bytes, 36);

        byte[] subChunk2 = BitConverter.GetBytes(length * 2);
        subChunk2.CopyTo(bytes, 40);

        return bytes;
    }
}
