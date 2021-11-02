using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{  
    public GameObject[] enemylist;
    public Transform[] spawnspot;

    public int MaxNum;
    static public int spawnedNum;

    void Update() {
        if(spawnedNum < MaxNum) {
            Instantiate(enemylist[Random.Range(0, 2)], spawnspot[Random.Range(0, 5)]);
            spawnedNum++;
        }
    }
}
