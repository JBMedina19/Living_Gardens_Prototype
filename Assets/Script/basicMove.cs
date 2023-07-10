using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class basicMove : MonoBehaviour
{
    public Transform targetPos;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMove(targetPos.position,30);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
