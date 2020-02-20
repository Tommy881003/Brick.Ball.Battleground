using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : MonoBehaviour
{
    public static Fall instance = null;

    [SerializeField]
    private GameObject brick;
    private float speed = 0;
    private float timer = -1;
    private int floor = 0;
    private int width = 28;
    private int height;
    private float seed;

    [SerializeField, Range(5, 15)]
    private float addHpTime = 8;
    private float addHptimer = 0;
    private int addHp = 0;

    public Gradient grad;

    [Range(0,1)]
    public float threshold = 0.5f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Camera.main.gameObject.GetComponent<CameraController>().Phase2.AddListener(StartUp);
        Camera.main.gameObject.GetComponent<CameraController>().StartEnd.AddListener(end);
        height = (int)Camera.main.orthographicSize;
        seed = Random.Range(0.1f, 100f);
        enabled = false;
    }

    private void StartUp()
    {
        enabled = true;
    }

    void end()
    {
        enabled = false;
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(0, -speed * Time.fixedDeltaTime, 0);
        timer += Time.fixedDeltaTime;
        speed = 0.25f * Mathf.Log10(Mathf.Max(timer,1)) + 0.4f;

        addHptimer += Time.fixedDeltaTime;
        if (addHptimer >= addHpTime * (addHp + 1))
            addHp++;

        if(transform.position.y < floor)
        {
            SpawnBrick(height - floor);
            floor --;
        }
    }

    private void SpawnBrick(int y)
    {
        for (int i = 0; i < width; i++)
        {
            float value = Mathf.PerlinNoise(i/4f + seed,(y-height)/4f + seed);
            if(value >= threshold)
            {
                GameObject go = Instantiate(brick, new Vector3(0, 10000, 0), Quaternion.identity, transform);
                go.transform.localPosition = new Vector3(-(width / 2) + 0.5f + i, y + 0.5f);
                go.GetComponentInChildren<SpriteRenderer>().color = grad.Evaluate((value - threshold) / (1 - threshold));
                go.GetComponent<Brick>().hp += addHp;
            }
        }
    }
}
