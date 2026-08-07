using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Localizator : MonoBehaviour
{
    [SerializeField] List<LocString> localizations;
    private TextMeshProUGUI label;


    private void OnEnable()
    {
        label = GetComponent<TextMeshProUGUI>();
        Localize(LanguageChanger.Language);

        LanguageChanger.OnChangeLanguage += Localize;
    }

    private void OnDisable()
    {
        LanguageChanger.OnChangeLanguage -= Localize;
    }

    private void OnDestroy()
    {
        LanguageChanger.OnChangeLanguage -= Localize;
    }

    public void Localize(string locale = "ru")
    {
        if (localizations == null) throw new NullReferenceException($"Не установлена локализация для {name}");

        string localizedText = localizations.Find(l => l.locale == locale).value;

        if(string.IsNullOrEmpty(localizedText))
        {
            label.text = localizations[0].value;
            return;
        }

        label.text = localizedText;
    }

    [Serializable]
    public struct LocString
    {
        public string locale;
        public string value;
    }
}
