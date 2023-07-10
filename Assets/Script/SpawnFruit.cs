using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;


public class SpawnFruit : MonoBehaviour
{

    public SpawnFruit sp;
    public GameObject spawnFruit;
    public Collider col;
    public bool canSpawn;

    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpawnFruit>();
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void OnEnable()
    {
        Rigidbody rb;
        GameObject fruit = Instantiate(spawnFruit, transform.position, Quaternion.identity);
        fruit.transform.SetParent(gameObject.transform);
        rb = fruit.GetComponent<Rigidbody>();
        rb.useGravity = false;
        col.enabled = true;
    }

    [Obsolete]
    private  void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            col.enabled = false;
            canSpawn = true;
        }
    }
}
