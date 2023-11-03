using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float gravity;
    public float damage = 20;
    public float colliderRadius;
    public float smoothRotTime;
    private float turnSmoothVelocity;
    public float totalHealth;
    private bool waitFor;
    private bool isHitting;
    public bool isDead; 
    
    private CharacterController controller;
    private Transform cam;
    private Vector3 moveDirection;

    private Animator anim;
    
    public List<Transform> enemyList = new List<Transform>();
    private bool isWalking;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        anim = GetComponent<Animator>();
        
        GameController.instance.Coracao(totalHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            Move();
            GetMouseInput();
        }
    }

    private void Move()
    {
        if (controller.isGrounded)
        {
            //pega a entrada na horizontal (tecla direita/esquerda)
            float horizontal = Input.GetAxisRaw("Horizontal");
            //pega a entrada na vertical (tecla cima/baixo)
            float vertical = Input.GetAxisRaw("Vertical");
            //variavel local que armazena o valor do eixo horizontal e vertical 
            Vector3 direction = new Vector3(horizontal, 0f, vertical);

            //verifica se o personagem está se movimentando (se for > 0)
            if (direction.magnitude > 0)
            {
                if (!anim.GetBool("attack"))
                {
                    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                    float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity,
                        smoothRotTime);
                    transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                    moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed;
                    anim.SetInteger("transition", 1);
                    isWalking = true;
                }
                else
                {
                    moveDirection = Vector3.zero;  
                    anim.SetBool("walk", false);
                }
            }
            else if (isWalking)
            {
                anim.SetInteger("transition", 0);
                anim.SetBool("walk", false);
                moveDirection = Vector3.zero;
                isWalking = false;
                //anim.SetInteger("transition", 0);
            }

        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void GetMouseInput()
    {
        if (controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (anim.GetBool("walk"))
                {
                    anim.SetBool("walk", false);
                    anim.SetInteger("transition", 0);
                }

                if (!anim.GetBool("walk"))
                {
                    StartCoroutine("Attack");
                }
            }
        }
    }

    IEnumerator Attack()
    {
        if (!waitFor && !isHitting)
        {
            waitFor = true;

            anim.SetBool("attack", true);
            anim.SetInteger("transition", 2);
            
            yield return new WaitForSeconds(0.4f);
            GetEnemiesList();

            foreach (Transform e in enemyList)
            {
                Debug.Log(e.name);
                CombatEnemy enemy = e.GetComponent<CombatEnemy>();

                if (enemy != null)
                {
                    enemy.GetHit(damage);
                }
            }

            yield return new WaitForSeconds(0.5f);
            anim.SetInteger("transition", 0);
            anim.SetBool("attack", false);
            waitFor = false; 
        }
    }

    void GetEnemiesList()
    {
        enemyList.Clear();
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
                enemyList.Add(c.transform);
            }
        }
    }
    
    public void GetHit(float damage)
    {
        totalHealth -= damage;
        //GameController.instance.UpdateLives(totalHealth);
        
        //totalHealth = totalHealth - damage;
        if (totalHealth > 0)
        {
            //esta vivo
            StopCoroutine("Attack");
            anim.SetInteger("transition", 3);
            isHitting = true;
            StartCoroutine("RecoverFromHit");

        }
        else
        {
            //esta morto
            isDead = true;
            anim.SetTrigger("death");
        }
    }

    public void IncreaseLife(float value)
    {
        totalHealth += value;
        GameController.instance.Coracao(totalHealth);
    }

    IEnumerator RecoverFromHit()
    {
        yield return new WaitForSeconds(1f);
        anim.SetInteger("transition", 0);
        isHitting = false;
        anim.SetBool("attacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, colliderRadius);
    }
}