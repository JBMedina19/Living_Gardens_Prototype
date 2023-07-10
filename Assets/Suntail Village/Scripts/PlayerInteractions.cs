﻿using UnityEngine;
using UnityEngine.UI;

//Interacting with objects and doors
namespace Suntail
{
    public class PlayerInteractions : MonoBehaviour
    {
        [Header("Interaction variables")]
        [Tooltip("Layer mask for interactive objects")]
        [SerializeField] private LayerMask interactionLayer;
        [Tooltip("Maximum distance from player to object of interaction")]
        [SerializeField] private float interactionDistance = 3f;
        [Tooltip("Tag for door object")]
        [SerializeField] private string doorTag = "Door";
        [Tooltip("Tag for pickable object")]
        [SerializeField] private string itemTag = "Item";
        [Tooltip("The player's main camera")]
        [SerializeField] private string petTag = "Pet";
        [Tooltip("The player's main camera")]
        [SerializeField] private Camera mainCamera;
        [Tooltip("Parent object where the object to be lifted becomes")]
        [SerializeField] private Transform pickupParent;

        [Header("Keybinds")]
        [Tooltip("Interaction key")]
        [SerializeField] private KeyCode interactionKey = KeyCode.E;

        [Header("Object Following")]
        [Tooltip("Minimum speed of the lifted object")]
        [SerializeField] private float minSpeed = 0;
        [Tooltip("Maximum speed of the lifted object")]
        [SerializeField] private float maxSpeed = 3000f;

        [Header("UI")]
        [Tooltip("Background object for text")]
        [SerializeField] private Image uiPanel;
        [Tooltip("Background object for text")]
        [SerializeField] private Image petPanel;
        [Tooltip("Text holder")]
        [SerializeField] private Text panelText;
        [Tooltip("PetStats holder")]
        [SerializeField] private Text levelText;
        [SerializeField] private Text healthText;
        [SerializeField] private Text energyText;
        [SerializeField] private Text hapinnessText;
        [SerializeField] private Text loyaltyText;
        [SerializeField] private Text agilityText;
        [SerializeField] private Text hungerText;
        [SerializeField] private Text personalityText;
        [SerializeField] private Text expText;
        [Tooltip("Text when an object can be lifted")]
        [SerializeField] private string itemPickUpText;
        [Tooltip("Text when an object can be drop")]
        [SerializeField] private string itemDropText;
        [Tooltip("Text when the door can be opened")]
        [SerializeField] private string doorOpenText;
        [Tooltip("Text when the door can be closed")]
        [SerializeField] private string doorCloseText;
        [SerializeField] private string petText;

        //Private variables.
        private PhysicsObject _physicsObject;
        private PhysicsObject _currentlyPickedUpObject;
        private PhysicsObject _lookObject;
        private PetManager _lookPet;
        private Quaternion _lookRotation;
        private Vector3 _raycastPosition;
        private Rigidbody _pickupRigidBody;
        private Door _lookDoor;
        private float _currentSpeed = 0f;
        private float _currentDistance = 0f;
        private CharacterController _characterController;


        private void Start()
        {
            mainCamera = Camera.main;
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Interactions();
            LegCheck();
        }

