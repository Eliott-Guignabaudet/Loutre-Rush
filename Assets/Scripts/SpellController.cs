using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    public float remainingTime;
    public GameObject launcher;
    public int damages;


    void Update()
    {
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
