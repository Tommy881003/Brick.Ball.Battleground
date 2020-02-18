using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BuffText : MonoBehaviour
{
    private TextMeshProUGUI buffText;
    private float y;

    void Start()
    {
        y = transform.position.y;
        buffText = GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        buffText.DOFade(0, 0.5f).SetEase(Ease.InQuint);
        transform.DOMoveY(y + 0.5f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
