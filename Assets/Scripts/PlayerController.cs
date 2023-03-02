using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 10f;
    public Rigidbody2D rb;
    public float hp = 500;
    public float launchPower = 10;
    public GameObject spellPrefab;

    void Start()
    {
        
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInput();
        }
        
    }

    public void ProcessInput()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetAxis("Horizontal") != 0)
        {
            movement += Vector2.right * moveSpeed * Input.GetAxis("Horizontal") ;
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            movement += Vector2.up * moveSpeed * Input.GetAxis("Vertical");
        }

        if (movement != Vector2.zero)
        {
            rb.velocity = new Vector2(movement.x, movement.y);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseInScreen = Input.mousePosition;
            mouseInScreen.z = 10;
            Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mouseInScreen);
            Vector3 launchOrigin;
            Vector3 direction;


            direction = (mouseInWorld - transform.position).normalized;
            launchOrigin = ((mouseInWorld - transform.position).normalized ) + transform.position;
            
            photonView.RPC("Fire", RpcTarget.AllViaServer, launchOrigin, direction);
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) 
        {
            return;
        }

        if (collision.gameObject.CompareTag("Spell"))
        {
            Destroy(collision.gameObject);
            hp -= 50;
        }
    }

    [PunRPC]
    public void Fire(Vector3 pos, Vector3 dir, PhotonMessageInfo info)
    {
        GameObject bullet;

        bullet = Instantiate(spellPrefab, pos, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().AddForce(dir * launchPower);
        Debug.Log(bullet.GetComponent<Rigidbody2D>());
    }
}
