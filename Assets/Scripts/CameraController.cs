using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class CameraController : MonoBehaviour
{
    public GameObject killBlock;
    public GameObject killHorizontal;
    public GameObject killVertical;
    private Vector2 previousKill = new Vector2(100, 100);
    private Vector2 kill;

    private Camera cam;
    private AudioSource music;
    public Ease shakeEase;

    [SerializeField]
    private double timer = 0;
    public double bpm;
    private double startTime;

    public static double timePerBeat { get; private set; }
    public static double beatProgress { get; private set; }
    public static int beatCount { get; private set; }
    
    [HideInInspector]
    public UnityEvent Phase2 = new UnityEvent();
    [HideInInspector]
    public UnityEvent StartEnd = new UnityEvent();
    [HideInInspector]
    public UnityEvent End = new UnityEvent();
    private int endCount = 240;
    private int Phase3Count = 46;
    private int Phase4Count = 78;

    private void Awake()
    {
        timePerBeat = 60F / bpm;
        beatCount = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        kill = new Vector2(Random.Range(-12, 13), Random.Range(-4, 8));
        music = GetComponent<AudioSource>();
        beatProgress = 0F;
        cam = GetComponent<Camera>();
        SceneAudioManager.instance.OnGo.AddListener(StartMusic);
        enabled = false;
    }

    private void Update()
    {
        if(beatProgress >= endCount)
        {
            StartEnd.Invoke();
            StartCoroutine(end());
            enabled = false;
        }
        if (Time.timeScale == 0)
            enabled = false;
        if (Phase3Count + 2 <= endCount && beatProgress >= Phase3Count)
        {
            while(Mathf.Abs(kill.x - previousKill.x) < 2 || Mathf.Abs(kill.y - previousKill.y) < 2)
                kill = new Vector2(Random.Range(-12, 13), Random.Range(-4, 8));
            previousKill = kill;
            GameObject go = Instantiate(killBlock, kill, Quaternion.identity);
            go.GetComponent<KillBlock>().beatToKill = Phase3Count + 2;
            go.transform.SetParent(Fall.instance.transform);
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, Mathf.FloorToInt(go.transform.localPosition.y));
            Phase3Count++;
        }

        if (Phase4Count + 2 <= endCount && beatProgress >= Phase4Count)
        {
            float rand = Random.Range(0f, 1f);
            GameObject go = null;
            if(rand > 0.5f)
            {
                go = Instantiate(killHorizontal, new Vector2(0, Random.Range(-4, 7)), Quaternion.identity);
                go.transform.SetParent(Fall.instance.transform);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, Mathf.FloorToInt(go.transform.localPosition.y) + 0.5f);
            }
            else
                go = Instantiate(killVertical, new Vector2(Random.Range(-14, 14) + 0.5f,1), Quaternion.identity);
            go.GetComponent<KillBlock>().beatToKill = Phase4Count + 2;
            Phase4Count += 2;
        }

        if (beatCount + 2 <= endCount && beatProgress >= beatCount)
        {
            cam.transform.position = new Vector3(0.3f, 0, -10);
            cam.transform.DOMoveX(0, 0.25f).SetEase(shakeEase);
            if (beatCount == 16)
                Phase2.Invoke();
            beatCount += 2;
        }
    }

    // Update is called once per frame
    void OnAudioFilterRead(float[] data, int channels)
    {
        beatProgress = (AudioSettings.dspTime - startTime) / timePerBeat;
        timer = AudioSettings.dspTime - startTime;
    }

    void StartMusic()
    {
        enabled = true;
        music.Play();
        startTime = AudioSettings.dspTime;
    }

    IEnumerator end()
    {
        yield return new WaitForSeconds(1f);
        End.Invoke();
    }
}
