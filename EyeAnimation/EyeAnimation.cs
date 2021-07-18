using System.Collections;
using System.Linq;
using ThunderRoad;
using UnityEngine;

namespace EyeAnimation
{
    public class EyeAnimation : MonoBehaviour
    {
        private Creature _creature;
        private float _closedUpperLid;
        private float _closedLowerLid;
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
                    boneCondition.Value.name == "LeftUpperLid_Mesh").Value;
            rightUpperLid =
                _creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.name == "RightUpperLid_Mesh").Value;
            leftLowerLid =
                _creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.name == "LeftLowerLid_Mesh").Value;
            rightLowerLid =
                _creature.manikinParts.Rig.bones.FirstOrDefault(boneCondition =>
                    boneCondition.Value.name == "RightLowerLid_Mesh").Value;
            _creature.OnKillEvent += CreatureOnOnKillEvent;
            // _creature.OnDamageEvent += CreatureOnOnDamageEvent;
            // _creature.ragdoll.OnStateChange += RagdollOnOnStateChange;
        }

        private void RagdollOnOnStateChange(Ragdoll.State state)
        {
            Debug.Log("Is state" + (state == Ragdoll.State.Destabilized));
        }

        private void CreatureOnOnDamageEvent(CollisionInstance collisioninstance)
        {
            _creature.Kill();
            if (_creature.state == Creature.State.Alive)
                StartCoroutine(FlashingEyes());
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

        private IEnumerator AnimateLocalRotationBackAndForth(Transform animatedTransform, float duration,
            Quaternion targetLocalRotation)
        {
            var startLocalRotation = animatedTransform.localRotation;
            var startTime = Time.time;

            var halfDuration = duration / 2;
            while (Time.time - startTime < halfDuration)
            {
                animatedTransform.localRotation = Quaternion.Lerp(startLocalRotation, targetLocalRotation,
                    (Time.time - startTime) / halfDuration);
                yield return new WaitForFixedUpdate();
            }

            startTime = Time.time;

            while (Time.time - startTime < halfDuration)
            {
                animatedTransform.localRotation = Quaternion.Lerp(targetLocalRotation, startLocalRotation,
                    (Time.time - startTime) / halfDuration);
                yield return new WaitForFixedUpdate();
            }

            yield return null;
        }

        public IEnumerator ClosingEyes(float duration = 2)
        {
            StartCoroutine(AnimateLocalRotation(leftUpperLid, duration, Quaternion.Euler(0, _closedUpperLid, 0)));
            StartCoroutine(AnimateLocalRotation(rightUpperLid, duration, Quaternion.Euler(0, _closedUpperLid, 0)));
            StartCoroutine(AnimateLocalRotation(leftLowerLid, duration, Quaternion.Euler(0, _closedLowerLid, 0)));
            StartCoroutine(AnimateLocalRotation(rightLowerLid, duration, Quaternion.Euler(0, _closedLowerLid, 0)));

            yield return null;
        }

        public IEnumerator FlashingEyes(float duration = 0.5f)
        {
            StartCoroutine(AnimateLocalRotationBackAndForth(leftUpperLid, 5, Quaternion.Euler(0, 0, 0)));
            StartCoroutine(AnimateLocalRotationBackAndForth(rightUpperLid, 5, Quaternion.Euler(0, 0, 0)));
            StartCoroutine(AnimateLocalRotationBackAndForth(leftLowerLid, 5, Quaternion.Euler(0, 0, 0)));
            StartCoroutine(AnimateLocalRotationBackAndForth(rightLowerLid, 5, Quaternion.Euler(0, 0, 0)));

            yield return null;
        }

        private void OnDestroy()
        {
            _creature.OnKillEvent -= CreatureOnOnKillEvent;
            // _creature.OnDamageEvent -= CreatureOnOnDamageEvent;
            // _creature.ragdoll.OnStateChange -= RagdollOnOnStateChange;
        }
    }
}