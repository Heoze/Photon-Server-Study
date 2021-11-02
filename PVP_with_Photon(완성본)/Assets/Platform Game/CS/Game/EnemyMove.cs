
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    AudioManager AM;

    public float HP;

    public Vector3 direction;
    public Vector3 toward;

    public GameObject enemy;

    SpriteRenderer renderer;
    Transform transform;
    Animator animator;
    Rigidbody2D rigid;

    void Start()
    {
        AM = GameObject.Find("Audio").GetComponent<AudioManager>();

        renderer = GetComponent<SpriteRenderer>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        rigid = gameObject.GetComponent<Rigidbody2D>();


        Think();
    }
    
    void FixedUpdate() {
        Move();
    }

    void Think() {
        // 이동 방향 설정
        direction = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);

        toward = transform.position + direction;

        toward.x = Mathf.Clamp(toward.x, -10, 20);
        toward.y = Mathf.Clamp(toward.y, -10, 8);

        // 이미지 방향 설정
        if(direction.x > 0) renderer.flipX = false;
        else if (direction.x < 0) renderer.flipX = true;

        float nextThinkTime = Random.Range(3f, 5f);

        // 반복
        Invoke("Think", nextThinkTime);
    }

    void Move() { // 설정된 방향대로 이동
        transform.position = Vector3.Lerp(transform.position, toward, 0.01f);
    }

    public int blink;
    void Blink() {
        if(blink > 0) {
            if(blink%2 == 1) 
                renderer.color = new Color(1f, 1f, 1f, 1f);
            else
                renderer.color = new Color(1f, 0.6f, 0.6f, 0.5f);
            
            blink--;
            Invoke("Blink", 0.2f);
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Bullet") { // Bullet
            if(blink == 0) {
                AM.sound[5].Play();
                Debug.Log("총알 맞았다!");

                HP -= 1;

                if(HP <= 0) {
                    AM.sound[6].Play();
                    EnemySpawn.spawnedNum -= 1;
                    Destroy(enemy, 0.1f);
                }
                blink = 4;
                Invoke("Blink", 0.2f);
            }
        }
    }

}
