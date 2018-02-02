using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.SecondaryControllerGrabActions;
using VRTK.GrabAttachMechanics;

public class Egg : MonoBehaviour
{

    public GameObject insideEggPrefab;
    PullFromBothSides_GrabAction secondaryAction;
    public bool isCracked;

    void Start()
    {
        secondaryAction = GetComponent<PullFromBothSides_GrabAction>();
    }

    void Update()
    {
        if (isCracked && secondaryAction && secondaryAction.isActionDone)
        {
            GameObject upper = transform.GetChild(0).gameObject;
            GameObject lower = transform.GetChild(1).gameObject;
            upper.transform.parent = null;
            lower.transform.parent = null;

            Vector3 initializePosition = (upper.transform.position + lower.transform.position) / 2;

            upper.AddComponent<BoxCollider>();
            lower.AddComponent<BoxCollider>();

            VRTK_InteractableObject upperInteractable = upper.AddComponent<VRTK_InteractableObject>();
            VRTK_InteractableObject lowerInteractable = lower.AddComponent<VRTK_InteractableObject>();

            upperInteractable.isGrabbable = true;
            lowerInteractable.isGrabbable = true;

            GameObject rightController = GameObject.Find("RightController");
            AutoGrab autoGrabRight = rightController.AddComponent<AutoGrab>();

            autoGrabRight.objectToGrab = upperInteractable;
            autoGrabRight.interactTouch = autoGrabRight.GetComponent<VRTK_InteractTouch>();
            autoGrabRight.interactGrab = autoGrabRight.GetComponent<VRTK_InteractGrab>();

            GameObject leftController = GameObject.Find("LeftController");
            AutoGrab autoGrabLeft = leftController.AddComponent<AutoGrab>();

            autoGrabLeft.objectToGrab = lowerInteractable;
            autoGrabLeft.interactTouch = autoGrabLeft.GetComponent<VRTK_InteractTouch>();
            autoGrabLeft.interactGrab = autoGrabLeft.GetComponent<VRTK_InteractGrab>();

            GameObject insideEgg = Instantiate(insideEggPrefab, initializePosition, insideEggPrefab.transform.rotation);
            Rigidbody rigidBody = insideEgg.AddComponent<Rigidbody>();
            rigidBody.mass = 0.2f;
            rigidBody.useGravity = true;
            rigidBody.isKinematic = false;
            insideEgg.AddComponent<BoxCollider>();
            rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            gameObject.SetActive(false);

        }
    }

}
