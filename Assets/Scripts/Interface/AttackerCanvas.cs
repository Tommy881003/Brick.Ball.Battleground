using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AttackerCanvas : MonoBehaviour
{
    public DeadLine line;
    public BrickPlayer player;
    private CanvasGroup group;
    private bool invoked = false;

    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;
        line.OnBrickEnter.AddListener(ShowCanvas);
    }

    void ShowCanvas()
    {
        if (invoked)
            return;
        player.OnDie.RemoveAllListeners();
        Camera.main.gameObject.GetComponent<CameraController>().End.RemoveAllListeners();
        invoked = true;
        SceneAudioManager.instance.PlayByName("Gameover");
        SceneAudioManager.instance.VolumeChange("Scene_1_bgm",0,0.5f);
        StartCoroutine(StartShowing());
    }

    IEnumerator StartShowing()
    {
        DOTween.To(() => group.alpha, x => group.alpha = x, 1, 0.5f).SetUpdate(true);
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, 0.5f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(0.5f);
        SceneAudioManager.instance.StopByName("Scene_1_bgm");
        SceneAudioManager.instance.VolumeChange("Scene_1_bgm", 1, 0.05f);
        group.blocksRaycasts = true;
        group.interactable = true;
    }
}
