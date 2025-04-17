using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.JomonKaenGazeData
{
    /// <summary>
    /// Game object will face user.
    /// </summary>
    [AddComponentMenu("Scripts/Utils/FaceUser")]
   public class FaceUser : MonoBehaviour
   {
        #region Serialized variables
        [SerializeField]
        private float Speed = 2f;

        [SerializeField]
        private float RotationTreshInDegrees = 3f;
        #endregion

        private GameObject targetToRotate = null;
        private GameObject objectWithCollider = null;
        private bool finished_returningToOrig = true;
        private bool finished_facingUser = false;
        private Vector3 origForwardNormalized = Vector3.zero;
        private bool turnToUser = false;

        private void Start()
        {
            if (targetToRotate == null)
            {
                targetToRotate = gameObject;
            }

            if (objectWithCollider == null)
            {
                Collider coll;
                coll = GetComponent<Collider>();
                if (coll == null)
                {
                    coll = GetComponentInChildren<Collider>();
                }

                if (coll != null)
                {
                    objectWithCollider = GetComponentInChildren<Collider>().gameObject;
                }

                origForwardNormalized = targetToRotate.transform.forward.normalized;
            }
        }

        public void Update()
        {
            Vector3 TargetToCam = (CameraCache.Main.transform.position - targetToRotate.transform.position).normalized;
            Vector3 TargetForw = -targetToRotate.transform.forward.normalized;

            if (turnToUser && (!finished_facingUser))
            {
                TurnToUser(TargetToCam, TargetForw);
            }
            else if ((!turnToUser) && (!finished_returningToOrig))
            {
                ReturnToOriginalRotation(TargetToCam, TargetForw);
            }
        }

        private void TurnToUser(Vector3 TargetToCam, Vector3 TargetForw)
        {
            if (Mathf.Abs(Vector3.Angle(TargetForw, TargetToCam)) < RotationTreshInDegrees)
            {
                finished_facingUser = true;
                return;
            }

            Quaternion rotateTowardsCamera = Quaternion.LookRotation(targetToRotate.transform.position - CameraCache.Main.transform.position);
            targetToRotate.transform.rotation = Quaternion.Slerp(targetToRotate.transform.rotation, rotateTowardsCamera, Speed *  Time.deltaTime);

            targetToRotate.transform.localScale = targetToRotate.transform.localScale;

            finished_returningToOrig = false;
        }

        private void ReturnToOriginalRotation(Vector3 TargetToCam, Vector3 TargetForw)
        {
            if (Mathf.Abs(Vector3.Angle(TargetForw, origForwardNormalized)-180) < RotationTreshInDegrees)
            {
                finished_returningToOrig = true;
                return;
            }

            Quaternion rotateBackToDefault = Quaternion.LookRotation(origForwardNormalized);
            targetToRotate.transform.rotation = Quaternion.Slerp(targetToRotate.transform.rotation, rotateBackToDefault, Speed * Time.deltaTime);
            finished_facingUser = false;
        }

        public void Engage()
        {
            turnToUser = true;
        }

        public void Disengage()
        {
            turnToUser = false;
        }
   }
}
