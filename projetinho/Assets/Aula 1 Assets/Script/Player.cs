using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    private CharacterController controller;
    private Transform cam;
    private Vector3 moveDirection;
    public float gravity;
    public float colliderRadius;
    public float smoothRotTime;
    private float turnSmoothVelocity;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        GetMouseinput();
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
                //armazena a rotação e o angulo da camera 
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                //armazena a rotação mais suave 
                float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity,
                    smoothRotTime);
                //rotaciona o personagem 
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                //armazena a direção 
                moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed;
                
                anim.SetInteger("transition", 1);
            }
            else
            {
                moveDirection = Vector3.zero;  
                anim.SetInteger("transition", 0);
            }

        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void GetMouseinput()
    {
        if (controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        anim.SetInteger("transition", 2);
        yield return new WaitForSeconds(10f);
    }

    void GetEnemiesList()
    {
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, colliderRadius);
    }
}