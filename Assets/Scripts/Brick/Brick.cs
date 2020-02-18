using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Brick : MonoBehaviour
{
    private int seed;
    [SerializeField]
    private GameObject[] buffs;
    private GameObject buffToSpawn = null;
    [SerializeField]
    private GameObject destroy;
    public int hp = 10; //生命值 

    [SerializeField]
    private TextMeshProUGUI hpText = null; //顯示血量的文字

    private void Start()
    {
        float rand = Random.Range(0f, 1f);
        seed = Mathf.FloorToInt(rand * 200);
        if (seed < buffs.Length)
            buffToSpawn = buffs[seed];
        hpText.text = hp.ToString(); //顯示血量
    }

    private void Update()
    {
        if (transform.position.y < -10)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ball")) //如果撞到的東西是球
        {
            hp -= collision.gameObject.GetComponent<Ball>().GetDamage(); //損血
            hpText.text = hp.ToString(); //顯示新的血量
        }          
        if (hp <= 0)
        {
            if(buffToSpawn != null)
                Instantiate(buffToSpawn, transform.position, Quaternion.identity);
            GameObject go = Instantiate(destroy, transform.position, Quaternion.identity);
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;
            main.startColor = GetComponentInChildren<SpriteRenderer>().color;
            ps.Play();
            Destroy(gameObject); //如果血量低於零，銷毀此物件
        }
    }
}
