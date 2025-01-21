using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ThunderRoad;
using ThunderRoad.AI.Action;
using UnityEngine;
//using static ThunderRoad.TextData;
using Item = ThunderRoad.Item;

namespace BaSSharpStab2
{
    public class ModifyPierceDamper : ThunderScript
    {
        public override void ScriptEnable()
        {
            base.ScriptEnable();
            EventManager.onItemSpawn += EventManager_onItemSpawn;
        }

        private void EventManager_onItemSpawn(Item item)
        {
            //make sure item is a weapon
            if (item.data.type != ItemData.Type.Weapon)
            {
                return;
            }
            Debug.Log("ButterStabs: Weapon Found");
           
            item.OnGrabEvent += Item_OnGrabEvent;
            
        }

        private void Item_OnGrabEvent(Handle handle, RagdollHand ragdollHand)
        {
            if (modOptions.Enabled != true) { return; }
            Debug.Log("ButterStabs: Weapon Grabbed");
            // get every damager
            foreach (var damager in handle.item.data.damagers)
            {
                    Debug.Log("ButterStabs: Damager found");
                //change main damper value according to modOption
                   damager.damagerData.penetrationDamper = damager.damagerData.penetrationDamper * modOptions.DamperMult;
                   if (modOptions.separateMults == true)
                   {
                        damager.damagerData.penetrationHeldDamperIn = damager.damagerData.penetrationHeldDamperIn * modOptions.DamperInMult;
                        damager.damagerData.penetrationHeldDamperOut = damager.damagerData.penetrationHeldDamperOut * modOptions.DamperOutMult;
                   }
            }
            {

            }
            
  //          System.Collections.IList damagerList = handle.item.data.damagers;
 //           for (int i = 0; i < damagerList.Count; i++)
  //          {
  //              Damager damager = (Damager)damagerList[i];
  //              if (damager.type == Damager.Type.Pierce)
  //              {
  //                  Debug.Log("ButterStabs: Pierce found");
   //                 damager.data.penetrationDamper = damager.data.penetrationDamper * modOptions.DamperMult;
   //                 if (modOptions.separateMults == true)
   //                 {
   //                     damager.data.penetrationHeldDamperIn = damager.data.penetrationHeldDamperIn * modOptions.DamperInMult;
   //                     damager.data.penetrationHeldDamperOut = damager.data.penetrationHeldDamperOut * modOptions.DamperOutMult;
   //                 }
   //             }

   //         }
        }

        //private void EventManager_onItemEquip(Item item)
        //{
        //    while (modOptions.Enabled==true)
        //    {// Make Sure item is a weapon
         //       if (item.data.type != ItemData.Type.Weapon) {
         //           Debug.Log("ButterStabs Weapon Equipped");
        //            return; }
                // find pierce damager
        //        System.Collections.IList damagerList = item.data.damagers;
        //        for (int i = 0; i < damagerList.Count; i++)
        //        {
        //            Damager damager = (Damager)damagerList[i];
       //             if (damager.type == Damager.Type.Pierce)
       //             {
        //                Debug.Log("ButterStabs Pierce found");
        //                damager.data.penetrationDamper = damager.data.penetrationDamper * modOptions.DamperMult;
        //                if (modOptions.separateMults == true)
       //                 {
        //                    damager.data.penetrationHeldDamperIn = damager.data.penetrationHeldDamperIn * modOptions.DamperInMult;
        //                    damager.data.penetrationHeldDamperOut = damager.data.penetrationHeldDamperOut * modOptions.DamperOutMult;
           //             }
         //           }
         //           
         //       }
         //   }
            
            
       // }
    }
}