        //Determine which object we are now looking at, depending on the tag and component
        private void Interactions()
        {
            _raycastPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit interactionHit;
            if (Physics.Raycast(_raycastPosition, mainCamera.transform.forward, 
                out interactionHit, interactionDistance, interactionLayer))
            {
                if (interactionHit.collider.CompareTag(itemTag))
                {
                    _lookObject = interactionHit.collider.GetComponentInChildren<PhysicsObject>();
                    ShowItemUI();
                }
                else if (interactionHit.collider.CompareTag(doorTag))
                {
                    _lookDoor = interactionHit.collider.gameObject.GetComponentInChildren<Door>();
                    ShowDoorUI();
                    if (Input.GetKeyDown(interactionKey))
                    {
                        _lookDoor.PlayDoorAnimation();
                    }
                }
                else if (interactionHit.collider.CompareTag(petTag))
                {
                    _lookPet = interactionHit.collider.gameObject.GetComponent<PetManager>();
                    string level, health, energy, hapinness, loyalty, agility, hunger;
                    health = _lookPet.health.ToString();
                    level = _lookPet.level.ToString();
                    energy = _lookPet.energy.ToString();
                    hapinness = _lookPet.happiness.ToString();
                    loyalty = _lookPet.Loyalty.ToString();
                    agility = _lookPet.Agility.ToString();
                    hunger = _lookPet.hunger.ToString();
                    levelText.text = level;
                    healthText.text = health;
                    energyText.text = energy;
                    hapinnessText.text = hapinness;
                    loyaltyText.text = loyalty;
                    agilityText.text = agility;
                    hungerText.text = hunger;
                    expText.text = _lookPet.exp.ToString();
                    personalityText.text = _lookPet.personalityType;
                    ShowPetUI();
                    if (Input.GetKeyDown(interactionKey))
                    {
                        _lookPet.Interaction();
                    }

                }
            }
            else
            {
                _lookDoor = null;
                _lookObject = null;
                _lookPet = null;
                uiPanel.gameObject.SetActive(false);
                petPanel.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(interactionKey))
            {
                if (_currentlyPickedUpObject == null)
                {
                    if (_lookObject != null)
                    {
                        PickUpObject();
                    }
                }
                else
                {
                    BreakConnection();
                }
            }
        }

        //Disconnects from the object when the player attempts to step on the object, prevents flight on the object
        private void LegCheck()
        {
            Vector3 spherePosition = _characterController.center + transform.position;
            RaycastHit legCheck;
            if (Physics.SphereCast(spherePosition, 0.3f, Vector3.down, out legCheck, 2.0f))
            {
                if (legCheck.collider.CompareTag(itemTag))
                {
                    BreakConnection();
                }
            }
        }

        //Velocity movement toward pickup parent
        private void FixedUpdate()
        {
            if (_currentlyPickedUpObject != null)
            {
                _currentDistance = Vector3.Distance(pickupParent.position, _pickupRigidBody.position);
                _currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, _currentDistance / interactionDistance);
                _currentSpeed *= Time.fixedDeltaTime;
                Vector3 direction = pickupParent.position - _pickupRigidBody.position;
                _pickupRigidBody.velocity = direction.normalized * _currentSpeed;
            }
        }

        //Picking up an looking object
        public void PickUpObject()
        {

            _physicsObject = _lookObject.GetComponentInChildren<PhysicsObject>();
            _physicsObject.transform.parent.gameObject.SetActive(false);
            _physicsObject.transform.SetParent(null);
            _currentlyPickedUpObject = _lookObject;
            _lookRotation = _currentlyPickedUpObject.transform.rotation;
            _pickupRigidBody = _currentlyPickedUpObject.GetComponent<Rigidbody>();
            _pickupRigidBody.useGravity = true;
            _pickupRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            _pickupRigidBody.transform.rotation = _lookRotation;
            _physicsObject.playerInteraction = this;
            StartCoroutine(_physicsObject.PickUp());
        }

        //Release the object
        public void BreakConnection()
        {
            if (_currentlyPickedUpObject)
            {
                _pickupRigidBody.constraints = RigidbodyConstraints.None;
                _currentlyPickedUpObject = null;
                _physicsObject.pickedUp = false;
                _currentDistance = 0;
                if (_physicsObject.isFruit == true)
                {
                    _physicsObject.transform.gameObject.tag = "Fruit";
                }
            }
        }

        //Show interface elements when hovering over an object
        private void ShowDoorUI()
        {
            uiPanel.gameObject.SetActive(true);

            if (_lookDoor.doorOpen)
            {
                panelText.text = doorCloseText;
            }
            else
            {
                panelText.text = doorOpenText;
            }
        }

        private void ShowItemUI()
        {
            uiPanel.gameObject.SetActive(true);

            if (_currentlyPickedUpObject == null)
            {
                panelText.text = itemPickUpText;
            }
            else if (_currentlyPickedUpObject != null)
            {
                panelText.text = itemDropText;
            }

        }

        private void ShowPetUI()
        {
            uiPanel.gameObject.SetActive(true);
            petPanel.gameObject.SetActive(true);
            panelText.text = petText;
            
        }

    }
}