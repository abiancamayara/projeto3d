using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CombatEnemy : MonoBehaviour
{
    [Header("Atributtes")] 
    public float totalHealth;
    public float attackDamage;
    public float movementSpeed;
    public float lookRadius;
    public float colliderRadius = 2;

    [Header("Components")]
    private Animator anim;
    private CapsuleCollider capsule;
    private NavMeshAgent agent;

    [Header("Others")] 
    private Transform player;

    private bool walking;
    private bool attacking;
    private bool hiting;
    private bool waitFor;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= lookRadius)
        {
            //o personagem está no raio de ação
            agent.isStopped = false;
            if (!attacking)
            {
                agent.SetDestination(player.position);
                anim.SetBool("Walk Forward", true);
                walking = true;
            }
            if (distance <= agent.stoppingDistance)
            {
                StartCoroutine("Attack");
            }
            else
            {
                attacking = false;
            }
        }
        else
        {
            //o personagem não está no raio de ação
            anim.SetBool("Walk Forward", false);
            agent.isStopped = true; 
            walking = false;
            attacking = false;
        }
        
    }
    
    IEnumerator Attack()
    {
        if (!waitFor)
        {
            waitFor = true;
            attacking = true;
            walking = false;
            anim.SetBool("Walk Forward", false);
            anim.SetBool("Claw Attack", true);
            yield return new WaitForSeconds(1.5f);
            GetPlayer();
            //yield return new WaitForSeconds(1f);
            waitFor = false;
        }
    }

    void GetPlayer()
    {
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (c.gameObject.CompareTag("Player"))
            {
                //vai causar dano no player 
                Debug.Log("atacou");
            }
        }
    }

    public void GetHit(float damage)
    {
        totalHealth -= damage;
        //totalHealth = totalHealth - damage;
        if (totalHealth > 0)
        {
            //esta vivo
            StopCoroutine("Attack");
            anim.SetTrigger("Take Damage");
            hiting = true;
            
        }
        else
        {
            //esta morto
            anim.SetTrigger("Die");
        }
    }

    IEnumerator RecoverFromHit()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Walk Forward", false);
        anim.SetBool("Claw Attack", false);
        hiting = false;
        waitFor = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
