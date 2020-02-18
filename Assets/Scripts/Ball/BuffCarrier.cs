using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffCarrier : MonoBehaviour
{
    public BallBuff buff;
    public GameObject text;
    private float fallSpeed = 5;

    private void FixedUpdate()
    {
        transform.position += new Vector3(0, -1, 0) * fallSpeed * Time.fixedDeltaTime;
        if (transform.position.y < -10)
            Destroy(gameObject);
    }
}
