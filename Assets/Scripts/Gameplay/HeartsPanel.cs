using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HeartsPanel : MonoBehaviour
{
    [SerializeField] private Transform _root;
    [SerializeField] private List<GameObject> _heartModels;
    [SerializeField] private GameObject _heartPrefab;

    [SerializeField] private float _heartXOffset = 1.5f;
    [SerializeField] private float _heartLocalY = 1.179f;

    public Sequence SetHearts(int amount)
    {
        Sequence seq = DOTween.Sequence();
        if (amount == _heartModels.Count) return seq;

        if (amount < _heartModels.Count)
        {
            int destroyCount = _heartModels.Count - amount;
            List<GameObject> heartsToDestroy = new List<GameObject>();
                        
            for (int i = _heartModels.Count - destroyCount; i < _heartModels.Count; i++)
                heartsToDestroy.Add(_heartModels[i]);

            for (int i = heartsToDestroy.Count - 1; i >= 0; i--)
            {
                GameObject heart = heartsToDestroy[i];
                seq.Append(DestroyHeart(heart.transform, 0.5f));
                seq.AppendCallback(() =>
                {
                    if (_heartModels.Contains(heart))
                    {
                        _heartModels.Remove(heart);
                        Destroy(heart);
                    }
                });
            }

            seq.Append(MoveAllHeartsToPositions(amount, 0.5f));
        }
        else if (amount > _heartModels.Count)
        {
            int addCount = amount - _heartModels.Count;
            int currentCount = _heartModels.Count;

            seq.Append(MoveAllHeartsToPositions(amount, 0.5f));

            for (int i = 0; i < addCount; i++)
            {
                int newIndex = currentCount + i;
                float x = GetHeartPosition(newIndex, amount);

                GameObject newHeart = Instantiate(_heartPrefab, _root);
                newHeart.transform.localPosition = new Vector3(x, _heartLocalY, 0);
                newHeart.transform.localScale = Vector3.zero;

                _heartModels.Add(newHeart);

                seq.Append(newHeart.transform.DOScale(62, 0.6f).SetEase(Ease.OutBack));
            }
        }

        return seq;
    }

    private float GetHeartPosition(int index, int totalAmount)
    {
        if (totalAmount <= 1) return 0;

        float totalWidth = (totalAmount - 1) * _heartXOffset;
        float firstHeartX = -totalWidth / 2f;
        return firstHeartX + index * _heartXOffset;
    }

    private Tween MoveAllHeartsToPositions(int totalAmount, float duration)
    {
        Sequence moveSeq = DOTween.Sequence();

        for (int i = 0; i < _heartModels.Count; i++)
        {
            float x = GetHeartPosition(i, totalAmount);
            moveSeq.Join(MoveHeart(_heartModels[i].transform, x, duration));
        }

        return moveSeq;
    }

    private Tween MoveHeart(Transform heart, float x, float duration) 
        => heart.DOLocalMoveX(x, duration).SetEase(Ease.OutCubic);

    private Tween DestroyHeart(Transform heart, float duration) 
        => heart.DOScale(0, duration).SetEase(Ease.InBack);
}