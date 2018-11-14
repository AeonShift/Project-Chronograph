﻿using System.Collections;
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
    public RaycastPlayerController playerCont;



    private void Start()
    {
        //GameObject player = GameObject.Find("RaycastPlayer");
        //RaycastPlayerController playerCont = player.GetComponent<RaycastPlayerController>();
        //playerCont.canMove = false;
    }


    private void Update()
    {
        if(timeBtwAttack <= 0){
            //then you can attack
            if(Input.GetButtonDown("Attack") && !attacking){
                attacking = true;
                timeBtwAttack = startTimeBtwAttack;
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    enemiesToDamage[i].GetComponent<EnemyHealthManager>().giveDamage(Damage);

                }

            }

        }

        if (attacking  && animator.GetBool("isGrounded")){
            if(timeBtwAttack > 0){

                playerCont.canMove = false;
                timeBtwAttack -= Time.deltaTime;
            }
            else {

                playerCont.canMove = true;
                attacking = false;
            }

        }
        if (attacking && !animator.GetBool("isGrounded"))
        {
            if (timeBtwAttack > 0)
            {

                playerCont.canMove = true;
                timeBtwAttack -= Time.deltaTime;
            }
            else
            {

                playerCont.canMove = true;
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
