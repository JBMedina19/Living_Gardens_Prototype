using System.Collections.Generic;

using UnityEngine;

using EasyBuildSystem.Features.Runtime.Bases;

using EasyBuildSystem.Features.Runtime.Buildings.Part;
using EasyBuildSystem.Features.Runtime.Buildings.Socket;
using EasyBuildSystem.Features.Runtime.Buildings.Manager;

public class Demo_BuildingDragging : Singleton<Demo_BuildingDragging>
{
    [SerializeField] LayerMask m_DraggingLayer;
    [SerializeField] float m_DraggingDistance = 10;
    [SerializeField] float m_DraggingForce = 500;

    Vector3 m_OriginalScreenTargetPosition;
    Vector3 m_OriginalRigidbodyPosition;

    float m_SelectionDistance;

    Rigidbody m_DraggingObject;
    BuildingSocket m_CurrentSocket;

    List<BuildingSocket> m_PotentialSockets = new List<BuildingSocket>();

    Camera m_Camera;
    Camera Camera
    {
        get
        {
            if (m_Camera == null)
            {
                m_Camera = GetComponent<Camera>();
            }

            return m_Camera;
        }
    }

    void Update()
    {
        if (m_DraggingObject != null)
        {
            for (int i = 0; i < m_PotentialSockets.Count; i++)
            {
                Vector3 mousePositionOffset =
                    Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_SelectionDistance)) - m_OriginalScreenTargetPosition;

                if (Vector3.Distance(m_PotentialSockets[i].transform.position, (m_OriginalRigidbodyPosition + mousePositionOffset))
                    < m_PotentialSockets[i].SocketRadius)
                {
                    BuildingSocket.SnappingPointSettings snappingPoint = m_PotentialSockets[i].GetOffset(m_DraggingObject.GetComponentInParent<BuildingPart>());

                    if (m_PotentialSockets[i].Snap(m_DraggingObject.GetComponentInParent<BuildingPart>(), snappingPoint, Vector3.zero))
                    {
                        m_CurrentSocket = m_PotentialSockets[i];
                        m_DraggingObject.transform.parent = m_PotentialSockets[i].transform;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            m_DraggingObject = FindDraggableObject();

            ClearPreviewSockets();

            if (m_DraggingObject != null)
            {
                PreviewSockets();

                //m_DraggingObject.GetComponent<BuildingPart>().DisableChildrenColliders();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (m_DraggingObject)
            {
                ClearPreviewSockets();

                if (m_CurrentSocket != null)
                {
                    if (Vector3.Distance(m_DraggingObject.transform.position, m_CurrentSocket.transform.position) < m_CurrentSocket.SocketRadius)
                    {
                        BuildingSocket.SnappingPointSettings snappingPoint = m_CurrentSocket.GetOffset(m_DraggingObject.GetComponentInParent<BuildingPart>());

                        m_CurrentSocket.Snap(m_DraggingObject.GetComponentInParent<BuildingPart>(), snappingPoint, Vector3.zero);

                        m_CurrentSocket.SocketBusy = true;
                        m_DraggingObject.isKinematic = true;

                        Destroy(m_DraggingObject.GetComponent<BuildingPart>());
                        Destroy(m_DraggingObject.GetComponent<Rigidbody>());
                    }
                    else
                    {
                        m_DraggingObject.transform.parent = null;
                    }
                }

                //m_DraggingObject.GetComponent<BuildingPart>().EnableChildrenColliders();

                m_CurrentSocket = null;
                m_DraggingObject = null;
            }
        }
    }

    void FixedUpdate()
    {
        if (m_DraggingObject)
        {
            Vector3 mousePositionOffset =
                Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_SelectionDistance)) - m_OriginalScreenTargetPosition;

            m_DraggingObject.velocity = (m_OriginalRigidbodyPosition + mousePositionOffset - m_DraggingObject.transform.position) * m_DraggingForce * Time.deltaTime;
        }
    }

    Rigidbody FindDraggableObject()
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, m_DraggingDistance, m_DraggingLayer))
        {
            Rigidbody rigidbody = hitInfo.collider.gameObject.GetComponentInParent<Rigidbody>();

            if (rigidbody != null)
            {
                BuildingPart buildingPart = hitInfo.collider.GetComponentInParent<BuildingPart>();

                if (buildingPart != null)
                {
                    m_SelectionDistance = Vector3.Distance(ray.origin, hitInfo.point);
                    m_OriginalScreenTargetPosition = Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_SelectionDistance));
                    m_OriginalRigidbodyPosition = hitInfo.collider.transform.position;

                    return rigidbody;
                }
            }
        }

        return null;
    }

    void ClearPreviewSockets()
    {
        for (int i = 0; i < BuildingManager.Instance.RegisteredBuildingSockets.Count; i++)
        {
            BuildingSocket socket = BuildingManager.Instance.RegisteredBuildingSockets[i];
            socket.ClearPreview();
        }

        m_PotentialSockets.Clear();
    }

    void PreviewSockets()
    {
        for (int i = 0; i < BuildingManager.Instance.RegisteredBuildingSockets.Count; i++)
        {
            BuildingSocket socket = BuildingManager.Instance.RegisteredBuildingSockets[i];

            BuildingSocket.SnappingPointSettings snappingPoint = socket.GetOffset(m_DraggingObject.GetComponentInParent<BuildingPart>());

            if (snappingPoint != null)
            {
                if (!socket.SocketBusy)
                {
                    socket.ShowPreview(snappingPoint);

                    m_PotentialSockets.Add(socket);
                }
            }
        }
    }
}