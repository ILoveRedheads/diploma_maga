using UnityEngine;

public static class CurrentState
{
    public static long SceneId { get; set; }
    public static string SceneName { get; set; }
    public static string MacAddress { get; set; }
    public static string FirstName { get; set; }
    public static string LastName { get; set; }
    public static string GroupName { get; set; }
    public static Texture2D SceneTexture { get; set; }
    public static long SessionId { get; set; }
    public static string IPAddress { get; set; } = "192.168.3.2";
}
