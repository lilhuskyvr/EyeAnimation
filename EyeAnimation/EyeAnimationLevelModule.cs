using System.Collections;
using IngameDebugConsole;
using ThunderRoad;
using UnityEngine;

namespace EyeAnimation
{
    public class EyeAnimationLevelModule : LevelModule
    {
        public override IEnumerator OnLoadCoroutine()
        {
            EventManager.onCreatureSpawn += EventManagerOnonCreatureSpawn;
            Debug.Log("Eye Animation");

            // DebugLogConsole.AddCommandInstance("creature_spawn_and_behead",
            //     "TestDismemberment", "TestDismemberment",
            //     this);

            return base.OnLoadCoroutine();
        }

        
        public void TestDismemberment()
        {
            GameManager.local.StartCoroutine(Catalog.GetData<CreatureData>("HumanFemale").SpawnCoroutine(
                Player.local.transform.position + Player.local.transform.forward,
                Player.local.transform.rotation,
                null,
                rsCreature =>
                {
                    GameManager.local.StartCoroutine(BeheadCreature(rsCreature));
                }
            ));
        }

        private IEnumerator BeheadCreature(Creature rsCreature)
        {
            yield return new WaitForSeconds(2);
            rsCreature.ragdoll.headPart.Slice();
            rsCreature.Kill();
        }

        private void EventManagerOnonCreatureSpawn(Creature creature)
        {
            if (!creature.isPlayer)
            {
                var eyeAnimation = creature.gameObject.GetComponent<EyeAnimation>();
                if (eyeAnimation != null)
                    Object.Destroy(eyeAnimation);

                eyeAnimation = creature.gameObject.AddComponent<EyeAnimation>();
            }
        }
    }
}