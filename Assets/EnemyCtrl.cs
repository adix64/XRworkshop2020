using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyCtrl : MonoBehaviour
{
    public Transform player;
    UnityEngine.AI.NavMeshAgent agent;
    Animator animator;
    int dirType = 0; // 0 -- inainte, 1 -- stanga, 2 -- dreapta, 3 -- spate, 4 -- idle
    // Start is called before the first frame update
    [Range(1f, 10f)]
    public float workaround = 1f;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        StartCoroutine(ChangeDir(1f));
    }
    IEnumerator ChangeDir(float t)
    {
        yield return new WaitForSeconds(t);
        dirType = UnityEngine.Random.Range(0, 5);
        yield return StartCoroutine(ChangeDir(UnityEngine.Random.Range(1f, 3f)));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.InverseTransformDirection(agent.velocity);

        animator.SetFloat("Forward", forward.z);
        animator.SetFloat("Turn", forward.x);

        Vector3 destination = transform.position;

        switch (dirType)
        { // randomly surround player 
            case 0:
                destination = player.position;
                break;
            case 1:
                destination = player.position - transform.right * workaround;
                break;
            case 2:
                destination = player.position + transform.right * workaround;
                break;
            case 3:
                destination = player.position - transform.forward * workaround;
                break;
            default:
                break;
        }
        
        agent.SetDestination(destination);
        Vector3 toPlayer = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(transform.forward, toPlayer) * transform.rotation;
        if (agent.remainingDistance < 1.5f)
            animator.SetTrigger("Punch");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.other.CompareTag("PlayerHitbox"))
            animator.SetTrigger("GetHit");
    }
}
