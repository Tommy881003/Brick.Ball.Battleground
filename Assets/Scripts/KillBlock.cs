using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KillBlock : MonoBehaviour
{
    public Color2 toColor2;
    public Color toColor;
    private Vector2 oriScale;

    [SerializeField]
    private LineRenderer lr;
    [SerializeField]
    private BoxCollider2D box;
    [SerializeField]
    private SpriteRenderer sr;
    
    public int beatToKill;
    private double currentBeat;
    private double timePerBeat;

    private float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        oriScale = transform.localScale;
        box.enabled = false;
        currentBeat = CameraController.beatProgress;
        timePerBeat = CameraController.timePerBeat;
        waitTime = (float)((beatToKill - currentBeat) * timePerBeat);
        StartCoroutine(Kill());
    }

    IEnumerator Kill()
    {
        sr.DOFade(0.5f, waitTime);
        sr.DOBlendableColor(Color.white, 0.05f).SetLoops(-1, LoopType.Yoyo);
        DOTween.To(() => lr.startWidth, x => lr.startWidth = x, 0.2f, waitTime);
        DOTween.To(() => lr.endWidth, x => lr.endWidth = x, 0.2f, waitTime);
        yield return new WaitForSeconds(waitTime);
        sr.DOKill();
        box.enabled = true;
        sr.color = Color.white;
        transform.localScale = oriScale + new Vector2(0.25f, 0.25f);
        sr.DOColor(toColor, 0.25f);
        transform.DOShakePosition(0.25f, 0.4f, 20, 90);
        transform.DOScale(oriScale, 0.25f);
        yield return new WaitForSeconds(0.25f);
        box.enabled = false;
        DOTween.To(() => lr.startWidth, x => lr.startWidth = x, 0, 0.5f);
        DOTween.To(() => lr.endWidth, x => lr.endWidth = x, 0, 0.5f);
        sr.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Brick Player"))
            collision.gameObject.GetComponent<BrickPlayer>().Suicide();
    }
}
