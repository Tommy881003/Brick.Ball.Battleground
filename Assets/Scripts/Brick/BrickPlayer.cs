using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class BrickPlayer : MonoBehaviour
{
    public DeadLine line;
    public LayerMask scanLayer;

    [HideInInspector]
    public UnityEvent OnDie = new UnityEvent();
    [SerializeField]
    protected TextMeshProUGUI hpText = null; //顯示血量的文字
    [SerializeField]
    private GameObject brick = null;
    [SerializeField]
    private SpriteRenderer bloomRect = null;
    private ObjAudioManager playerAudio;

    [SerializeField]
    private Rigidbody2D rb;
    private Vector2 velocity;

    private bool canJump = true;

    [Range(10, 30)]
    public float JumpForce = 15;
    private float lowGravity = 3f, fallGravity = 4.5f, walkVelocity = 7.5f;

    public int hp; //生命值 
    private float regenTime;    //回血週期
    private float regenTimer;   //回血冷卻
    private int maxHp = 40;
    private float maxHpTime = 15;
    private float maxHpTimer;

    void Start()
    {
        rb.gravityScale = fallGravity;
        hp = maxHp;
        hpText.text = hp.ToString();
        regenTimer = 0;
        maxHpTimer = maxHpTime;
        playerAudio = GetComponent<ObjAudioManager>();
        line.OnPlayerEnter.AddListener(Suicide);
        SceneAudioManager.instance.OnGo.AddListener(StartUp);
        Camera.main.gameObject.GetComponent<CameraController>().End.AddListener(end);
        enabled = false;
    }

    private void Update()
    {
        velocity = rb.velocity;
        JumpGravity(velocity);
        CheckCollision();
        /* Control moving */
        HorizontalMoving(velocity);
        if (Input.GetKeyDown(KeyCode.W))
            Jump(velocity);
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -walkVelocity, walkVelocity), rb.velocity.y);
    }

    private void LateUpdate()
    {
        SpawnBrick();
    }

    private void FixedUpdate()
    {
        regenTimer -= Time.fixedDeltaTime;
        maxHpTimer -= Time.fixedDeltaTime;
        regenTime = Mathf.Max(0.125f, hp / (maxHp * 2.5f));
        if(regenTimer <= 0)
        {
            hp = Mathf.Min(hp + 1, maxHp);
            hpText.text = hp.ToString();
            regenTimer = regenTime;
        }
        if(maxHpTimer <= 0)
        {
            maxHp += 5;
            maxHpTimer = maxHpTime;
        }
    }

    void SpawnBrick()
    {
        int x = Mathf.FloorToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);
        float catchFall = Fall.instance.transform.position.y - (int)Fall.instance.transform.position.y;
        Vector2 spawnPos = new Vector2(x + 0.5f, y - 0.5f + catchFall);
        spawnPos = new Vector2(spawnPos.x, (transform.position.y - spawnPos.y) > 0.95f ? spawnPos.y : spawnPos.y - 1);
        
        RaycastHit2D hitInfo = Physics2D.Raycast(spawnPos, Vector2.zero, 0, scanLayer);

        bloomRect.gameObject.transform.position = spawnPos;
        bloomRect.color = (hitInfo.collider == null) ? Color.white : Color.red;

        if(Input.GetKeyDown(KeyCode.S) && hp > 10 && hitInfo.collider == null)
        {
            playerAudio.PlayByName("Place");
            GameObject go = Instantiate(brick, spawnPos, Quaternion.identity);
            go.transform.SetParent(Fall.instance.transform);
            go.GetComponent<Brick>().hp = hp / 3;
            hp -= hp / 5;
            hpText.text = hp.ToString();
        }
    }

    void HorizontalMoving(Vector2 velocity)
    {
        int dir = 0; // 1 for right, -1 for left, 0 for none
        if (Input.GetKey(KeyCode.D))
            dir ++;
        else if (Input.GetKey(KeyCode.A))
            dir --;
        rb.AddForce(new Vector2(75 * dir, 0));
        if (dir == 0)
            rb.velocity = new Vector2(0.5f * rb.velocity.x, rb.velocity.y);
    }

    void Jump(Vector2 velocity)
    {
        if (canJump == true)
        {
            playerAudio.PlayByName("Jump");
            rb.velocity = new Vector2(rb.velocity.x,JumpForce);
            canJump = false;
        }
    }

    void JumpGravity(Vector2 velocity)
    {
        if (velocity.y > 0 && Input.GetKey(KeyCode.W))
            rb.gravityScale = lowGravity;
        else
            rb.gravityScale = fallGravity;
    }

    void CheckCollision()
    {
        Vector2 leftfoot, rightfoot;
        leftfoot = new Vector2(transform.position.x - 0.5f * transform.localScale.x, transform.position.y - 0.5f * transform.localScale.y);
        rightfoot = new Vector2(rb.transform.position.x + 0.5f * transform.localScale.x, transform.position.y - 0.5f * transform.localScale.y);
        RaycastHit2D hit = Physics2D.CircleCast(leftfoot, 0.04f, rightfoot - leftfoot, (rightfoot - leftfoot).magnitude, 1 << LayerMask.NameToLayer("Brick"));
        if (hit.collider != null && hit.collider.isTrigger == false)
            canJump = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ball")) //如果撞到的東西是球
        {
            hp -= collision.gameObject.GetComponent<Ball>().GetDamage(); //損血
            hpText.text = hp.ToString(); //顯示新的血量
        }
        if (hp <= 0)
            OnDie.Invoke();
            /*Destroy(gameObject);*/ //如果血量低於零，銷毀此物件
    }

    public void Suicide()
    {
        hp = 0;
        enabled = false;
        OnDie.Invoke();
    }

    void StartUp()
    {
        enabled = true;
    }

    void end()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        enabled = false;
    }
}
