using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Transform transform;
    Transform player;

    void Start()
    {
        transform = GetComponent<Transform>();
    }

    void Update() {
        if(!player) {
            if(GameObject.FindWithTag("LocalPlayer") != null)
                player = GameObject.FindWithTag("LocalPlayer").transform;
            transform.position = new Vector3(0,0,0);
        }
        else {
            Vector3 tmp = player.position;
            tmp += new Vector3(0, 1.5f, 0);

            float newX = Mathf.Clamp(tmp.x, -4, 15);
            float newY = Mathf.Clamp(tmp.y, -10, 6);

            tmp = new Vector3(newX, newY, -5);

            transform.position = Vector3.MoveTowards(transform.position, tmp, 1f);
        }
    }
}
