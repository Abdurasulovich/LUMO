namespace Lummo.Mobile.Services.Enums;

/// <summary>
/// User qaysi provider orqali ro'yxatdan o'tganini belgilaydi
/// </summary>
public enum AuthProvider
{
    /// <summary>
    /// Email va parol orqali (manual registration)
    /// </summary>
    Email = 0,

    /// <summary>
    /// Google OAuth orqali
    /// </summary>
    Google = 1,

    /// <summary>
    /// Apple Sign In orqali
    /// </summary>
    Apple = 2,

    /// <summary>
    /// Facebook orqali
    /// </summary>
    Facebook = 3
}

