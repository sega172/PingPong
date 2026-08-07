using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Localizator : MonoBehaviour
{
    [SerializeField] List<LocString> localizations;
    private TextMeshProUGUI label;


    private void OnEnable()
    {
        label = GetComponent<TextMeshProUGUI>();
        Localize(YG2.lang);

        YG2.onSwitchLang += Localize;
        
    }

    private void OnDisable()
    {
        YG2.onSwitchLang -= Localize;
    }

    private void OnDestroy()
    {
        YG2.onSwitchLang -= Localize;
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
