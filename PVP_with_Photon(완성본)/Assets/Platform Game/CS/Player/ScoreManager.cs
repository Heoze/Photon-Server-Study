using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    GameManager GM;
    AudioManager AM;
    public PlayerMove PM;
    SpriteRenderer renderer;
    Transform trans;

    public int HP;
    public Image[] healthView = new Image[3];
    public Sprite[] healthImage;

    public int Score;
    public Image[] scoreView = new Image[3];
    public Sprite[] scoreImage;

    // Start is called before the first frame update
    void Start() {

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        AM = GameObject.Find("Audio").GetComponent<AudioManager>();

        renderer = GetComponent<SpriteRenderer>();
        trans = GetComponent<Transform>();

        for(int i = 0; i < 3; i++) {
            healthView[i] = GM.healthView[i];
            scoreView[i] = GM.scoreView[i];
        }
    }

    // Update is called once per frame
    void Update() {
        sync_HP();
        sync_Score();
    }

    void sync_HP() {
        int tmp = HP;
        for(int i = 0; i < 3; i++) {
            if(tmp > 1)
                healthView[i].sprite = healthImage[0];
            else if(tmp == 1)
                healthView[i].sprite = healthImage[1];
            else
                healthView[i].sprite = healthImage[2];
            tmp -= 2;
        }
    }

    void sync_Score() {
        int tmp = Score;
        for(int i = 0; i < 3; i++) {
            scoreView[i].sprite = scoreImage[tmp%10];
            tmp /= 10;
        }
    }

    public int blink;
    public void Blink() {
        if(blink > 0) {
            if(blink%2 == 1) 
                renderer.color = new Color(1f, 1f, 1f, 1f);
            else
                renderer.color = new Color(1f, 1f, 1f, 0.5f);
            blink--;
            Invoke("Blink", 0.25f);
        }
    }
    void OnCollisionEnter2D(Collision2D other) {
        if(PM.PV.IsMine) {
            if(other.gameObject.tag == "Bullet" || other.gameObject.tag == "Enemy") { // Bullet
                if(HP == 1) {
                    AM.sound[5].Play();

                    HP -= 1;
                }
                else if(blink == 0) {
                    AM.sound[5].Play();

                    HP -= 1;

                    blink = 5;
                    Blink();
                }
            }
        }
    }
}
