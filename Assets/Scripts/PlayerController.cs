using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;


public class PlayerController : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 10f;
    public Rigidbody2D rb;

    private float _hp;
    private float _maxHp;
    private float _xp;
    private int _level;
    private float _maxXpLevel;
    private float _spellDamage;
    public float hp { 
        get { return _hp; } 
        set { 
            _hp = value;
            if (photonView.IsMine)
            {
                Slider healthBarSlider = healthBar.GetComponent<Slider>();
                healthBarSlider.value = _hp / _maxHp;
            }
            
        }

    }

    public float xp
    {
        get { return _xp; }
        set
        {
            _xp = value;
            if (_xp >= _maxXpLevel)
            {
                level++;
                xp -= _maxXpLevel;
                _spellDamage += 10;
            }

            if (photonView.IsMine)
            {
                Slider experienceBarSlider = experienceBar.GetComponent<Slider>();
                experienceBarSlider.value = _xp/_maxXpLevel;
            }
        }
    }

    public int level
    {
        get { return _level; }
        set
        {
            _level = value;
            if (photonView.IsMine)
            {
                levelUI.GetComponent<TextMeshProUGUI>().text = _level.ToString();
            }
        }
    }

    

    public float launchPower = 10;
    public GameObject spellPrefab;

    public GameObject endPanel;

    public GameObject healthBar;
    public GameObject experienceBar;
    public GameObject levelUI;



    private bool isPaused;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            healthBar = GameObject.Find("HealthBar");
            experienceBar = GameObject.Find("ExperienceBar");
            levelUI = GameObject.Find("LevelUI");
        }

        _spellDamage = 50;
        _maxHp = 250;
        _maxXpLevel = 200;
        hp = 250;
        level = 1;
        xp = 0;

    }



    void Update()
    {
        if (photonView.IsMine && !isPaused)
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

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) 
        {
            return;
        }

        if (!collision.gameObject.CompareTag("Spell"))
        {
            return;
        }

        SpellController spell = collision.gameObject.GetComponent<SpellController>();
        GameObject launcher = spell.launcher;

        if (spell.launcher != gameObject)
        {
            hp -= spell.damages;
            Destroy(spell.gameObject);
        }

        if (hp <=0)
        {
            DisplayEndScreen();
            isPaused = true;
            launcher.GetComponent<PlayerController>().isPaused = true;
        }
    }*/

    public void TakeDamage(float damages, GameObject launcher)
    {
        hp -= damages;
        if (hp <= 0)
        {
            DisplayEndScreen();
            isPaused = true;
            launcher.GetComponent<PlayerController>().isPaused = true;
        }

    }

    public void DisplayEndScreen()
    {
        
        if (photonView.IsMine)
        {
            Loose();
            return;
        }
        Win();
    }

    public void Win()
    {
        GameObject newEndPanel = Instantiate(endPanel, Vector3.zero, Quaternion.identity);
        newEndPanel.transform.parent = GameObject.Find("Canvas").transform;
        newEndPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
        TextMeshProUGUI endPanelText = newEndPanel.GetComponent<EndPanel>().victoryText;
        endPanelText.text = "You Win";
        endPanelText.color = Color.blue;
    }

    public void Loose()
    {
        GameObject newEndPanel = Instantiate(endPanel, Vector3.zero, Quaternion.identity);
        newEndPanel.transform.parent = GameObject.Find("Canvas").transform;
        newEndPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
        TextMeshProUGUI endPanelText = newEndPanel.GetComponent<EndPanel>().victoryText;
        endPanelText.text = "You Loose";
        endPanelText.color = Color.red;
    }

    [PunRPC]
    public void Fire(Vector3 pos, Vector3 dir, PhotonMessageInfo info)
    {
        GameObject bullet;

        bullet = Instantiate(spellPrefab, pos, Quaternion.identity);
        SpellController spellController = bullet.GetComponent<SpellController>();
        spellController.damages = _spellDamage;
        spellController.launcher = gameObject;
        spellController.remainingTime = 1f;

        bullet.GetComponent<Rigidbody2D>().AddForce(dir * launchPower);
        Debug.Log(bullet.GetComponent<Rigidbody2D>());
    }
}
