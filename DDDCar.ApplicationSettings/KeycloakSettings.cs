namespace DDDCar.ApplicationSettings;

/// <summary> Настройки для авторизации и аутенификации по keycloak </summary>
public class KeycloakSettings
{
    /// <summary> Базовый адрес вида http://localhost:8080/ </summary>
    public string BaseUrl { get; set; } = null!;
    
    /// <summary> Realm в keycloak </summary>
    public string Realm { get; set; } = null!;
    
    /// <summary> Аудиенция вида my-service-name </summary>
    public string Audience { get; set; } = null!;
    
    /// <summary> Authority keycloak </summary>
    public string Authority { get; set; } = null!;
}
