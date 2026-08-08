using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class StatusPanel : MonoBehaviour
{
    [SerializeField] Transform root;
    [SerializeField] List<GameObject> heartModels;
    [SerializeField] float heartXOffset = 1.5f;
    [SerializeField] GameObject heartPrefab;
    [SerializeField] float heartLocalY = 1.179f;

    public Sequence ShowStatusSeq()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(root.DOScale(1, 1).SetEase(Ease.OutBack));
        return seq;
    }

    public Sequence HideStatusSeq()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(root.DOScale(0, 1).SetEase(Ease.InBack));
        return seq;
    }

    public Sequence SetHearts(int amount)
    {
        Sequence seq = DOTween.Sequence();

        // Если количество совпадает - возвращаем пустую секвенцию
        if (amount == heartModels.Count) return seq;

        // Если нужно УМЕНЬШИТЬ количество
        if (amount < heartModels.Count)
        {
            int destroyCount = heartModels.Count - amount;
            List<GameObject> heartsToDestroy = new List<GameObject>();

            // Удаляем с ПРАВОГО конца (последние destroyCount штук)
            for (int i = heartModels.Count - destroyCount; i < heartModels.Count; i++)
            {
                heartsToDestroy.Add(heartModels[i]);
            }

            // Анимируем удаление в обратном порядке (справа налево)
            for (int i = heartsToDestroy.Count - 1; i >= 0; i--)
            {
                GameObject heart = heartsToDestroy[i];
                seq.Append(DestroyHeart(heart.transform, 0.5f));
                seq.AppendCallback(() =>
                {
                    if (heartModels.Contains(heart))
                    {
                        heartModels.Remove(heart);
                        Destroy(heart);
                    }
                });
            }

            // После удаления - перецентровка оставшихся
            seq.Append(MoveAllHeartsToPositions(amount, 0.5f));
        }
        // Если нужно УВЕЛИЧИТЬ количество
        else if (amount > heartModels.Count)
        {
            int addCount = amount - heartModels.Count;
            int currentCount = heartModels.Count;

            // Сначала перецентровка существующих сердец
            seq.Append(MoveAllHeartsToPositions(amount, 0.5f));

            // Затем спавн новых сердец справа
            for (int i = 0; i < addCount; i++)
            {
                int newIndex = currentCount + i;
                float x = GetHeartPosition(newIndex, amount);

                // Создаём новое сердце
                GameObject newHeart = Instantiate(heartPrefab, root);
                newHeart.transform.localPosition = new Vector3(x, heartLocalY, 0);
                newHeart.transform.localScale = Vector3.zero;

                heartModels.Add(newHeart);

                // Анимация появления
                seq.Append(newHeart.transform.DOScale(62, 0.6f).SetEase(Ease.OutBack));
            }
        }

        return seq;
    }

    // Получить позицию сердца по индексу для общего количества
    private float GetHeartPosition(int index, int totalAmount)
    {
        if (totalAmount <= 1) return 0;

        float totalWidth = (totalAmount - 1) * heartXOffset;
        float firstHeartX = -totalWidth / 2f;
        return firstHeartX + index * heartXOffset;
    }

    // Движение всех сердец к новым центрированным позициям
    private Tween MoveAllHeartsToPositions(int totalAmount, float duration)
    {
        Sequence moveSeq = DOTween.Sequence();

        for (int i = 0; i < heartModels.Count; i++)
        {
            float x = GetHeartPosition(i, totalAmount);
            moveSeq.Join(MoveHeart(heartModels[i].transform, x, duration));
        }

        return moveSeq;
    }

    Tween MoveHeart(Transform heart, float x, float duration)
    {
        return heart.DOLocalMoveX(x, duration).SetEase(Ease.OutCubic);
    }

    Tween DestroyHeart(Transform heart, float duration)
    {
        return heart.DOScale(0, duration).SetEase(Ease.InBack);
    }
}