using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Localizator : MonoBehaviour
{
    [SerializeField] List<Localization> _localizations;
    private TextMeshProUGUI _label;

    private void OnEnable()
    {
        _label = GetComponent<TextMeshProUGUI>();
        Localize(YG2.lang);

        YG2.onSwitchLang += Localize;
    }

    private void OnDisable() => YG2.onSwitchLang -= Localize;

    private void OnDestroy() => YG2.onSwitchLang -= Localize;

    public void Localize(string locale = "ru")
    {
        if (_localizations == null) throw new NullReferenceException($"Не установлена локализация для {name}");

        string localizedText = _localizations.Find(l => l.locale == locale).value;

        if(string.IsNullOrEmpty(localizedText))
        {
            _label.text = _localizations[0].value;
            return;
        }

        _label.text = localizedText;
    }

    [Serializable]
    public struct Localization
    {
        public string locale;
        public string value;
    }
}