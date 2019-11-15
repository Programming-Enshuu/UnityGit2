using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigid2D;
    private Animator playerAnimater;
    private SpriteRenderer playerRenderer;

    private CircleCollider2D playerCircleCollider2D;

    private string groundTag = "Ground";//地面のタグ
    private string wallTag = "Wall";    //壁のタグ
    private string enemyTag = "Enemy";  //敵のタグ
    private string goalTag = "Goal";

    private bool isGroundEnter, isGroundStay, isGroundExit;    
    private bool isJump = false;
    private bool isGround = false;      //地面にいるかいないかの判定
    private bool isDamage = false;      //ダメージを受けたかどうか
    private bool isRight = false;
    private bool isLeft = false;
    public bool playerArive = true;     //プレイヤーの生存


    public int key = 0;                //プレイヤーの左右の向き
    [Header("プレイヤーのHP")]public int playerHp = 3;

    private float boost = 1.0f;         //ダッシュした時の倍率
    private float jumpForce = 680.0f;   //ジャンプ力
    private float walkForce = 20.0f;    //歩く力
    public float damageForce = 200.0f;  //被ダメージ時の吹き飛ばし力
    private float maxWalkSpeed = 3.0f;  //最大スピード
    private float speedX = 0.0f;        //スピード計算用
    [Header("踏みつけ判定の高さの割合")]public float stepOnRate;

    void Start()
    {
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.playerAnimater = GetComponent<Animator>();
        this.playerRenderer = GetComponent<SpriteRenderer>();
        this.playerCircleCollider2D = GetComponent<CircleCollider2D>();
    }

    void FixedUpdate()
    {
        //地面にいるかどうかの判定
        if (isGroundStay || isGroundEnter) isGround = true;
        else if (isGroundExit) isGround = false;

        if (isJump)
        {
            isJump = false;
            this.playerAnimater.SetTrigger("JumpTrigger");
            this.rigid2D.AddForce(transform.up * this.jumpForce);
        }

        if (isRight)
        {
            key = 1;
        }else if (isLeft)
        {
            key = -1;
        }
        else
        {
            key = 0;
        }

        //歩く速さの計算
        speedX = Mathf.Abs(this.rigid2D.velocity.x);
        if (speedX < maxWalkSpeed　*　boost) this.rigid2D.AddForce(transform.right * this.walkForce * key);

        isGroundEnter = false;
        isGroundStay = false;
        isGroundExit = false;
    }

    void Update()
    {
        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true) isJump = true;

        //プレイヤーの向き
        isRight = isLeft = false;
        boost = 1.0f;
        if (Input.GetKey(KeyCode.RightArrow)) isRight = true;
        if (Input.GetKey(KeyCode.LeftArrow)) isLeft = true;

        //shift押すとダッシュ
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) boost = 2.0f;

        //アニメーションの向き
        if (key != 0) transform.localScale = new Vector3(key, 1, 1);

        //アニメーションの速さ
        if (this.rigid2D.velocity.y == 0)
        {
            this.playerAnimater.speed = speedX / 2.0f;
        }
        else
        {
            this.playerAnimater.speed = 1.0f;
        }

        //プレイヤーの点滅
        if (isDamage == true && playerHp > 0)
        {
            float level = Mathf.Abs(Mathf.Sin(Time.time * 10));
            playerRenderer.color = new Color(1f, 1f, 1f, level);
        }

        if (playerHp <= 0 || this.rigid2D.position.y < -10f && playerArive == true)
        {
            playerHp = 0;
            playerArive = false;
            SceneManager.LoadScene("GameOver");
            Debug.Log("gameover");
            Destroy(this.gameObject);
        }
    }

    //地面にいるか判定するためのやつ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == groundTag || collision.tag == wallTag)
        {
            isGroundEnter = true;
        }
        if (collision.tag == goalTag)
        {
            Debug.Log("gameclear");
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == groundTag || collision.tag == wallTag)
        {
            isGroundStay = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == groundTag || collision.tag == wallTag)
        {
            isGroundExit = true;
        }
    }

    //敵に当たった時の処理
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == enemyTag)
        {
            //敵に当たった時のプレイヤーのポジションの収得
            float stepOnHeight = playerCircleCollider2D.radius * (stepOnRate / 100f);
            float judgePos = transform.position.y - (playerCircleCollider2D.radius) + stepOnHeight;

            //敵を踏んだ時の処理
            foreach (ContactPoint2D p in collision.contacts)
            {
                if(p.point.y < judgePos)
                {
                    ObjectCollision oc = collision.gameObject.GetComponent<ObjectCollision>();
                    oc.playerStepOn = true;
                    this.rigid2D.AddForce(transform.up * this.jumpForce / 2.0f);//敵を踏んだときにジャンプ
                }
                else if(p.point.y >= judgePos && isDamage == false)
                {
                    //プレイヤーがダメージを受けた時の処理
                    isDamage = true;
                    float s = 100f * Time.deltaTime;
                    transform.Translate(Vector3.right * s);
                    playerHp -= 1;
                    StartCoroutine("WaitForIt");
                }
            }
        }
    }

    //無敵時間の設定
    IEnumerator WaitForIt()
    {
        // 2秒間処理を止める
        yield return new WaitForSeconds(2);

        // 2秒後ダメージフラグをfalseにして点滅を戻す
        isDamage = false;
        playerRenderer.color = new Color(1f, 1f, 1f, 1f);
    }
}