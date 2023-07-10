using EasyBuildSystem.Features.Runtime.Buildings.Placer;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject buildManager, BuildUI, MainUI,buildingPlacerGO;
    public BuildingPlacer buildingPlacer;
    

    // Start is called before the first frame update
    void Start()
    {
        buildingPlacer = buildingPlacerGO.GetComponent<BuildingPlacer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildingPlacer.enabled = true;
            BuildUI.SetActive(true);
            buildManager.SetActive(true);
            MainUI.SetActive(false);

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            buildingPlacer.enabled = false;
            BuildUI.SetActive(false);
            buildManager.SetActive(false);
            MainUI.SetActive(true);
        }
    }
}
