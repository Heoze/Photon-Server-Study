using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerMove : MonoBehaviour {
    public Transform trans;
    public Rigidbody2D rigid;
    public Animator anim;
    public SpriteRenderer renderer;

    void Awake() {

    }

    void Update() {
        Move();
        Jump();
        Shot();
    }

    // 플레이어 이동
    void Move() {                                      // rigidbody.AddForce로 하면 더 자연스럽게 구현 가능하다.
        float direction = Input.GetAxis("Horizontal");
        // Input.GetAxis("Horizontal"): 방향키 입력 -1.0 ~ 1.0 (음수면 왼쪽, 양수면 오른쪽, 0이면 눌리지 않음)
        direction /= 10; // 수치 조정(= 속도 조절)

        trans.Translate(new Vector3(direction, 0f, 0f)); 
        // Transform.Translate: 상대 좌표값을 주면, 그 만큼 이동한다.

        if(trans.position.y < -15f)
            trans.position = new Vector3(0f, 0f, 0f);


        // 애니메이션
        if(direction != 0) { // 이동 중이라면,
            anim.SetBool("isWalking", true);

            if(direction > 0) // 오른쪽으로 이동 중
                renderer.flipX = false;
            else
                renderer.flipX = true;
        }
        else // 이동 중이 아니라면
            anim.SetBool("isWalking", false);
    }

    // 플레이어 점프
    void Jump() {
        if(Input.GetKeyDown(KeyCode.Z)) {
            rigid.AddForce(new Vector3(0f, 10f, 0f), ForceMode2D.Impulse);
            // rigid.AddForce: rigidbody2D에 힘을 주어서 위치 변경시키기
            // ForceMode.Impulse: 순간적으로 힘을 줄 때 (Ex. 타격, 폭발..)

            // 애니메이션
            anim.SetTrigger("Jumping");
        }
    }

    // 총알 발사
    public GameObject bulletPrefab;

    void Shot() {
        if(Input.GetKeyDown(KeyCode.X)) {

            // 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            

            // 총알 위치 조정
            SpriteRenderer bullet_renderer = bullet.GetComponent<SpriteRenderer>();
            // bullet의 SpriteRenderer 컴포넌트 가져오기

            if(renderer.flipX) // 플레이어가 왼쪽을 향해 있음
                bullet.transform.Translate(-0.8f, -0.15f, 0f);
            else 
                bullet.transform.Translate(0.8f, -0.15f, 0f);

            bullet_renderer.flipX = renderer.flipX;


            // 총알 발사
            Rigidbody2D bullet_rigid = bullet.GetComponent<Rigidbody2D>();

            if(renderer.flipX) // 플레이어가 왼쪽을 향해 있음
                bullet_rigid.AddForce(new Vector3(-10f, 0f, 0f), ForceMode2D.Impulse);
            else
                bullet_rigid.AddForce(new Vector3(10f, 0f, 0f), ForceMode2D.Impulse);


            // 5초 뒤에 총알 삭제하기
            Destroy(bullet, 2f);
        }
    }
}
