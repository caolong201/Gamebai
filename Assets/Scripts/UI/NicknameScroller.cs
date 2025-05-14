using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NicknameScroller : MonoBehaviour
{
    public RectTransform textTransform; // RectTransform của Text
    public RectTransform maskTransform; // RectTransform của khung hiển thị
    public float scrollDuration = 3f;
    public float pauseDuration = 1f;

    private Tween scrollTween;

    public void Init()
    {
        scrollTween?.Kill();
        DOVirtual.DelayedCall(0.5f, () =>
        {
            float textWidth = textTransform.rect.width;
            float maskWidth = maskTransform.rect.width;
            if (textWidth <= maskWidth) return; // Không cần scroll nếu text ngắn

            float moveDistance = textWidth - maskWidth;

            // Tạo tween chạy sang trái rồi quay lại
            scrollTween = DOTween.Sequence()
                .Append(textTransform.DOAnchorPosX(moveDistance / 2f, scrollDuration).SetEase(Ease.Linear))
                .AppendInterval(pauseDuration)
                .Append(textTransform.DOAnchorPosX(-moveDistance / 2f, scrollDuration).SetEase(Ease.Linear))
                .AppendInterval(pauseDuration)
                .SetLoops(-1,LoopType.Yoyo); // Lặp vô hạn

            // textTransform.DOAnchorPosX(moveDistance / 2f, scrollDuration).SetDelay(1).SetEase(Ease.Linear)
            //     .SetLoops(-1, LoopType.Yoyo);
        });
    }
}