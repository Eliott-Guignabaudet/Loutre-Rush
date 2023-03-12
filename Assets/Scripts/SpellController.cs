using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    public float remainingTime;
    public GameObject launcher;
    public float damages;


    void Update()
    {
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<AliveEntity>() == null)
        {
            return;
        }

        if (collision.gameObject != launcher)
        {
            collision.gameObject.GetComponent<AliveEntity>().TakeDamage(damages, launcher);
            Destroy(gameObject);
        }
    }
}
