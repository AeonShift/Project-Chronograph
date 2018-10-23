using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour {

    public int damage = 1;
    public EnemyHealthManager Ehealthman;

    void OnTrggerEnter2D(Collider2D col){

        if(col.isTrigger != true && col.CompareTag("Enemy")){

            Ehealthman.giveDamage(damage);

        }


    }

}
