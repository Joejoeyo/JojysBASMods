using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using static ThunderRoad.DamageModifierData.Modifier;

namespace LessForcefulPunches
{
    public class LessPuncheForce : ThunderScript
    {
        internal class modOptions
        {
            [ModOption(name = "Enable Punch Modifications", tooltip = "Enabled: uses the properties below to change punches \n Disabled: Reset to vanilla punches \n All changes require a level reload!", defaultValueIndex = 1, order = 1)]
            public static bool PunchModEnabled;
            [ModOptionIntValues(0, 1000, 1)]
            [ModOptionSlider]
            [ModOption(name = "Punch AddForce", tooltip = "The amount that an enemy gets pushed back when they get ragdolled, and only then. Does NOT change staggers at all. \n Vanilla: 100", defaultValueIndex = 30, order = 2)]
            public static int AddForceValue;
            [ModOption(name = "Punch AddForceMode", tooltip = "Vanilla: Acceleration", defaultValueIndex = 0, valueSourceName = nameof(AddForceTypeStrings), order = 3)]
            public static string AddForceModeValue;
            [ModOptionFloatValues(0.0f, 10.0f, 0.1f)]
            [ModOptionSlider]
            [ModOption(name = "Punch Damage Multiplier", tooltip = "Multiplies the damage dealt from punches. \n Vanilla: 1.0", defaultValueIndex = 10, order = 4)]
            public static float DamageMultValue;
            [ModOptionFloatValues(0.0f, 5.0f, 0.1f)]
            [ModOptionSlider]
            [ModOption(name = "Min HitVelocity Multiplier", tooltip = "Multiplies the minumum velocity for each PushLevel. In other words, higher = more force needed to do powerful punches.\n Vanilla: 1.0", defaultValueIndex = 20, order = 5)]
            public static float HitVelocityMultValue;


            [ModOption(name = "Enable Kick Modifications", tooltip = "Enabled: uses the properties below to change kicks \n Disabled: Reset to vanilla kicks \n All changes require a level reload!", defaultValueIndex = 1, order = 6)]
            public static bool KickModEnabled;
            [ModOptionIntValues(0, 1000, 1)]
            [ModOptionSlider]
            [ModOption(name = "Kick AddForce", tooltip = "The amount that an enemy gets pushed back when they get ragdolled, and only then. Does NOT change staggers at all. \n Vanilla: 400, FBT kicks will have 1/4 this value", defaultValueIndex = 200, order = 7)]
            public static int AddForceValueKick;
            [ModOption(name = "Kick AddForceMode", tooltip = "Vanilla: Acceleration", defaultValueIndex = 0, valueSourceName = nameof(AddForceTypeStrings), order = 8)]
            public static string AddForceModeValueKick;
            [ModOptionFloatValues(0.0f, 10.0f, 0.1f)]
            [ModOptionSlider]
            [ModOption(name = "Kick Damage Multiplier", tooltip = "Multiplies the damage dealt from kicks. \n Vanilla: 1.0", defaultValueIndex = 10, order = 9)]
            public static float DamageMultValueKick;
            [ModOptionFloatValues(0.0f, 5.0f, 0.1f)]
            [ModOptionSlider]
            [ModOption(name = "Kick Min HitVelocity Multiplier", tooltip = "Multiplies the minumum velocity for each PushLevel. In other words, higher = more force needed to do powerful kicks.\n Vanilla: 1.0", defaultValueIndex = 20, order = 10)]
            public static float HitVelocityMultValueKick;

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
            public static ModOptionString[] AddForceTypeStrings = {
                new ModOptionString("Impulse", "Impulse"),
                new ModOptionString("Acceleration", "Acceleration"),
                new ModOptionString("Force","Force")
                };
        }


        private class CriticalWeaknessCode : ThunderScript
        {
            public override void ScriptEnable()
            {
                base.ScriptEnable();
                EventManager.onLevelLoad += EventManager_onLevelLoad;
            }
            public static DamagerData realPunch;
            public static DamageModifierData realPunchMod;
            public static DamagerData defaultPunch;
            public static DamageModifierData defaultPunchMod;

