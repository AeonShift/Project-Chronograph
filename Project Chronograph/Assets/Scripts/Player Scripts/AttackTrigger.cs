using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour {

    public int damage = 1;
    public EnemyHealthManager enemyHealthMan;

    void OnTrggerEnter2D(Collider2D col){

        if(col.isTrigger != true && col.CompareTag("Enemy")){

            enemyHealthMan.giveDamage(damage);

        }


    }

}
