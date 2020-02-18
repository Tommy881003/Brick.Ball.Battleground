using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClipArray sceneClipArray;
    [HideInInspector]
    public List<Clip> sceneClips = new List<Clip>();
    public static SceneAudioManager instance;
    private Dictionary<Clip, Sound> clip2Sound = new Dictionary<Clip, Sound>();
    [Range(0f, 1f)]
    public float musicAmp = 1, fxAmp = 1;
    private bool sceneChangeComplete = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        if(sceneClipArray != null)
        {
            foreach (Sound s in sceneClipArray.sounds)
            {
                Clip newClip = new Clip();
                newClip.name = s.name;
                newClip.hash = s.hashCode;
                newClip.isFX = s.isFX;
                newClip.volume = s.volume;
                newClip.source = gameObject.AddComponent<AudioSource>();
                newClip.source.clip = s.clip;
                newClip.source.playOnAwake = false;
                newClip.source.volume = s.volume;
                newClip.source.pitch = s.pitch;
                newClip.source.loop = s.loop;
                newClip.source.spatialBlend = s.blend;
                newClip.source.bypassEffects = true;
                newClip.source.bypassReverbZones = true;
                sceneClips.Add(newClip);
                clip2Sound.Add(newClip, s);
            }
        }

        SceneManager.activeSceneChanged += OnSceneChange;
    }

    public void PlayByName(string name)
    {
        foreach (Clip s in sceneClips)
        {
            if (name.Equals(s.name))
            {
                s.source.volume = s.volume * ((s.isFX)? fxAmp : musicAmp);
                s.source.Stop();
                s.source.Play();
                return;
            }
        }
        Debug.Log("The audio clip " + name + " is not in the array.");
    }

    public void StopByName(string name)
    {
        foreach (Clip s in sceneClips)
        {
            if (name.Equals(s.name))
            {
                if (s.source.isPlaying)
                    s.source.Stop();
                return;
            }
        }
        Debug.Log("The audio clip " + name + " is not in the array.");
    }

    public void StopAll()
    {
        foreach (Clip s in sceneClips)
            if (s.source.isPlaying)
                s.source.Stop();
    }

    public void VolumeChange(string name, float endValue, float time)
    {
        foreach (Clip s in sceneClips)
        {
            if (name.Equals(s.name))
            {
                float origin = clip2Sound[s].volume;
                DOTween.To(() => s.volume, x => s.volume = x, origin * endValue, time).SetUpdate(true);
                return;
            }
        }
        Debug.Log("The audio clip " + name + " is not in the array.");
    }

    public void VolumeChange(float endValue, float time)
    {
        foreach (Clip s in sceneClips)
        {
            float origin = clip2Sound[s].volume;
            DOTween.To(() => s.volume, x => s.volume = x, origin * endValue, time).SetUpdate(true);
        }
    }

    /*以下不是音效系統的必要函式，拷貝此script到另一個專案時記得移除這些東西*/
    public UnityEvent OnGo = new UnityEvent();

    private void OnSceneChange(Scene current, Scene next)
    {
        StopByName("Scene_0_bgm");
        if(next.buildIndex == 0)
            PlayByName("Scene_0_bgm");
    }
}