            public static DamagerData realKick;
            public static DamagerData realKickTracked;
            public static DamageModifierData realKickMod;
            public static DamagerData defaultKick;
            public static DamageModifierData defaultKickMod;


            private void EventManager_onLevelLoad(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
            {
                //Reset Punch Damager
                realPunch = Catalog.GetData<DamagerData>("Punch");
                realPunchMod = Catalog.GetData<DamageModifierData>("Punch");
                defaultPunch = Catalog.GetData<DamagerData>("savedPunch");
                defaultPunchMod = Catalog.GetData<DamageModifierData>("savedPunch");

                //defaultPunch.id = "Punch";
                //defaultPunchMod.id = "Punch";

                //realPunch = defaultPunch;
                //realPunchMod = defaultPunchMod;

                //do stuff!!! but first check if enabled
                if (modOptions.PunchModEnabled)
                {
                    Debug.Log("Punches Enabled yay");
                    //Debug.Log($"previous addforce: {realPunch.addForce}");
                    realPunch.addForce = modOptions.AddForceValue;
                    //Debug.Log($"new addforce: {realPunch.addForce}");
                    //force modes because its stupid
                    if (modOptions.AddForceModeValue == "Impulse") { realPunch.addForceMode = ForceMode.Impulse; }
                    if (modOptions.AddForceModeValue == "Acceleration") { realPunch.addForceMode = ForceMode.Acceleration; }
                    if (modOptions.AddForceModeValue == "Force") { realPunch.addForceMode = ForceMode.Force; }
                    //Debug.Log($"realPunch addforcemode: {realPunch.addForceMode}");

                    //damage modifier stuff
                    for (int i = 0; i < realPunchMod.collisions.Count; i++)
                    {
                        for (int j = 0; j < realPunchMod.collisions[i].modifiers.Count; j++)
                        {
                            //Debug.Log($"previous damage multiplier: {realPunchMod.collisions[i].modifiers[j].damageMultiplier}");
                            realPunchMod.collisions[i].modifiers[j].damageMultiplier = defaultPunchMod.collisions[i].modifiers[j].damageMultiplier * modOptions.DamageMultValue;
                            //Debug.Log($"new damage multiplier: {realPunchMod.collisions[i].modifiers[j].damageMultiplier}");
                            for (int k = 0; k < realPunchMod.collisions[i].modifiers[j].pushLevels.Length; k++)
                            {
                                //Debug.Log($"previous hitVelocity: {realPunchMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity}");
                                realPunchMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity = defaultPunchMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity * modOptions.HitVelocityMultValue;
                                //Debug.Log($"new hitVelocity: {realPunchMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity}");
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("Punches Disabled Wild");
                    realPunch.addForce = defaultPunch.addForce;
                    realPunch.addForceMode = ForceMode.Impulse;
                    for (int i = 0; i < realPunchMod.collisions.Count; i++)
                    {
                        for (int j = 0; j < realPunchMod.collisions[i].modifiers.Count; j++)
                        {
                            //Debug.Log($"previous damage multiplier: {realPunchMod.collisions[i].modifiers[j].damageMultiplier}");
                            realPunchMod.collisions[i].modifiers[j].damageMultiplier = defaultPunchMod.collisions[i].modifiers[j].damageMultiplier;
                            //Debug.Log($"new damage multiplier: {realPunchMod.collisions[i].modifiers[j].damageMultiplier}");
                            for (int k = 0; k < realPunchMod.collisions[i].modifiers[j].pushLevels.Length; k++)
                            {
                                //Debug.Log($"previous hitVelocity: {realPunchMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity}");
                                realPunchMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity = defaultPunchMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity;
                                //Debug.Log($"new hitVelocity: {realPunchMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity}");
                            }
                        }
                    }
                }
                if (modOptions.KickModEnabled)
                {
                    Debug.Log("Less Forceful Kicks Disabled");
                    //Reset Kick Damager
                    realKick = Catalog.GetData<DamagerData>("Kick");
                    realKickTracked = Catalog.GetData<DamagerData>("KickTracked");
                    realKickMod = Catalog.GetData<DamageModifierData>("Kick");
                    defaultKick = Catalog.GetData<DamagerData>("savedKick");
                    defaultKickMod = Catalog.GetData<DamageModifierData>("savedKick");

                    //defaultKick.id = "Kick";
                    //defaultKickMod.id = "Kick";

                    //realKick = defaultKick;
                    //realKickMod = defaultKickMod;

                    //do stuff!!! but first check if enabled
                    if (modOptions.KickModEnabled)
                    {
                        Debug.Log("kicks Enabled yay");
                        //Debug.Log($"previous addforce: {realKick.addForce}");
                        realKick.addForce = modOptions.AddForceValueKick;
                        //Debug.Log($"new addforce: {realKick.addForce}");
                        //force modes because its stupid
                        if (modOptions.AddForceModeValueKick == "Impulse") { realKick.addForceMode = ForceMode.Impulse; }
                        if (modOptions.AddForceModeValueKick == "Acceleration") { realKick.addForceMode = ForceMode.Acceleration; }
                        if (modOptions.AddForceModeValueKick == "Force") { realKick.addForceMode = ForceMode.Force; }
                        //Debug.Log($"realKick addforcemode: {realKick.addForceMode}");

                        for (int i = 0; i < realKickMod.collisions.Count; i++)
                        {
                            for (int j = 0; j < realKickMod.collisions[i].modifiers.Count; j++)
                            {
                                //Debug.Log($"previous damage multiplier: {realKickMod.collisions[i].modifiers[j].damageMultiplier}");
                                realKickMod.collisions[i].modifiers[j].damageMultiplier = defaultKickMod.collisions[i].modifiers[j].damageMultiplier * modOptions.DamageMultValueKick;
                                //Debug.Log($"new damage multiplier: {realKickMod.collisions[i].modifiers[j].damageMultiplier}");
                                for (int k = 0; k < realKickMod.collisions[i].modifiers[j].pushLevels.Length; k++)
                                {
                                    //Debug.Log($"previous hitVelocity: {realKickMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity}");
                                    realKickMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity = defaultKickMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity * modOptions.HitVelocityMultValueKick;
                                    //Debug.Log($"new hitVelocity: {realKickMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity}");
                                }
                            }
                        }
                    }
                    else {
                        Debug.Log("Less Forceful Kicks Disabled");
                        //Debug.Log($"previous addforce: {realKick.addForce}");
                        realKick.addForce = defaultKick.addForce;
                        //Debug.Log($"new addforce: {realKick.addForce}");
                        //force modes because its stupid
                        realKick.addForceMode = ForceMode.Impulse;
                        //Debug.Log($"realKick addforcemode: {realKick.addForceMode}");

                        for (int i = 0; i < realKickMod.collisions.Count; i++)
                        {
                            for (int j = 0; j < realKickMod.collisions[i].modifiers.Count; j++)
                            {
                                //Debug.Log($"previous damage multiplier: {realKickMod.collisions[i].modifiers[j].damageMultiplier}");
                                realKickMod.collisions[i].modifiers[j].damageMultiplier = defaultKickMod.collisions[i].modifiers[j].damageMultiplier;
                                //Debug.Log($"new damage multiplier: {realKickMod.collisions[i].modifiers[j].damageMultiplier}");
                                for (int k = 0; k < realKickMod.collisions[i].modifiers[j].pushLevels.Length; k++)
                                {
                                    //Debug.Log($"previous hitVelocity: {realKickMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity}");
                                    realKickMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity = defaultKickMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity;
                                    //Debug.Log($"new hitVelocity: {realKickMod.collisions[i].modifiers[j].pushLevels[k].hitVelocity}");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
