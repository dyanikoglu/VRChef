namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;

    //[AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Secondary Controller Grab Actions/VRTK_AxisScaleGrabAction")]
    public class PullFromBothSides_GrabAction : VRTK_BaseGrabAction
    {
        [Tooltip("The distance the secondary grabbing object must move away from the original grab position before the secondary grabbing object auto ungrabs the Interactable Object.")]
        public float ungrabDistance = 2f;

        public bool isActionDone = false;

        /// <summary>
        /// The Initalise method is used to set up the state of the secondary action when the Interactable Object is initially grabbed by a secondary Interact Grab.
        /// </summary>
        /// <param name="currentGrabbdObject">The Interactable Object script for the object currently being grabbed by the primary grabbing object.</param>
        /// <param name="currentPrimaryGrabbingObject">The Interact Grab script for the object that is associated with the primary grabbing object.</param>
        /// <param name="currentSecondaryGrabbingObject">The Interact Grab script for the object that is associated with the secondary grabbing object.</param>
        /// <param name="primaryGrabPoint">The point on the Interactable Object where the primary Interact Grab initially grabbed the Interactable Object.</param>
        /// <param name="secondaryGrabPoint">The point on the Interactable Object where the secondary Interact Grab initially grabbed the Interactable Object.</param>
        public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
        {
            base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
        }

        /// <summary>
        /// The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary Interact Grab.
        /// </summary>

        protected virtual void Awake()
        {
            isSwappable = false;
            isActionable = true;
        }

        public override void ProcessUpdate()
        {
            base.ProcessUpdate();
            CheckForceStopDistance(ungrabDistance);
        }

        protected override void CheckForceStopDistance(float ungrabDistance)
        {
            if (initialised && Vector3.Distance(secondaryGrabbingObject.transform.position, secondaryInitialGrabPoint.position) > ungrabDistance)
            {
                grabbedObject.ForceStopSecondaryGrabInteraction();
                isActionDone = true;
            }
        }
    }
}