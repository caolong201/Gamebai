using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemQuydinhController : MonoBehaviour
{
    public Button btnToggleContent;           // Nút bấm để hiện/ẩn nội dung
    public TextMeshProUGUI contentText;       // Nội dung text sẽ được show/ẩn

    private bool isContentVisible = false;

    private void Start()
    {
        // Ban đầu ẩn nội dung
        contentText.gameObject.SetActive(false);

        // Gán listener cho nút
        btnToggleContent.onClick.AddListener(ToggleContentVisibility);
    }

    private void ToggleContentVisibility()
    {
        isContentVisible = !isContentVisible;
        contentText.gameObject.SetActive(isContentVisible);
    }
}
