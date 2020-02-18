using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CountDown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI countDown;

    private void Start()
    {
        StartCoroutine(StartCountDown());
        countDown.text = "";
    }

    public IEnumerator StartCountDown()
    {
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(0.25f);

        countDown.text = "3...";
        SceneAudioManager.instance.PlayByName("3");

        yield return new WaitForSecondsRealtime(1f);

        countDown.text = "2...";
        SceneAudioManager.instance.PlayByName("2");

        yield return new WaitForSecondsRealtime(1f);

        countDown.text = "1...";
        SceneAudioManager.instance.PlayByName("1");

        yield return new WaitForSecondsRealtime(1f);

        countDown.text = "Go!";
        SceneAudioManager.instance.PlayByName("Go");

        yield return new WaitForSecondsRealtime(1f);

        countDown.text = "";
        Time.timeScale = 1;
        SceneAudioManager.instance.PlayByName("Scene_1_bgm");
        SceneAudioManager.instance.OnGo.Invoke();
    }
}
