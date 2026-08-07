using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Saved : MonoBehaviour
{
    private RectTransform rect;

    Sequence seq;
    float startx;

    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();
        startx = rect.anchoredPosition.x;
        SettingsApplier.OnSettingsSaved += SettingsApplier_OnSettingsSaved;
    }

    private void OnDestroy()
    {
        SettingsApplier.OnSettingsSaved -= SettingsApplier_OnSettingsSaved;
        if (seq != null)
            seq.Kill();
    }

    private void SettingsApplier_OnSettingsSaved()
    {
        if (seq != null)
        {
            seq.Kill();
            seq = null;
        }

        seq = DOTween.Sequence();

        seq.Append(rect.DOAnchorPosX(0, 0.3f));
        seq.AppendInterval(1);
        seq.Append(rect.DOAnchorPosX(startx, 0.5f).SetEase(Ease.InBack));

        seq.PlayForward();
    }
}
