using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using ThunderRoad.AI.Get;
using UnityEngine;

namespace BaSSharpStab2
{
    public class ModifyAllDampers : ThunderScript
    {
        public override void ScriptEnable()
        {
            base.ScriptEnable();
            EventManager.onPossess += EventManager_onPossess;
            EventManager.OnToggleOptionsMenu += EventManager_OnToggleOptionsMenu;
            EventManager.onItemSpawn += EventManager_onItemSpawn;
        }

        

        public void updateDamagerDampers()
        {
            //if mod is disabled in options, don't update damagers
            if (modOptions.Enabled != true) { return; }
           
            foreach (Item item in Item.allActive)
            {
                Debug.Log("Butterstabs: i happened!");
                if (item.data.type != ItemData.Type.Weapon) { return; }
                foreach (var damager in item.data.damagers)
                {
                    Debug.Log($"{Time.time}Butterstabs: Updating Damagers ({damager.damagerID}) for {item.name}");
                    Debug.Log($"damager {damager.damagerID} before damperIN value: {damager.damagerData.penetrationHeldDamperIn}");
                    damager.damagerData.penetrationHeldDamperIn = damager.damagerData.penetrationHeldDamperIn * modOptions.DamperInMult;
                    Debug.Log($"damager {damager.damagerID} after damperIN value: {damager.damagerData.penetrationHeldDamperIn}");
                    Debug.Log($"damager {damager.damagerID} before damperOUT value: {damager.damagerData.penetrationHeldDamperOut}");
                    damager.damagerData.penetrationHeldDamperOut = damager.damagerData.penetrationHeldDamperOut * modOptions.DamperOutMult;
                    Debug.Log($"damager {damager.damagerID} after damperOUT value: {damager.damagerData.penetrationHeldDamperOut}");
                }
            }
        }

        private void EventManager_onItemSpawn(Item item)
        {
            if (item.data.type != ItemData.Type.Weapon) { return; }
            Debug.Log("Butterstabs: updating from weapon spawn!");
            updateDamagerDampers();
        }

        private void EventManager_OnToggleOptionsMenu(bool isVisible)
        {
            Debug.Log("Butterstabs: updating from book close!");
            updateDamagerDampers();
        }

        private void EventManager_onPossess(Creature creature, EventTime eventTime)
        {
            Debug.Log("Butterstabs: updating from player possess!");
            updateDamagerDampers();
        }
        
    }

}
