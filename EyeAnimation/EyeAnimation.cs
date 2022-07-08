using System.Collections;
using System.Linq;
using System.Reflection;
using RainyReignGames.MeshDismemberment;
using ThunderRoad;
using UnityEngine;

namespace EyeAnimation
{
    public class EyeAnimation : MonoBehaviour
    {
        private Creature _creature;
        private float _closedUpperLid;
        private float _closedLowerLid;
        private Dismemberment _dismemberment;
        public Transform leftUpperLid;
        public Transform rightUpperLid;
        public Transform leftLowerLid;
        public Transform rightLowerLid;

        private void Awake()
        {
            _creature = GetComponent<Creature>();
            _closedUpperLid = _creature.data.gender == CreatureData.Gender.Male ? -55 : -25;
            _closedLowerLid = _creature.data.gender == CreatureData.Gender.Male ? 20 : 5;

            leftUpperLid =
                _creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.boneTransform.name == "LeftUpperLid_Mesh").Value.boneTransform;
            rightUpperLid =
                _creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.boneTransform.name == "RightUpperLid_Mesh").Value.boneTransform;
            leftLowerLid =
                _creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.boneTransform.name == "LeftLowerLid_Mesh").Value.boneTransform;
            rightLowerLid =
                _creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.boneTransform.name == "RightLowerLid_Mesh").Value.boneTransform;
            _creature.OnKillEvent += CreatureOnOnKillEvent;

            var creatureRagdoll = _creature.ragdoll;

            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                     | BindingFlags.Static;
            _dismemberment =
                creatureRagdoll.GetType().GetField("dismemberment", bindFlags).GetValue(creatureRagdoll) as
                    Dismemberment;

            _dismemberment.Completed += DismembermentOnCompleted;
        }

        private void DismembermentOnCompleted(object sender, Dismemberment.CompletedEventArgs e)
        {
            var severedMesh = e.splitGameObject;

            foreach (var skinnedMeshRenderer in severedMesh.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (skinnedMeshRenderer.name.Contains("Eyes_"))
                {
                    foreach (var bone in skinnedMeshRenderer.bones)
                    {
                        if (bone.name.Contains("UpperLid"))
                        {
                            StartCoroutine(AnimateLocalRotation(bone, 5, Quaternion.Euler(0, _closedUpperLid, 0)));
                        }
                        if (bone.name.Contains("LowerLid"))
                        {
                            StartCoroutine(AnimateLocalRotation(bone, 5, Quaternion.Euler(0, _closedLowerLid, 0)));
                        }
                    }
                }
            }
        }

        private void CreatureOnOnKillEvent(CollisionInstance collisioninstance, EventTime eventtime)
        {
            if (eventtime == EventTime.OnEnd)
            {
                StartCoroutine(ClosingEyes());
            }
        }

        private IEnumerator AnimateLocalRotation(Transform animatedTransform, float duration,
            Quaternion targetLocalRotation)
        {
            var startLocalRotation = animatedTransform.localRotation;
            var startTime = Time.time;

            while (Time.time - startTime < duration)
            {
                animatedTransform.localRotation = Quaternion.Lerp(startLocalRotation, targetLocalRotation,
                    (Time.time - startTime) / duration);
                yield return new WaitForFixedUpdate();
            }

            yield return null;
        }

        public IEnumerator ClosingEyes(float duration = 3)
        {
            StartCoroutine(AnimateLocalRotation(leftUpperLid, duration, Quaternion.Euler(0, _closedUpperLid, 0)));
            StartCoroutine(AnimateLocalRotation(rightUpperLid, duration, Quaternion.Euler(0, _closedUpperLid, 0)));
            StartCoroutine(AnimateLocalRotation(leftLowerLid, duration, Quaternion.Euler(0, _closedLowerLid, 0)));
            StartCoroutine(AnimateLocalRotation(rightLowerLid, duration, Quaternion.Euler(0, _closedLowerLid, 0)));

            yield return null;
        }

        private void OnDestroy()
        {
            _creature.OnKillEvent -= CreatureOnOnKillEvent;
            _dismemberment.Completed -= DismembermentOnCompleted;
        }
    }
}