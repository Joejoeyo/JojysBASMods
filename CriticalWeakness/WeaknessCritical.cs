using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace CriticalWeakness
{
    public class WeaknessCritical : ThunderScript
    {
        internal class modOptions
        {
            [ModOption(name = "Enable Unstable Multiplier", tooltip = "Enabled: If the enemy is not standing up, adds bonus damage based on multiplier below", defaultValueIndex = 1, order = 1)]
            public static bool RagdollMultEnabled;
            [ModOptionSlider]
            [ModOption(name = "Unstable Damage Multiplier", defaultValueIndex = 20, valueSourceName = nameof(MultiplierArray), order = 2)]
            public static float RagdollMult;
            [ModOption(name = "Enable Unaware Multiplier", tooltip = "Enabled: If the enemy not alert or in combat, adds bonus damage based on multiplier below", defaultValueIndex = 1, order = 3)]
            public static bool StealthMultEnabled;
            [ModOptionSlider]
            [ModOption(name = "Unaware Damage Multiplier", defaultValueIndex = 20, valueSourceName = nameof(MultiplierArray), order = 4)]
            public static float StealthMult;
            [ModOption(name = "Stack Multipliers", tooltip = "Stacks multipliers if applicable, otherwise uses higher multiplier", defaultValueIndex = 1, order = 5)]
            public static bool StackEnabled;

            public static ModOptionFloat[] MultiplierArray()
            {
                List<ModOptionFloat> options = new List<ModOptionFloat>();
                float val = 0.0f;
                while (val <= 10.0f)
                {
                    options.Add(new ModOptionFloat(val.ToString("0.0"), val));
                    val += 0.1f;
                }
                return options.ToArray();
            }
        }


        public static float initialDamage = 0;
        private class CriticalWeaknessCode : ThunderScript
        {
            public override void ScriptEnable()
            {
                base.ScriptEnable();
                EventManager.onCreatureHit += EventManager_onCreatureHit;
            }

            private void EventManager_onCreatureHit(Creature creature, CollisionInstance collisionInstance, EventTime eventTime)
            {
                if (creature.isPlayer) { return; }
                if (creature.isKilled) { return; }
                if (creature == null) { return; }
                //non player damage doesn't count?
                if (collisionInstance.targetColliderGroup == collisionInstance.sourceColliderGroup) { return; }
                //Debug.Log($"sourceColliderGroup = {collisionInstance.sourceColliderGroup}, sourceCollider = {collisionInstance.sourceCollider}, targetColliderGroup = {collisionInstance.targetColliderGroup}, targetCollider = {collisionInstance.targetCollider}");
                bool creatureStanding = false;
                bool creatureUnaware = false;
                initialDamage = collisionInstance.damageStruct.damage;
                if (modOptions.StealthMultEnabled)
                {
                    if (creature.brain.state != Brain.State.Combat && creature.brain.state != Brain.State.Alert && creature.brain.state != Brain.State.Investigate)
                    {
                        creatureUnaware = true;
                        //Debug.Log($"Stealth Multiplier enabled, initial damage = {initialDamage}");
                        collisionInstance.damageStruct.damage = initialDamage * modOptions.StealthMult;
                        //Debug.Log($"Stealth Multiplier enabled, modified damage = {collisionInstance.damageStruct.damage}");
                    }
                    else
                    {
                        creatureUnaware = false;
                        //Debug.Log($"Creature brainstate is {creature.brain.state}, not combat nor alert");
                    }

                }
                if (modOptions.RagdollMultEnabled)
                {
                    Debug.Log($"Current creature state: {creature.state}, current creature grounded: {creature.locomotion.isGrounded}");
                    if (creature.state == Creature.State.Destabilized) { creatureStanding = true; }
                    else
                    {
                        creatureStanding = false;
                        //Debug.Log($"Ragdoll Multiplier enabled, initial damage = {initialDamage}");
                        collisionInstance.damageStruct.damage = initialDamage * modOptions.RagdollMult;
                        //Debug.Log($"Ragdoll Multiplier enabled, modified damage = {collisionInstance.damageStruct.damage}");
                    }
                    if (modOptions.StackEnabled && modOptions.StealthMultEnabled && creatureUnaware && !creatureStanding)
                    {
                        //Debug.Log("Stacking Damage");
                        collisionInstance.damageStruct.damage = initialDamage * modOptions.StealthMult;
                        collisionInstance.damageStruct.damage = collisionInstance.damageStruct.damage * modOptions.RagdollMult;
                    }
                    if (!modOptions.StackEnabled && modOptions.StealthMultEnabled && creatureUnaware && !creatureStanding)
                    {
                        //Debug.Log("stack disabled, getting higher mult");
                        if (modOptions.StealthMult > modOptions.RagdollMult) { collisionInstance.damageStruct.damage = initialDamage * modOptions.StealthMult; }
                        if (modOptions.StealthMult < modOptions.RagdollMult) { collisionInstance.damageStruct.damage = initialDamage * modOptions.RagdollMult; }
                        if (modOptions.StealthMult == modOptions.RagdollMult) { collisionInstance.damageStruct.damage = initialDamage * modOptions.StealthMult; }
                    }
                }
                Debug.Log($"Damage Modification complete, new damage: {collisionInstance.damageStruct.damage}, initial damage: {initialDamage}, stealth: {creatureUnaware}, standing: {creatureStanding}");
            }
        }
    }
}
