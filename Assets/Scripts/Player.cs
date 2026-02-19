using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameHandler gameHandler;
    Vector2 vel_act;
    RaycastHit2D col;
    public BoxCollider2D col_idle, col_jump, col_crouched, col_aim_up, col_walk;
    public GameObject pies1, pies2;
    public List<KeyCode> teclas;
    public float vel_mov, vel_sal, dist_pies;
    bool puede_suelo = true, puede_saltar = true;
    float tiempo_aire = 0.0f;
    enum estado {tierra, aire, agua}
    estado estado_act = estado.aire;
    enum direccion {positivo, negativo, ninguno}
    direccion direccion_v = direccion.ninguno, direccion_h = direccion.ninguno;

    void Start()
    {
        gameHandler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        dist_pies = pies1.transform.position.y - pies2.transform.position.y;
    }

    void Update()
    {
        ProcesarTeclas();
        ProcesarDireccion();
        CheckDirection();
    }

    void FixedUpdate()
    {
        vel_act.y += Physics2D.gravity.y / 4 * Time.deltaTime;
        vel_act.y = Mathf.Clamp(vel_act.y, Physics2D.gravity.y / 4, -Physics2D.gravity.y);
        if (GetComponent<Animator>().GetInteger("state") == 5)
        {
            col = Physics2D.Raycast(new Vector2(pies2.transform.position.x, pies2.transform.position.y), new Vector2(0, -1), 0.08f, 1 << LayerMask.NameToLayer("Level"));
        }
        else
        {
            col = Physics2D.Raycast(new Vector2(pies1.transform.position.x, pies1.transform.position.y), new Vector2(0, -1), 0.01f, 1 << LayerMask.NameToLayer("Level"));
        }
        if (col && col.collider != null)
        {
            if (col.transform.tag == "Suelo")
            {
                if (puede_suelo)
                {
                    tiempo_aire = 0.0f;
                    col_jump.enabled = false;
                    if (GetComponent<Animator>().GetInteger("state") == 0)
                    {
                        col_crouched.enabled = false;
                        col_aim_up.enabled = false;
                        col_walk.enabled = false;
                        col_idle.enabled = true;
                    }
                    else if (GetComponent<Animator>().GetInteger("state") == 1 || GetComponent<Animator>().GetInteger("state") == 3 || GetComponent<Animator>().GetInteger("state") == 4)
                    {
                        col_idle.enabled = false;
                        col_crouched.enabled = false;
                        col_aim_up.enabled = false;
                        col_walk.enabled = true;
                    }
                    else if (GetComponent<Animator>().GetInteger("state") == 5)
                    {
                        col_idle.enabled = false;
                        col_aim_up.enabled = false;
                        col_walk.enabled = false;
                        col_crouched.enabled = true;
                    }
                    else if (GetComponent<Animator>().GetInteger("state") == 6)
                    {
                        col_idle.enabled = false;
                        col_crouched.enabled = false;
                        col_walk.enabled = false;
                        col_aim_up.enabled = true;
                    }
                    estado_act = estado.tierra;
                    vel_act.y = 0;
                    if (!puede_saltar)
                    {
                        puede_saltar = true;
                    }
                }
            }
            if (col.transform.tag == "Agua")
            {
                if (puede_suelo)
                {
                    col_jump.enabled = false;
                    if (GetComponent<Animator>().GetInteger("state") == 8)
                    {
                        
                    }
                    else
                    {

                    }
                    estado_act = estado.agua;
                    vel_act.y = 0;
                    if (!puede_saltar)
                    {
                        puede_saltar = true;
                    }
                }
            }
        }
        if (vel_act.y < 0)
        {
            tiempo_aire += Time.deltaTime;
            if (tiempo_aire > 0.2f)
            {
                estado_act = estado.aire;
                puede_suelo = true;
                col_idle.enabled = false;
                col_crouched.enabled = false;
                col_aim_up.enabled = false;
                col_walk.enabled = false;
                col_jump.enabled = true;
            }
        }
        transform.GetComponent<Rigidbody2D>().velocity = vel_act;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, gameHandler.minimo.transform.position.x, gameHandler.maximo.transform.position.x), transform.position.y, transform.position.z);
    }

    void ProcesarTeclas()
    {
        if (Input.GetKeyUp(teclas[0]))
        {
            if (GetComponent<Animator>().GetInteger("state") != 4 && GetComponent<Animator>().GetInteger("state") != 5)
            {
                direccion_v = direccion.ninguno;
            }
        }
        if (Input.GetKeyUp(teclas[1]))
        {
            if (GetComponent<Animator>().GetInteger("state") != 3 && GetComponent<Animator>().GetInteger("state") != 6)
            {
                direccion_v = direccion.ninguno;
            }
        }
        if (Input.GetKeyUp(teclas[2]) && direccion_h == direccion.negativo)
        {
            direccion_h = direccion.ninguno;
        }
        if (Input.GetKeyUp(teclas[3]) && direccion_h == direccion.positivo)
        {
            direccion_h = direccion.ninguno;
        }
        if (Input.GetKeyDown(teclas[0]))
        {
            if (GetComponent<Animator>().GetInteger("state") != 4 && GetComponent<Animator>().GetInteger("state") != 5)
            {
                direccion_v = direccion.positivo;
            }
        }
        if (Input.GetKeyDown(teclas[1]))
        {
            if (GetComponent<Animator>().GetInteger("state") != 3 && GetComponent<Animator>().GetInteger("state") != 6)
            {
                direccion_v = direccion.negativo;
            }
        }
        if (Input.GetKeyDown(teclas[2]))
        {
            if (estado_act != estado.agua)
            {
                direccion_h = direccion.negativo;
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                if (direccion_v != direccion.negativo)
                {
                    direccion_h = direccion.negativo;
                    GetComponent<SpriteRenderer>().flipX = false;
                }
            }
        }
        if (Input.GetKeyDown(teclas[3]))
        {
            if (estado_act != estado.agua)
            {
                direccion_h = direccion.positivo;
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                if (direccion_v != direccion.negativo)
                {
                    direccion_h = direccion.positivo;
                    GetComponent<SpriteRenderer>().flipX = true;
                }
            }
        }
        if (Input.GetKeyDown(teclas[4]) && puede_saltar)
        {
            vel_act.y += vel_sal;
            puede_saltar = false;
            puede_suelo = false;
            estado_act = estado.aire;
            col_idle.enabled = false;
            col_crouched.enabled = false;
            col_aim_up.enabled = false;
            col_walk.enabled = false;
            col_jump.enabled = true;
            Invoke("RestaurarColisionSuelo", 0.5f);
        }
    }

    void RestaurarColisionSuelo()
    {
        puede_suelo = true;
    }

    void ProcesarDireccion()
    {
        if (direccion_h == direccion.positivo)
        {
            vel_act.x = vel_mov;
        }
        else if (direccion_h == direccion.negativo)
        {
            vel_act.x = -vel_mov;
        }
        else
        {
            vel_act.x = 0;
        }
    }

    void CheckDirection()
    {
        if (estado_act != estado.aire)
        {
            if (direccion_h == direccion.ninguno && direccion_v == direccion.ninguno)
            {
                if (estado_act == estado.tierra)
                {
                    if (GetComponent<Animator>().GetInteger("state") == 5)
                    {
                        CorregirAltura(false);
                    }
                    GetComponent<Animator>().SetInteger("state", 0);
                }
                else if (estado_act == estado.agua)
                {
                    GetComponent<Animator>().SetInteger("state", 7);
                }
            }
            else if (direccion_h != direccion.ninguno)
            {
                if (estado_act == estado.tierra)
                {
                    if (direccion_v == direccion.ninguno)
                    {
                        GetComponent<Animator>().SetInteger("state", 1);
                    }
                    else
                    {
                        if (direccion_v == direccion.positivo)
                        {
                            GetComponent<Animator>().SetInteger("state", 3);
                        }
                        else
                        {
                            if (GetComponent<Animator>().GetInteger("state") == 5)
                            {
                                CorregirAltura(false);
                            }
                            GetComponent<Animator>().SetInteger("state", 4);
                        }
                    }
                }
                if (estado_act == estado.agua)
                {
                    GetComponent<Animator>().SetInteger("state", 9);
                }
            }
            else if (direccion_v == direccion.positivo)
            {
                if (estado_act == estado.tierra)
                {
                    GetComponent<Animator>().SetInteger("state", 6);
                }
                if (estado_act == estado.agua)
                {
                    GetComponent<Animator>().SetInteger("state", 7);
                }
            }
            else if (direccion_v == direccion.negativo)
            {
                if (estado_act == estado.tierra)
                {
                    if (GetComponent<Animator>().GetInteger("state") != 5)
                    {
                        CorregirAltura(true);
                    }
                    GetComponent<Animator>().SetInteger("state", 5);
                }
                if (estado_act == estado.agua)
                {
                    GetComponent<Animator>().SetInteger("state", 8);
                }
            }
        }
        else
        {
            if (GetComponent<Animator>().GetInteger("state") == 5)
            {
                CorregirAltura(true);
            }
            GetComponent<Animator>().SetInteger("state", 2);
        }
    }

    void CorregirAltura(bool pararse)
    {
        if (!pararse)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - dist_pies + 0.025f);
        }
        else
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + dist_pies + 0.025f);
        }
    }
}