using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using System.Linq;
using DG.Tweening;

public class PatrolState : StateMachineBehaviour
{

    PetManager petManager;
    NavMeshAgent petAgent;
    Animator petAnimation;
    List<Transform> wayPoints = new List<Transform>();
    private int currentWaypointIndex = 0;
    float timer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        petManager = animator.GetComponent<PetManager>();
        petManager.timerToAvoid = 5;
        petAgent = animator.GetComponent<NavMeshAgent>();
        petAnimation = animator.GetComponent<Animator>();
        wayPoints = petManager.wayPoints.ToList();
        timer = petManager.timerToAvoid;
        Debug.Log(wayPoints.Count);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public async void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (!petAgent.pathPending && petAgent.remainingDistance < 0.5f)
        {
            // Wait for a few seconds before moving to the next waypoint
            await Task.Delay(2000);
            WaitBeforeNextWaypoint(2000);
        }

        RaycastHit[] hits = Physics.SphereCastAll(animator.transform.position, petManager.radius, Vector3.forward);

        foreach (RaycastHit hit in hits)
        {

            Collider[] colliders = Physics.OverlapSphere(animator.transform.position, petManager.radius);

            foreach (Collider collider in colliders)
            {
                
                // Check if the detected collider has the target tag
                if (collider.CompareTag("Player") && petManager.ePersonality == PetPersonality.Lonely)
                {
                    petManager.timerToAvoid -= Time.deltaTime;
                    if (petManager.timerToAvoid<=0)
                    {
                        petAnimation.SetBool("Patrol",false);
                        petAnimation.SetBool("Avoid", true);

                    }

                }
                if (collider.CompareTag("Player") && petManager.ePersonality == PetPersonality.Social)
                {
                    petManager.timerToAvoid -= Time.deltaTime;
                    if (petManager.timerToAvoid <= 0)
                    {
                        petAnimation.SetBool("Patrol", false);
                        petAnimation.SetBool("Follow", true);

                    }

                }
            }
        }

       
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    public async void WaitBeforeNextWaypoint(int waitTime)
    {
        // Disable the NavMeshAgent temporarily
        //petAgent.enabled = false;

        // Wait for the specified time
        await Task.Delay(waitTime);

        // Enable the NavMeshAgent and set the next waypoint
        petAgent.enabled = true;
        SetNextWaypoint();
    }
    void SetNextWaypoint()
    {
        if (wayPoints.Count == 0)
            return;

        // Set the destination of the NavMeshAgent to the current waypoint
        petAgent.destination = wayPoints[currentWaypointIndex].position;
        currentWaypointIndex = (currentWaypointIndex + 1) % wayPoints.Count;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
