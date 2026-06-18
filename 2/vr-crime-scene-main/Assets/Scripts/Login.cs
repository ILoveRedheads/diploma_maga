using System.Collections;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
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
    [SerializeField] private int _maxScenePhotos = 20;

    private void Start() {
        CurrentState.MacAddress = GetDeviceMacAddress();
        _macAddressText.text = CurrentState.MacAddress;
    }

    public void OnEnterClick() => StartCoroutine(SendRequest());

    private IEnumerator SendRequest() {
        _popup.SetActive(true);
        _popupText.text = "Загрузка";

        if (string.IsNullOrEmpty(_firstName.text)
            || string.IsNullOrEmpty(_lastName.text)
            || string.IsNullOrEmpty(_groupName.text)) {
            StartCoroutine(Wait("Ошибка\nДолжны быть заполнены все поля"));
            yield break;
        }

        CurrentState.FirstName = _firstName.text;
        CurrentState.LastName = _lastName.text;
        CurrentState.GroupName = _groupName.text;
        CurrentState.ScenePhotos.Clear();
        CurrentState.CurrentPhotoIndex = 0;
        CurrentState.TotalPhotos = 0;

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
                StartCoroutine(Wait("Ошибка\nНе найден сервер!"));
                yield break;
            }
        }

        _popupText.text = "Загрузка сцены";

        using (var metaRequest = UnityWebRequest.Get($"http://{CurrentState.IPAddress}:5197/api/scenes/meta")) {
            metaRequest.SetRequestHeader("MacAddress", CurrentState.MacAddress);

            yield return metaRequest.SendWebRequest();

            if (metaRequest.result != UnityWebRequest.Result.Success) {
                StartCoroutine(Wait($"Ошибка\n{metaRequest.responseCode} : {metaRequest.error}"));
                yield break;
            }

            var metaJson = metaRequest.downloadHandler.text;
            CurrentState.SceneId = ParseJsonLong(metaJson, "sceneId");
            CurrentState.SceneName = ParseJsonString(metaJson, "sceneName");
            CurrentState.TotalPhotos = (int)ParseJsonLong(metaJson, "photoCount");
        }

        if (CurrentState.TotalPhotos <= 0) {
            CurrentState.TotalPhotos = 1;
        }

        var photosToLoad = Mathf.Min(CurrentState.TotalPhotos, _maxScenePhotos);

        for (var photoIndex = 0; photoIndex < photosToLoad; photoIndex++) {
            using var webRequest = UnityWebRequestTexture.GetTexture(
                $"http://{CurrentState.IPAddress}:5197/api/scenes/photos/by-index/{photoIndex}");
            webRequest.SetRequestHeader("MacAddress", CurrentState.MacAddress);

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success) {
                if (photoIndex == 0) {
                    StartCoroutine(Wait($"Ошибка\n{webRequest.responseCode} : {webRequest.error}"));
                    yield break;
                }

                break;
            }

            var texture = DownloadHandlerTexture.GetContent(webRequest);
            CurrentState.ScenePhotos.Add(texture);

            if (photoIndex == 0) {
                CurrentState.SceneTexture = texture;
                CurrentState.CurrentPhotoIndex = 0;
            }
        }

        CurrentState.TotalPhotos = CurrentState.ScenePhotos.Count;

        if (CurrentState.ScenePhotos.Count == 0) {
            StartCoroutine(Wait("Ошибка\nНе удалось загрузить панорамы сцены"));
            yield break;
        }

        _popupText.text = "Начинаем сеанс";

        var body = System.Text.Encoding.UTF8.GetBytes(
            $"{{\"firstName\": \"{CurrentState.FirstName}\", \"lastName\": \"{CurrentState.LastName}\", \"groupName\": \"{CurrentState.GroupName}\"}}");

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
                StartCoroutine(Wait("Ошибка\nНеверный формат ответа со стороны сервера!"));
                yield break;
            }

            CurrentState.SessionId = sessionId;
        }

        SceneManager.LoadSceneAsync("CrimePhoto");
    }

    private static string GetDeviceMacAddress() {
        try {
            var wifiNic = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(nic => nic.OperationalStatus == OperationalStatus.Up
                    && nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);

            if (wifiNic != null) {
                var address = NormalizeMac(wifiNic.GetPhysicalAddress().ToString());
                if (IsUsableMac(address)) {
                    return address;
                }
            }

            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up
                    && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)) {
                var physicalAddress = NormalizeMac(networkInterface.GetPhysicalAddress().ToString());
                if (IsUsableMac(physicalAddress)) {
                    return physicalAddress;
                }
            }
        } catch (System.Exception ex) {
            Debug.LogWarning($"Не удалось получить MAC-адрес: {ex.Message}");
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        var androidFallback = TryGetAndroidMacStyleId();
        if (!string.IsNullOrEmpty(androidFallback)) {
            Debug.LogWarning(
                "Используется идентификатор устройства в формате MAC (на Quest реальный Wi‑Fi MAC часто недоступен). " +
                "Скопируйте строку с экрана VR в поле «MAC адрес» в веб-клиенте.");
            return androidFallback;
        }
#endif

        return "UNKNOWN_MAC";
    }

    private static string TryGetAndroidMacStyleId() {
        try {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var resolver = activity.Call<AndroidJavaObject>("getContentResolver");
            var secure = new AndroidJavaClass("android.provider.Settings$Secure");
            var androidId = secure.CallStatic<string>("getString", resolver, "android_id");
            return ToMacStyleIdentifier(androidId);
        } catch (System.Exception ex) {
            Debug.LogWarning($"Не удалось получить Android ID: {ex.Message}");
            return string.Empty;
        }
    }

    private static string NormalizeMac(string raw) =>
        string.IsNullOrEmpty(raw) ? string.Empty : raw.Replace(":", "").Replace("-", "").ToUpperInvariant();

    private static bool IsUsableMac(string mac) =>
        !string.IsNullOrEmpty(mac)
        && mac != "00000000000000"
        && mac != "020000000000";

    /// <summary>
    /// 12 hex-символов для поля mac_address на сервере (как у Pico), если реальный MAC недоступен.
    /// </summary>
    private static string ToMacStyleIdentifier(string source) {
        if (string.IsNullOrEmpty(source)) {
            return string.Empty;
        }

        var hex = new string(source.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        if (hex.Length < 12) {
            hex = hex.PadRight(12, '0');
        }

        return hex.Length > 12 ? hex[..12] : hex;
    }

    private static long ParseJsonLong(string json, string key) {
        var match = Regex.Match(json, $"\"{key}\"\\s*:\\s*(\\d+)", RegexOptions.IgnoreCase);
        return match.Success && long.TryParse(match.Groups[1].Value, out var value) ? value : 0;
    }

    private static string ParseJsonString(string json, string key) {
        var match = Regex.Match(json, $"\"{key}\"\\s*:\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private IEnumerator Wait(string text) {
        _popupText.text = text;
        yield return new WaitForSeconds(2);
        _popupText.text = string.Empty;
        _popup.SetActive(false);
    }
}
