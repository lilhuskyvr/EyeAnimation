using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace EyeAnimation
{
    public class EyeAnimationLevelModule : LevelModule
    {
        public override IEnumerator OnLoadCoroutine(Level level)
        {
            EventManager.onCreatureSpawn += EventManagerOnonCreatureSpawn;
            Debug.Log("Eye Animation");
            return base.OnLoadCoroutine(level);
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