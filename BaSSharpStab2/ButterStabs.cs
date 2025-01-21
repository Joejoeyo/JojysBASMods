using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BaSSharpStab2
{
    public class ButterStabs : ThunderScript
    {

        public override void ScriptEnable()
        {
            base.ScriptEnable();
            EventManager.onPossess += EventManager_onPossess;
            //Debug.Log("Soup");
        }

        private void EventManager_onPossess(Creature creature, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart) return;
            GetAllDamagers();
        }

        private static Dictionary<DamagerData, List<(float, float)>> originalDamperValues = new Dictionary<DamagerData, List<(float, float)>>();
        
        public static async void GetAllDamagers()
        {
            //Debug.Log("Butter Stabs Refresh Activated");
            CatalogCategory[] catalogCategoryArray = Catalog.data;
            for (int index = 0; index < catalogCategoryArray.Length; ++index)
            {
                CatalogCategory datas = catalogCategoryArray[index];
                if (datas.category == Category.Damager)
                {
                    //Debug.Log("Damagers found!");
                    //Debug.Log(datas.catalogDatas.Count);
                    for (int i = 0; i < datas.catalogDatas.Count; ++i)
                    {
                        //Debug.Log("E");
                        //Debug.Log(datas.catalogDatas[i].GetType());
                        DamagerData iData = datas.catalogDatas[i] as DamagerData;
                        //Debug.Log(iData.id);
                        //Debug.Log(iData.GetType());
                        ChangeDampers(iData);
                        iData = (DamagerData)null;
                    }
                    datas = (CatalogCategory)null;
                }
            }
            catalogCategoryArray = (CatalogCategory[])null;
        }

        private static void SaveOriginalDamperValues(DamagerData damagerData)
        {
            List<(float, float)> damperValues = new List<(float, float)>();
            damperValues.Add((damagerData.penetrationHeldDamperIn, damagerData.penetrationHeldDamperOut));
            
            originalDamperValues[damagerData] = damperValues;
            //Debug.Log($"damper dictionary count {originalDamperValues.Count}");
        }

        private static void ChangeDampers(DamagerData damagerData)
        {
            
            
            if (!originalDamperValues.ContainsKey(damagerData))
            {
                SaveOriginalDamperValues(damagerData);
                
            }
            var originalValues = originalDamperValues[damagerData];
            //if disabled, set to original values and don't proceed
            if (modOptions.EnabledValue != true)
            {
                damagerData.penetrationHeldDamperIn = originalValues[0].Item1;
                damagerData.penetrationHeldDamperOut = originalValues[0].Item2;
                return;
            }
            //Debug.Log($"mod option damper multiplier In{modOptions.DamperInMult} Out{modOptions.DamperOutMult}");
            damagerData.penetrationHeldDamperIn = originalValues[0].Item1 * modOptions.DamperInMult;
            //Debug.Log($"{damagerData.id}New DamperIn{damagerData.penetrationHeldDamperIn} Original DamperIn{originalValues[0].Item1}");
            damagerData.penetrationHeldDamperOut = originalValues[0].Item2 * modOptions.DamperOutMult;
            //Debug.Log($"{damagerData.id}New DamperOut{damagerData.penetrationHeldDamperOut} Original DamperOut{originalValues[0].Item2}");
           
        }
        // cancelled but saving it just in case

        //public static void UpdateExistingDamagers()
        //{
        //    Debug.Log("updating existing damagers");
        //    for (int i = 0; i < Item.allActive.Count; i++)
        //    {
        //        Debug.Log($"number of items on update{i}");
        //        ItemData itemdata = Item.allActive[i].data;
        //        Debug.Log($"somethingorother{itemdata.type} {itemdata.id} {itemdata.damagers.Count}");
        //        if (itemdata.type != ItemData.Type.Weapon && itemdata.type != ItemData.Type.Shield && itemdata.type != ItemData.Type.Prop) { continue; }
        //        foreach (var damagerObj in itemdata.damagers)
        //        {
        //            Debug.Log($"Updated damager {damagerObj.damagerID} in {itemdata.displayName}");
        //            Debug.Log($"    BEFORE: in:{damagerObj.damagerData.penetrationHeldDamperIn} out:{damagerObj.damagerData.penetrationHeldDamperOut} general:{damagerObj.damagerData.penetrationDamper}");
        //            ChangeDampers(damagerObj.damagerData);
        //            Debug.Log($"    AFTER : in:{damagerObj.damagerData.penetrationHeldDamperIn} out:{damagerObj.damagerData.penetrationHeldDamperOut} general:{damagerObj.damagerData.penetrationDamper}");

        //        }
        //    }
        //}


        
            }
    }

