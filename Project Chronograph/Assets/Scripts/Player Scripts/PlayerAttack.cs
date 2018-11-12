using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    private float timeBtwAttack;
    public float startTimeBtwAttack;
    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange;
    public int Damage;
    public Animator animator;
    public bool attacking = false;

    private void Update()
    {
        if(timeBtwAttack <= 0){
            //then you can attack
            if(Input.GetButton("Attack") && !attacking){
                attacking = true;
                timeBtwAttack = startTimeBtwAttack;
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    enemiesToDamage[i].GetComponent<EnemyHealthManager>().giveDamage(Damage);

                }

            }

        }

        if(attacking){
            if(timeBtwAttack > 0){

                timeBtwAttack -= Time.deltaTime;
            }
            else {

                attacking = false;
            }

        }
       
        animator.SetBool("isAttacking", attacking);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
