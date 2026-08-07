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
    }

    public void Localize(string locale = "ru")
    {
        if (localizations == null) throw new NullReferenceException($"Не установлена локализация для {name}");
        label.text = localizations.Find(l => l.locale == locale).value;   
    }

    [Serializable]
    public struct LocString
    {
        public string locale;
        public string value;
    }
}
