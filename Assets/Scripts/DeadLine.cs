using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeadLine : MonoBehaviour
{
    public UnityEvent OnPlayerEnter = new UnityEvent();
    public UnityEvent OnBrickEnter = new UnityEvent();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            if (collision.attachedRigidbody.velocity.y < 0)
                Destroy(collision.gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Brick"))
            OnBrickEnter.Invoke();

        if (collision.gameObject.layer == LayerMask.NameToLayer("Brick Player"))
            OnPlayerEnter.Invoke();
    }
}
