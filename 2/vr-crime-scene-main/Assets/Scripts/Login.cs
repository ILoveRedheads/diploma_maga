using System.Collections;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField _firstName;
    [SerializeField] private TMP_InputField _lastName;
    [SerializeField] private TMP_InputField _groupName;
    [SerializeField] private TMP_Text _popupText;
    [SerializeField] private TMP_Text _macAddressText;
    [SerializeField] private GameObject _popup;

    private void Start() {
        CurrentState.MacAddress = NetworkInterface.GetAllNetworkInterfaces()
            .First(x => x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
            .GetPhysicalAddress().ToString();

        _macAddressText.text = CurrentState.MacAddress;
    }

    public void OnEnterClick() => StartCoroutine(SendRequest());

    private IEnumerator SendRequest() {
        _popup.SetActive(true);
        _popupText.text = "Загрузка";

        if (string.IsNullOrEmpty(_firstName.text)
            || string.IsNullOrEmpty(_lastName.text)
            || string.IsNullOrEmpty(_groupName.text)) {
            StartCoroutine(Wait($"Ошибка\nДолжны быть заполнены все поля"));
            yield break;
        }

        CurrentState.FirstName = _firstName.text;
        CurrentState.LastName = _lastName.text;
        CurrentState.GroupName = _groupName.text;

        var isCorrectIp = true;
        using (var webRequest = UnityWebRequest.Get($"http://{CurrentState.IPAddress}:5197/ping")) {
            yield return webRequest.SendWebRequest();

            isCorrectIp = webRequest.result == UnityWebRequest.Result.Success;
        }

        if (!isCorrectIp) {
            _popupText.text = "Поиск сервера";

            var ipAddress = Dns.GetHostAddresses(Dns.GetHostName())
                .First(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();

            var lastIndex = ipAddress.LastIndexOf('.');
            var ipBase = ipAddress[..(lastIndex + 1)];
            var lastByte = 2;

            while (!isCorrectIp && lastByte < 256) {
                CurrentState.IPAddress = ipBase + lastByte;

                using var webRequest = UnityWebRequest.Get($"http://{CurrentState.IPAddress}:5197/ping");
                yield return webRequest.SendWebRequest();

                isCorrectIp = webRequest.result == UnityWebRequest.Result.Success;
                lastByte++;
            }

            if (lastByte >= 256) {
                StartCoroutine(Wait($"Ошибка\nНе найден сервер!"));
                yield break;
            }
        }

        _popupText.text = "Загрузка";

        using (var webRequest = UnityWebRequestTexture.GetTexture($"http://{CurrentState.IPAddress}:5197/api/scenes")) {
            webRequest.SetRequestHeader("MacAddress", CurrentState.MacAddress);

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success) {
                StartCoroutine(Wait($"Ошибка\n{webRequest.responseCode} : {webRequest.error}"));
                yield break;
            }

            CurrentState.SceneTexture = DownloadHandlerTexture.GetContent(webRequest);
        }

        _popupText.text = "Начинаем сенас";

        var body = System.Text.Encoding.UTF8.GetBytes(
            $"{{\"firstName\": \"{CurrentState.FirstName}\", \"lastName\": \"{CurrentState.LastName}\", \"groupName\": \"{CurrentState.GroupName}\"}}"
        );

        using (var webRequest = new UnityWebRequest($"http://{CurrentState.IPAddress}:5197/api/sessions", UnityWebRequest.kHttpVerbPOST)) {
            webRequest.uploadHandler = new UploadHandlerRaw(body);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("MacAddress", CurrentState.MacAddress);
            webRequest.SetRequestHeader("accept", "text/plain");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success) {
                StartCoroutine(Wait($"Ошибка\n{webRequest.responseCode} : {webRequest.error}"));
                yield break;
            }

            if (!long.TryParse(webRequest.downloadHandler.text, out var sessionId)) {
                StartCoroutine(Wait($"Ошибка\nНеверный формат ответа со стороны сервера!"));
                yield break;
            }

            CurrentState.SessionId = sessionId;
        }

        SceneManager.LoadSceneAsync("CrimePhoto");
    }

    private IEnumerator Wait(string text) {
        _popupText.text = text;

        yield return new WaitForSeconds(2);

        _popupText.text = string.Empty;
        _popup.SetActive(false);
    }
}
