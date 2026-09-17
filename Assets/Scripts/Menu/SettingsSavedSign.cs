using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SettingsSavedSign : MonoBehaviour
{
    private RectTransform _rect;
    private Sequence _seq;
    private float _startX;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _startX = _rect.anchoredPosition.x;
    }

    private void OnEnable()
    {
        SettingsManager.OnSettingsSaved += SettingsApplier_OnSettingsSaved;
    }

    private void OnDisable()
    {
        SettingsManager.OnSettingsSaved -= SettingsApplier_OnSettingsSaved;
        _seq?.Kill();
        _seq = null;
    }

    private void SettingsApplier_OnSettingsSaved()
    {
        if (_seq != null)
        {
            _seq.Kill();
            _seq = null;
        }

        _seq = GetAnimationSequence();
        _seq.PlayForward();
    }

    private Sequence GetAnimationSequence()
    {
        var seq = DOTween.Sequence();

        seq.Append(_rect.DOAnchorPosX(0, 0.3f));
        seq.AppendInterval(1);
        seq.Append(_rect.DOAnchorPosX(_startX, 0.5f).SetEase(Ease.InBack));

        return seq;
    }
}