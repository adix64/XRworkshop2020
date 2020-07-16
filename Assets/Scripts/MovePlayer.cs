using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MovePlayer : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float rotSpeed = 10f;
    public Transform L, R, F, B;
    public GameObject projectile;
    public float jumpPower = 700f;
    Transform camTransform;
    Rigidbody rigidBody;
    CapsuleCollider capsule;
    Vector3 initialPos;
    Vector3 groundNormal;
    bool isOnGround = false;
    Animator animator;
    Transform particles;
    void Start()
    {// apelat o singura data, la initializare
        camTransform = Camera.main.transform;
        rigidBody = GetComponent<Rigidbody>(); // sau transform.GetComponent<Rigidbody>() sau gameObject.GetComponent<Rigidbody>()
        capsule = GetComponent<CapsuleCollider>();
        initialPos = transform.position;
        groundNormal = Vector3.up;
        animator = GetComponentInChildren<Animator>();
        particles = GetComponentInChildren<ParticleSystem>().transform;
    }

    void Update()
    {// apelat de N ori pe secunda, preferabil N > 60FPS pentru fluiditate
      
        float h = Input.GetAxis("Horizontal");//-1 pentru tasta A, 1 pentru tasta D si 0 altfel
        float v = Input.GetAxis("Vertical"); //-1 pentru tasta S, 1 pentru tasta W si 0 altfel

        Vector3 dir = (h * camTransform.right + v * camTransform.forward).normalized;
        if (dir.magnitude > 0f)
            dir = Vector3.ProjectOnPlane(dir, groundNormal).normalized; //directia trebuie sa fie tangenta pe sol       

        CheckIfOnGround();
        HandleTranslation(dir);
        HandleRotation(dir);
        AnimateCharacter(dir);
        HandleAttack();

        if (isOnGround && Input.GetButtonDown("Jump"))
        {
            rigidBody.AddForce(Vector3.up * jumpPower);
            animator.SetTrigger("Jump");
        }

      
        ArrowDisplay(h, v);
       
        if (transform.position.y < -10f) // player a cazut in gol
            transform.position = initialPos; // reset pozitie

        Debug.DrawLine(transform.position, transform.position + dir * 3, Color.blue, 1f);
    }

    void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 projectilePos = transform.position + transform.forward * 0.5f;
            var obj = GameObject.Instantiate(projectile);
            Projectile projectileCtrl = obj.GetComponent<Projectile>();
            projectileCtrl.direction = transform.forward;
            obj.transform.position = projectilePos;
            obj.transform.rotation = Quaternion.AngleAxis(90f, transform.right) * transform.rotation;
        }
        particles.gameObject.SetActive(Input.GetButton("Fire1"));
    }
    void AnimateCharacter(Vector3 dir)
    {
        dir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
        Vector3 localCoords = transform.InverseTransformDirection(dir);
        animator.SetFloat("Forward", localCoords.z);
        animator.SetFloat("Turn", localCoords.x);
    }
    void CheckIfOnGround()
    { // raycast din interiorul, centrul capsulei
        //pentru ca originea lui ray e in interiorul capsulei, ray nu va intersecta capsula
        Ray ray = new Ray(transform.position, Vector3.down);
        // ne interesea intersectia doar putin(epsilon) sub capsula
        float epsilon = 10E-2f; // 0.01f
        float maxDist = capsule.height / 2f * (1f + epsilon);
        //raycast
        isOnGround = Physics.Raycast(ray, out RaycastHit rayHit, maxDist);
        if (isOnGround)
            groundNormal = rayHit.normal;
        else
            groundNormal = Vector3.up;
    }

    void HandleTranslation(Vector3 dir)
    {
        //transform.position += dir * Time.deltaTime * mSpeed; // nu e recomandat...
        //...sa suprascriem pozitia cand avem un Rigidbody atasat, decat daca era Kinematic
        //in schimb:
        Vector3 newVelocity = dir * moveSpeed;

        if (isOnGround)
            rigidBody.velocity = newVelocity;
        else // il putem controla putin si in aer + se elimina si bug-ul "getting stuck on a ledge"
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, newVelocity, Time.deltaTime);
    }

    void HandleRotation(Vector3 dir)
    {
        dir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized; //pentru ca si forward e tot in planul xOz, personajul sta vertical
        float fwdDotDir = Vector3.Dot(transform.forward, dir); // ||fwd|| * ||dir|| * cos(angle(fwd, dir))
        if (fwdDotDir < 1f) // transform.forward este diferit de dir
        {
            Vector3 axis = Vector3.up;
            float angle = 15f; // cat rotim daca fwd si dir sunt opuse, poate fi oricate grade
            if (fwdDotDir > -1f) // fwd si dir sa nu fie opuse, i.e. fwd != -dir, ca axis sa poata fi calculat
            {
                axis = Vector3.Cross(transform.forward, dir);
                angle = Mathf.Acos(fwdDotDir) * Mathf.Rad2Deg
                        * Time.deltaTime * rotSpeed;
            }

            transform.rotation = Quaternion.AngleAxis(angle, axis) * transform.rotation;
        }
    }

    void ArrowDisplay(float h, float v)
    {
        L.gameObject.SetActive(h < 0f);
        R.gameObject.SetActive(h > 0f);
        F.gameObject.SetActive(v > 0f);
        B.gameObject.SetActive(v < 0f);
    }
}

//https://docs.unity3d.com/Manual/CollidersOverview.html