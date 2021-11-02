using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class PlayerMove : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PV;
    GameManager GM;
    AudioManager AM;

    public ScoreManager SM;

    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer renderer;

    public Text NickNameText;

    public float jumpPower;
    public float movePower;

    public GameObject explosion;
    SpriteRenderer exp_renderer;
    Transform exp_transform;

    Vector3 curPos;

    GameObject bullet;

    public int HP;

    void Awake() {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        AM = GameObject.Find("Audio").GetComponent<AudioManager>();

        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();

        exp_renderer = explosion.GetComponent<SpriteRenderer>();
        exp_transform = explosion.GetComponent<Transform>();

        // 닉네임 불러오기
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

        if(PV.IsMine) {
            AM.sound[2].Play();

            gameObject.layer = 8;
            gameObject.tag = "LocalPlayer";
            gameObject.name = PhotonNetwork.NickName;
        }
        else {
            gameObject.GetComponent<ScoreManager>().enabled = false;
            gameObject.name = PV.Owner.NickName;
        }

    }

    void FixedUpdate()
    {
        Move();
        Jump();
    }

    void Update()
    {
        Shot();
    }

    Vector3 movement;
    void Move()
    {
        if(PV.IsMine) {
            Vector3 moveVelocity = Vector3.zero;

            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                moveVelocity = Vector3.left;
                renderer.flipX = true;
            }

            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                moveVelocity = Vector3.right;
                renderer.flipX = false;
            }

            transform.position += moveVelocity * movePower * Time.deltaTime;

            if (moveVelocity == Vector3.zero) // isWalking false
                anim.SetBool("isWalking", false);
            else // true
                anim.SetBool("isWalking", true);

            PV.RPC("AnimRPC", RpcTarget.All, anim.GetBool("isWalking"), renderer.flipX);

            if (transform.position.y < -15) {
                AM.sound[2].Play();

                SM.blink = 10;
                SM.Blink();

                transform.position = new Vector3(0, 0, 0);
            }

            if(SM.HP <= 0) {
                AM.sound[6].Play();
                Invoke("dead", 0.5f);
                SM.HP=6;
            }
            PV.RPC("colorRPC", RpcTarget.All, SM.blink > 0 && SM.blink%2 == 0);

            HP = SM.HP;
            PV.RPC("hpRPC", RpcTarget.All, HP);

            if(SM.Score >= 100) {
                PV.RPC("GameEndRPC", RpcTarget.All, PhotonNetwork.NickName);
            }
        }
        else {
            if ((transform.position - curPos).sqrMagnitude >= 10) transform.position = curPos;
            else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 9999999);
        }
    }

    void dead() {
        /*GM.Roaded();
        PV.RPC("DestroyRPC", RpcTarget.All);*/

        transform.position = new Vector3(0, 0, 0);
        SM.blink = 10;
        SM.Blink();
        SM.Score = Mathf.Clamp(SM.Score-50, 0, 999);
    }

    void Jump()
    {
        if(PV.IsMine) {
            if(Input.GetKeyDown(KeyCode.Z)) {
                AM.sound[4].Play();
                PV.RPC("JumpRPC", RpcTarget.All);

                rigid.velocity = Vector2.zero;

                Vector2 jumpVelocity = new Vector2(0, jumpPower);
                rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);
            }
        }
    }

    int shot_num;
    void deactivate() {
        shot_num--;

        if(shot_num == 0)
            exp_renderer.color = new Color(1f, 1f, 1f, 0f);
    }
    void Shot() {
        if(PV.IsMine) {
            if(Input.GetKeyDown(KeyCode.C)) {
                Quaternion tmp;
                if(renderer.flipX)
                    tmp = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                else
                    tmp = Quaternion.Euler(new Vector3(0f, 0f, 0f));

                bullet = PhotonNetwork.Instantiate("Bullet", exp_transform.position, tmp);
                PV.RPC("ShootRPC", RpcTarget.All);
            }
        }
        exp_renderer.flipX = renderer.flipX;

        exp_transform.position = transform.position;
        if(renderer.flipX) {
            exp_transform.position += new Vector3(-0.83f, -0.18f, 0);
        }
        else {
            exp_transform.position += new Vector3(0.83f, -0.18f, 0);
        }
    }

    // 애니메이션 정보 전달
    [PunRPC]
    void AnimRPC(bool isWalking, bool filpX){
        anim.SetBool("isWalking", isWalking);
        renderer.flipX = filpX;
    } 

    [PunRPC]
    void JumpRPC()=> anim.SetTrigger("Jumping");

    [PunRPC]
    void colorRPC(bool blink) {
        if(blink) renderer.color = new Color(1f, 1f, 1f, 0.5f);
        else renderer.color = new Color(1f, 1f, 1f, 1f);
    }

    [PunRPC]
    void ShootRPC() {
        AM.sound[3].Play();

        anim.SetTrigger("Shooting");
        exp_renderer.color = new Color(1f, 1f, 1f, 1f);

        Invoke("deactivate", 0.5f);
        shot_num++;
    }

    [PunRPC]
    void hpRPC(int hp)=> HP = hp;

    [PunRPC]
    void GameEndRPC(string winner) {
        GM.GameEnding(winner);
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
    
    // 위치 정보 공유
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(transform.position);
        else
            curPos = (Vector3)stream.ReceiveNext();
    }
}
