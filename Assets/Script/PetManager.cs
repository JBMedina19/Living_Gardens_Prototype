using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public enum PetEvolution
{
    Egg,
    Child,
    Teen,
    Adult
}
public enum PetPersonality
{
    Social,
    Lonely,
    Grumpy,
    Normal
}

public class PetManager : MonoBehaviour
{
    //stats
    public int exp;
    public int level;
    public int health;
    public int energy;
    public int happiness;
    public int Loyalty;
    public int Agility;
    public int hunger;
    public PetPersonality ePersonality;
    public string personalityType;

    public GameObject[] PetLooks;
    public PetEvolution ePetEvol;
    public string targetTag;
    public float radius = 5f;
    public ParticleSystem heartsPartSys;
    //patrol way points
    public List<Transform> wayPoints;
    public Transform player;
    public float timerToAvoid = 5;
    Animator anim;
    bool levelUp;
    // Start is called before the first frame update
    void Start()
    {
        levelUp = false;
        anim = GetComponent<Animator>();
        level = 1;
        PetPersonality randomEnum = (PetPersonality)Random.Range(0, System.Enum.GetValues(typeof(PetPersonality)).Length);
        ePersonality = randomEnum;
        personalityType = ePersonality.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        switch (ePetEvol)
        {
            case PetEvolution.Egg:
                PetLooks[0].SetActive(true);
                PetLooks[1].SetActive(false);
                PetLooks[2].SetActive(false);
                PetLooks[3].SetActive(false);
                break;
            case PetEvolution.Child:
                PetLooks[0].SetActive(false);
                PetLooks[1].SetActive(true);
                PetLooks[2].SetActive(false);
                PetLooks[3].SetActive(false);
                break;
            case PetEvolution.Teen:
                PetLooks[0].SetActive(false);
                PetLooks[1].SetActive(false);
                PetLooks[2].SetActive(true);
                PetLooks[3].SetActive(false);
                break;
            case PetEvolution.Adult:
                PetLooks[0].SetActive(false);
                PetLooks[1].SetActive(false);
                PetLooks[2].SetActive(false);
                PetLooks[3].SetActive(true);
                break;
            default:
                break;
        }
        switch (ePersonality)
        {
            case PetPersonality.Social:
               
                break;
            case PetPersonality.Lonely:
                break;
            case PetPersonality.Grumpy:
                break;
            case PetPersonality.Normal:
                if (level == 2 && levelUp)
                {
                    ePetEvol = PetEvolution.Child;
                    health += 3;
                    Agility += 2;
                    energy += 1;
                    anim.SetBool("isEgg", false);
                    anim.SetBool("Patrol", true);
                    levelUp = false;
                }
                break;
            default:
                break;
        }

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, Vector3.forward);

        foreach (RaycastHit hit in hits)
        {
            // Check if the hit object has the desired tag
            //if (hit.collider.CompareTag(targetTag))
            //{
            //    // Do something with the collected object
            //    GameObject collectedObject = hit.collider.gameObject;
                
            //    Debug.Log("Collected: " + collectedObject.name);

            //    // Optionally, you can remove or disable the collected object
            //    // collectedObject.SetActive(false);
            //    // or
            //    // Destroy(collectedObject);
            //}
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider collider in colliders)
            {
                // Check if the detected collider has the target tag
                if (collider.CompareTag(targetTag))
                {
                    Collider col;
                    col = collider.GetComponent<Collider>();
                    col.enabled = false;
                    Rigidbody rb;
                    rb = collider.GetComponent<Rigidbody>();
                    rb.useGravity = false;
                    collider.transform.DOMove(gameObject.transform.position, 2f).SetEase(Ease.Linear);
                    collider.transform.DOScale(Vector3.zero, 2f).SetEase(Ease.Linear).OnComplete(() => AddXP(collider));
                    // Replace this line with your desired logic for collecting the objects
                    

                    // For example, you can destroy the collected object:
                   
                }
            }
            if (exp>=10)
            {
                exp = 0;
                level++;
                levelUp = true;
            }
            if (level == 2 && levelUp)
            {
                ePetEvol = PetEvolution.Child;
                health += 3;
                Agility += 2;
                energy += 1;
                anim.SetBool("isEgg", false);
                anim.SetBool("Patrol", true);
                levelUp = false;
            }
            if (level == 3 && levelUp)
            {
                ePetEvol = PetEvolution.Teen;
                health += 3;
                Agility += 2;
                energy += 1;
                levelUp = false;
            }
            if (level == 4 && levelUp)
            {
                ePetEvol = PetEvolution.Adult;
                health += 3;
                Agility += 2;
                energy += 1;
                levelUp = false;
            }
        }
    }

    public void AddXP(Collider collider)
    {
        Destroy(collider.gameObject);
        exp += 2;
        heartsPartSys.Play();
        hunger+=2;
    }
    public void Interaction()
    {
        heartsPartSys.Play();
        happiness++;
        Loyalty++;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
