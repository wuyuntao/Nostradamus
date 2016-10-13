using BulletUnity;
using NLog;
using Nostradamus.Physics;
using UnityEngine;

namespace Nostradamus.Examples
{
    public class RigidBodyInterpolate : MonoBehaviour
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        RigidBodyActor actor;
        RigidBodySnapshot snapshot;
        Vector3 smoothVelocity;
        float remainingTime;

        public static RigidBodyInterpolate Initialize(GameObject go, RigidBodyActor actor)
        {
            var ai = go.AddComponent<RigidBodyInterpolate>();

            ai.actor = actor;

            return ai;
        }

        void Start()
        {
            snapshot = (RigidBodySnapshot)actor.Snapshot;

            transform.position = snapshot.Position.ToUnity();
            transform.rotation = snapshot.Rotation.ToUnity();
        }

        void FixedUpdate()
        {
            snapshot = (RigidBodySnapshot)actor.Snapshot;
        }

        void Update()
        {
            transform.position = Vector3.SmoothDamp(transform.position, snapshot.Position.ToUnity(), ref smoothVelocity, 0.3f);
            transform.rotation = Quaternion.Slerp(transform.rotation, snapshot.Rotation.ToUnity(), 0.3f);
        }
    }
}
