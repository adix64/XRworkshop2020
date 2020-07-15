﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float rotSpeed = 10f;
    public Transform L, R, F, B;
    public float jumpPower = 700f;
    Transform camTransform;
    Rigidbody rigidBody;
    CapsuleCollider capsule;
    Vector3 initialPos;
    bool isOnGround = false;

    void Start()
    {// apelat o singura data, la initializare
        camTransform = Camera.main.transform;
        rigidBody = GetComponent<Rigidbody>(); // sau transform.GetComponent<Rigidbody>() sau gameObject.GetComponent<Rigidbody>()
        capsule = GetComponent<CapsuleCollider>();
        initialPos = transform.position;
    }

    void Update()
    {// apelat de N ori pe secunda, preferabil N > 60FPS pentru fluiditate
      
        float h = Input.GetAxis("Horizontal");//-1 pentru tasta A, 1 pentru tasta D si 0 altfel
        float v = Input.GetAxis("Vertical"); //-1 pentru tasta S, 1 pentru tasta W si 0 altfel

        Vector3 dir = (h * camTransform.right + v * camTransform.forward).normalized;
        if (dir.magnitude > 0f)
            //dir.y = 0; dir = dir.normalized;// pentru miscare in plan orizontal... sau:
            dir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;        

        CheckIfOnGround();
        HandleTranslation(dir);
        HandleRotation(dir);
            
        if (isOnGround && Input.GetButtonDown("Jump"))
            rigidBody.AddForce(Vector3.up * jumpPower);
        // else ...motorul de fizica gestioneaza deja deplasarea in aer
        
        ArrowDisplay(h, v);
       
        if (transform.position.y < -10f) // player a cazut in gol
            transform.position = initialPos; // reset pozitie
    }

    void CheckIfOnGround()
    { // raycast din interiorul, centrul capsulei
        //pentru ca originea lui ray e in interiorul capsulei, ray nu va intersecta capsula
        Ray ray = new Ray(transform.position, Vector3.down);
        // ne interesea intersectia doar putin(epsilon) sub capsula
        float epsilon = 10E-3f; // 0.001f
        float maxDist = capsule.height / 2f * (1f + epsilon);
        //raycast
        isOnGround = Physics.Raycast(ray, maxDist);
    }

    void HandleTranslation(Vector3 dir)
    {
        //transform.position += dir * Time.deltaTime * mSpeed; // nu e recomandat...
            //...sa suprascriem pozitia cand avem un Rigidbody atasat, decat daca era Kinematic
        //in schimb:
        Vector3 newVelocity = dir * moveSpeed
                            + rigidBody.velocity.y * Vector3.up; // pastram componenta verticala
        if (isOnGround)
            rigidBody.velocity = newVelocity;
        else // il putem controla putin si in aer, se elimina si bug-ul "getting stuck on a ledge"
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, newVelocity, Time.deltaTime);
    }

    void HandleRotation(Vector3 dir)
    {
        float fwdDotDir = Vector3.Dot(transform.forward, dir); // ||fwd|| * ||dir|| * cos(angle(fwd, dir))
        if (fwdDotDir < 1f) // transform.forward este diferit de dir
        {
            Vector3 axis = Vector3.up;
            float angle = 45f;
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