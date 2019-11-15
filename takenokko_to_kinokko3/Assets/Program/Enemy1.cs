using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{   
    private Rigidbody2D rigid2D;
    private SpriteRenderer enemyRenderer;
    private ObjectCollision oc;
    private BoxCollider2D enemyBoxCollider2D;
    private CircleCollider2D enemyCircleCollider2D;
    private Animator enemyAnimater;

    private string wallTag = "Wall";
    private string enemyTag = "Enemy";

    private bool isDead = false;
    [Header("画面外でも敵が動くかどうか")]public bool nonVisibleAct;//画面外でも動くかどうか

    public int enemyMove = -1;

    private float turnTime = 0.0f;      //向きを変えるまでの時間
    private float deadTime = 0.0f;      //死んでから消えるまでの時間
    private float walkForce = 10.0f;    //敵の歩く力
    private float enemySpeedX = 0.0f;   //スピード計算用
    private float maxWalkSpeed = 2.0f;  //最大スピード

    // Start is called before the first frame update
    void Start()
    {        
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.enemyRenderer = GetComponent<SpriteRenderer>();
        this.enemyAnimater = GetComponent<Animator>();
        this.oc = GetComponent<ObjectCollision>();
        this.enemyBoxCollider2D = GetComponent<BoxCollider2D>();
        this.enemyCircleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (enemyRenderer.isVisible || nonVisibleAct)
        {
            if (oc.playerStepOn == false)
            {
                    enemySpeedX = Mathf.Abs(this.rigid2D.velocity.x);
                    if (enemySpeedX < maxWalkSpeed) this.rigid2D.AddForce(transform.right * this.walkForce * enemyMove);
                    transform.localScale = new Vector3(enemyMove, 1, 1);
                    this.enemyAnimater.speed = enemySpeedX / 3.0f;
            }
        }
        if (isDead)
        {
            transform.Rotate(new Vector3(0, 0, 10 * enemyMove));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //敵が死んだ時の描写と処理
        if (isDead == false && oc.playerStepOn == true)
        {
            this.enemyBoxCollider2D.enabled = false;
            this.enemyCircleCollider2D.enabled = false;

            transform.localScale = new Vector3(1, 0.5f, 0);

            isDead = true;
        }
        else if (isDead == true)
        {
            

            if (deadTime > 1.0f)
            {
                Destroy(this.gameObject);
            }
            else
            {
                deadTime += Time.deltaTime;
            }
        }


    }

    //壁に当たったら向き変更
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == wallTag || collision.tag == enemyTag)
        {
            enemyMove *= -1;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == wallTag || collision.tag == enemyTag)
        {
            if (turnTime > 1.5f)
            {
                enemyMove *= -1;
                turnTime = 0f;
            }
            else
            {
                turnTime += Time.deltaTime;
            }
        }
    }
}
