using System;

public static class LanguageChanger
{
    public static event Action<string> OnChangeLanguage;

    public static string Language { get; private set; }

    public static void SetLanguage(string language)
    {
        Language = language;
        OnChangeLanguage?.Invoke(language);
    }
}
