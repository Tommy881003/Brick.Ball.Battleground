using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField]
    private GameObject ballHit;
    [SerializeField]
    private BallProperty property;
    private Rigidbody2D rb;
    private float speed;    //速度
    private int reflect;  //反彈
    private int damage;     //傷害值

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //從遊戲物件上獲得2D剛體
        float angle = Turret.angle * Mathf.Deg2Rad; //從砲台上獲得發射角度，角度要換成弧度制
        speed = property.speed; //從property上獲得速度
        rb.velocity = speed * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)); //套用速度
        damage = property.damage; //從property上獲得傷害值
        reflect = property.reflect; //從property上獲得反彈次數
    }

    private void OnCollisionEnter2D(Collision2D collision)  //發生碰撞時呼叫
    {
        Instantiate(ballHit, collision.GetContact(0).point, Quaternion.identity);
        SceneAudioManager.instance.PlayByName("Hit");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            reflect--;
            if (reflect <= 0)
                Destroy(gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        rb.velocity = rb.velocity.normalized * speed; //確保碰撞後速度量值維持一致
    }

    public int GetDamage() 
    { 
        return damage; 
    }
}
