using UnityEngine;

namespace Nostradamus.Examples
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField]
        public Transform followTarget;

        [SerializeField]
        Vector3 cameraOffset = new Vector3(0, 1, -5);

        [SerializeField]
        float smoothTime = 1f;

        Vector3 smoothVelocity;

        void LateUpdate()
        {
            if (followTarget == null)
                return;

            var targetPosition = followTarget.position + cameraOffset;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothVelocity, smoothTime);
        }
    }
}