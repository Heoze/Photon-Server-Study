using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class BulletMove : MonoBehaviourPunCallbacks, IPunObservable
{
    ChattingManager CM;
    public PhotonView PV;
    AudioManager AM;

    Transform trans;
    SpriteRenderer renderer;
    Animator anim;

    bool stop = false; 

    ScoreManager sm;

    Vector3 curPos;

    void Awake() {
        CM = GameObject.Find("GameManager").GetComponent<ChattingManager>();
        AM = GameObject.Find("Audio").GetComponent<AudioManager>();

        trans = GetComponent<Transform>();
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        sm = GameObject.FindWithTag("LocalPlayer").GetComponent<ScoreManager>();

        Destroy(gameObject, 0.5f);

        if(PV.IsMine) gameObject.layer = 12;
        else gameObject.layer = 11;
    }

    void Update()
    {   
        if(PV.IsMine) {
            if(!stop) {
                Vector3 toward = transform.position;
                if(transform.rotation.y != 0) 
                    toward += new Vector3(-0.1f, 0, 0);
                else
                    toward += new Vector3(0.1f, 0, 0);

                transform.position = Vector3.MoveTowards(transform.position, toward, 0.1f);
            }
        }
        else if ((transform.position - curPos).sqrMagnitude >= 2) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(PV.IsMine) {
            if(other.gameObject.tag != "Bullet") {
                stop = true;
                PV.RPC("DestroyRPC", RpcTarget.All);

                if(other.gameObject.tag == "Enemy") {
                    EnemyMove em = other.gameObject.GetComponent<EnemyMove>();
                    if(em.HP <= 1 && em.blink == 0) {
                        AM.sound[7].Play();
                        sm.Score+=5;
                    }
                }
                if(other.gameObject.tag == "Player") {
                    PlayerMove pm = other.gameObject.GetComponent<PlayerMove>();

                    if(pm.HP <= 1) {
                        AM.sound[7].Play();
                        sm.Score += 10;
                        CM.KillMessage(PhotonNetwork.NickName, other.gameObject.name);
                    }
                }
            }
        }
    }

    [PunRPC]
    void DestroyRPC() {
        anim.SetBool("Collision", true);
        renderer.flipX = renderer.flipX ? false : true;

        Destroy(gameObject, 0.5f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(transform.position);
        else
            curPos = (Vector3)stream.ReceiveNext();
    }
}
