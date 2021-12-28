using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

// 에셋에서 Photon Pun2 Free 임포트
using Photon.Pun;
using Photon.Realtime;

public class PlayerMove : MonoBehaviourPunCallbacks {
    public PhotonView PV;

    public Text NicknameView;

    public Transform trans;
    public Rigidbody2D rigid;
    public Animator anim;
    public SpriteRenderer renderer;

    public AudioManager am;

    void Awake() {
        am.sound[2].Play();

        if(PV.IsMine) { // 내가 소환했나?
            NicknameView.text = PhotonNetwork.LocalPlayer.NickName;
            NicknameView.color = Color.green;
        }
        else {
            NicknameView.text = PV.Owner.NickName; // PV.Owner => Player 객체를 반환
            NicknameView.color = Color.red;
        }
    }

    void Update() {
        if(PV.IsMine) {
            Move();
            Jump();
            Shot();
        }
    }

    // 플레이어 이동
    void Move() {
        float direction = Input.GetAxis("Horizontal");
        
        trans.Translate(new Vector3(direction/10, 0f, 0f));

        if(trans.position.y < -15f) {
            am.sound[2].Play();
            trans.position = new Vector3(0f, 0f, 0f);
        }

        if(direction != 0) {
            anim.SetBool("isWalking", true);

            if(direction > 0)
                renderer.flipX = false;
            else
                renderer.flipX = true;
        }
        else
            anim.SetBool("isWalking", false);

        // renderer.flipX 동기화
        PV.RPC("Sync_FlipX", RpcTarget.All, renderer.flipX);
    }

    [PunRPC]
    void Sync_FlipX(bool flipX) {
        renderer.flipX = flipX;
    }

    // 플레이어 점프
    void Jump() {
        if(Input.GetKeyDown(KeyCode.Z)) {
            am.sound[4].Play();

            rigid.AddForce(new Vector3(0f, 10f, 0f), ForceMode2D.Impulse);
            anim.SetTrigger("Jumping");
        }
    }

    // 총알 발사
    void Shot() {
        if(Input.GetKeyDown(KeyCode.X)) {
            am.sound[3].Play();

            // 총알 생성
            GameObject bullet =  PhotonNetwork.Instantiate("bullet", transform.position, Quaternion.identity);
            

            // 총알 위치 조정
            SpriteRenderer bullet_renderer = bullet.GetComponent<SpriteRenderer>();
            Rigidbody2D bullet_rigid = bullet.GetComponent<Rigidbody2D>();

            if(renderer.flipX) {
                bullet.transform.Translate(-0.8f, -0.15f, 0f);
                bullet_rigid.AddForce(new Vector3(-10f, 0f, 0f), ForceMode2D.Impulse);
            }
            else {
                bullet.transform.Translate(0.8f, -0.15f, 0f);
                bullet_rigid.AddForce(new Vector3(10f, 0f, 0f), ForceMode2D.Impulse);
            }

            bullet_renderer.flipX = renderer.flipX;

            Destroy(bullet, 2f);
        }
    }
}
