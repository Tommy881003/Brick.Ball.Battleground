using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField]
    private BallProperty property;

                                //SerializeField讓我們可以在unity介面上替這個變數指派值
    [SerializeField,Range(5,25)]//Range讓我們們可以在unity介面上用滑桿調整數值
    private float speed = 10;   //砲台左右移動的速度

    private Camera cam;         //主攝像機
    private Vector3 mousePos;   //紀錄滑鼠的世界座標
    private Vector3 direction;  //紀錄砲台應指向的方向向量
    private float fireTime;     //紀錄砲台發射週期
    private float timer;        //發射的冷卻
    public static float angle = 90;     //紀錄砲台的旋轉,設static以方便球取得目前的角度

    [SerializeField]            
    private Transform muzzle = null;    //發射的位置(槍口)
    [SerializeField]
    private GameObject ball = null;     //拿來發射的球

    [SerializeField]
    private LayerMask trajectoryLayer = 0;
    [SerializeField,Range(5,30)]
    private float trajectoryLength = 15;
    private LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        property.Initialize();
        cam = Camera.main; //獲取主攝像機
        lr = GetComponent<LineRenderer>();
        fireTime = 1 / property.fireRate; //週期 = 頻率的倒數
        timer = 0; //冷卻歸零
        SceneAudioManager.instance.OnGo.AddListener(StartUp);
        Camera.main.gameObject.GetComponent<CameraController>().StartEnd.AddListener(end);
        enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*發射砲彈*/
        timer -= Time.fixedDeltaTime; //計算新的冷卻時間
        if(timer <= 0) //如果冷卻結束
        {
            Instantiate(ball, muzzle.position, Quaternion.identity); //在槍口位置生成球
            timer = fireTime; //進入冷卻
        }

        /* 計算砲台的移動 */
        CalculatePosition();
        /* 計算砲台的旋轉 */
        CalculateRotation();
        /* 計算彈道 */
        ForecastTrajectory();
    }

    void CalculatePosition()
    {
        Vector2 pos = transform.position; //得到目前砲台的位置
        if (Input.GetKey(KeyCode.LeftArrow))
            pos += new Vector2(-speed * Time.fixedDeltaTime, 0); //如果有按左方向鍵，往左移
        if (Input.GetKey(KeyCode.RightArrow))
            pos += new Vector2(speed * Time.fixedDeltaTime, 0); //如果有按左方向鍵，往右移
        pos = new Vector2(Mathf.Clamp(pos.x, -12, 12), pos.y); //確保得到的新位置不會壞掉(例如超出邊界)
        transform.position = pos; //把算出來的位置套用到砲台上
    }

    void CalculateRotation()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition); //獲得滑鼠的世界座標
        direction = mousePos - transform.position; //獲得砲台指向滑鼠座標的向量
        angle = Vector2.Angle(Vector2.right, direction); //計算砲台應該指向的角度(指向滑鼠)
        angle = Mathf.Clamp(angle, 15, 165); //確保獲得的角度不會壞掉(例如指到下方)
        transform.rotation = Quaternion.Euler(0, 0, angle); //把算出來的旋轉套用到砲台上
    }

    void ForecastTrajectory()
    {
        List<Vector3> reflects = new List<Vector3>();
        float length = 0;
        float reflectAngle = angle * Mathf.Deg2Rad;
        Vector3 origin = muzzle.position;
        Vector3 reflectDir = new Vector2(Mathf.Cos(reflectAngle), Mathf.Sin(reflectAngle));
        reflects.Add(origin);

        int i = 0;
        while (length < trajectoryLength)
        {
            if (i >= 10)
                break;
            RaycastHit2D hitInfo = Physics2D.CircleCast(origin, 0.1f, reflectDir, 100, trajectoryLayer);
            if(hitInfo.collider == null || hitInfo.distance > trajectoryLength - length)
            {
                reflects.Add(origin + reflectDir.normalized * (trajectoryLength - length));
                break;
            }
            else
            {
                reflects.Add(hitInfo.point + hitInfo.normal.normalized * 0.1f);
                length += hitInfo.distance;
                origin = hitInfo.point + hitInfo.normal.normalized * 0.1f;
                reflectDir = Vector2.Reflect(reflectDir, hitInfo.normal);
            }
            i++;
        }

        lr.positionCount = reflects.Count;
        for (int j = 0; j < reflects.Count; j++)
            lr.SetPosition(j, reflects[j]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Buff"))
        {
            property.SetValue(collision.gameObject.GetComponent<BuffCarrier>().buff);
            Instantiate(collision.gameObject.GetComponent<BuffCarrier>().text, new Vector3(transform.position.x, -6), Quaternion.identity);
            Destroy(collision.gameObject);
            fireTime = 1 / property.fireRate;
        }   
    }

    void StartUp()
    {
        enabled = true;
    }

    void end()
    {
        enabled = false;
    }
}
