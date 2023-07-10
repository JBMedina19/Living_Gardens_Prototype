using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class TreeFruitManager : MonoBehaviour
{
    public SpawnFruit[] _spawnFruitpos;
    public GameObject[] _spawnFruit;
    public float[] spawnTimer;
    public Collider[] spawnerCollider;  
    // Start is called before the first frame update
    private void Start()
    {
        
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < _spawnFruit.Length; i++)
        {
            if (_spawnFruit[i].activeInHierarchy == false && _spawnFruitpos[i].canSpawn && _spawnFruit[i].transform.childCount >= 0)
            {
                spawnTimer[i]-= Time.deltaTime;
                if (spawnTimer[i]<=0)
                {
                    _spawnFruit[i].SetActive(true);
                    spawnTimer[i] = 10;
                }


            }
        }

    }




}
