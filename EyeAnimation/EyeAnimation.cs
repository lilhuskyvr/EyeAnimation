using System;
using System.Collections;
using System.Linq;
using ThunderRoad;
using UnityEngine;

namespace EyeAnimation
{
    public class EyeAnimation : MonoBehaviour
    {
        private Creature _creature;

        private void Awake()
        {
            _creature = GetComponent<Creature>();
            _creature.OnKillEvent += CreatureOnOnKillEvent;
        }

        private void CreatureOnOnKillEvent(CollisionInstance collisioninstance, EventTime eventtime)
        {
            if (eventtime == EventTime.OnEnd)
            {
                Debug.Log("creature killed");
                StartCoroutine(ClosingEyes(_creature));
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

        private IEnumerator ClosingEyes(Creature creature, float duration = 2)
        {
            var upperLidClosed = creature.data.gender == CreatureData.Gender.Male ? -55 : -25;
            var lowerLidClosed = creature.data.gender == CreatureData.Gender.Male ? 20 : 5;
            var leftUpperLid =
                creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.name == "LeftUpperLid_Mesh");
            var rightUpperLid =
                creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.name == "RightUpperLid_Mesh");
            var leftLowerLid =
                creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.name == "LeftLowerLid_Mesh");
            var rightLowerLid =
                creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.name == "RightLowerLid_Mesh");

            StartCoroutine(AnimateLocalRotation(leftUpperLid.Value, duration,
                Quaternion.Euler(0, upperLidClosed, 0)));
            StartCoroutine(AnimateLocalRotation(rightUpperLid.Value, duration,
                Quaternion.Euler(0, upperLidClosed, 0)));
            StartCoroutine(AnimateLocalRotation(leftLowerLid.Value, duration,
                Quaternion.Euler(0, lowerLidClosed, 0)));
            StartCoroutine(AnimateLocalRotation(rightLowerLid.Value, duration,
                Quaternion.Euler(0, lowerLidClosed, 0)));

            yield return null;
        }

        private IEnumerator OpeningEyes(Creature creature)
        {
            var leftUpperLid =
                creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.name == "LeftUpperLid_Mesh");
            var rightUpperLid =
                creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.name == "RightUpperLid_Mesh");
            var leftLowerLid =
                creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.name == "LeftLowerLid_Mesh");
            var rightLowerLid =
                creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.name == "RightLowerLid_Mesh");

            StartCoroutine(AnimateLocalRotation(leftUpperLid.Value, 5, Quaternion.Euler(0, 0, 0)));
            StartCoroutine(AnimateLocalRotation(rightUpperLid.Value, 5, Quaternion.Euler(0, 0, 0)));
            StartCoroutine(AnimateLocalRotation(leftLowerLid.Value, 5, Quaternion.Euler(0, 0, 0)));
            StartCoroutine(AnimateLocalRotation(rightLowerLid.Value, 5, Quaternion.Euler(0, 0, 0)));

            yield return null;
        }

        private void OnDestroy()
        {
            _creature.OnKillEvent -= CreatureOnOnKillEvent;
        }
    }
}