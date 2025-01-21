using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BaSSharpStab2
{
    public class ButterMaterials : ThunderScript
    {
        public override void ScriptEnable()
        {
            base.ScriptEnable();
            EventManager.onPossess += EventManager_onPossess;
        }

        private void EventManager_onPossess(Creature creature, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart) return;
            GetAllModifiers();
        }
        
        private static List<string> AddMaterials = new List<string>();
        private static List<string> RemoveMaterials = new List<string>();
        public static async void GetAllModifiers()
        {
            Debug.Log("Butter Materials Refresh Activated");
            AddMaterials.Clear();
            RemoveMaterials.Clear();
            if (modOptions.MetalToggle)
                AddMaterials.Add("Metal");
            else RemoveMaterials.Add("Metal");
            if (modOptions.PlateToggle)
                AddMaterials.Add("Plate");
            else RemoveMaterials.Add("Plate");
            if (modOptions.WoodToggle)
                AddMaterials.Add("Wood");
            else RemoveMaterials.Add("Wood");
            if (modOptions.LeatherToggle)
                AddMaterials.Add("Leather");
            else RemoveMaterials.Add("Leather");
            if (modOptions.GlassToggle)
                AddMaterials.Add("Glass");
            else RemoveMaterials.Add("Glass");
            if (modOptions.StoneToggle)
                AddMaterials.Add("Stone");
            else RemoveMaterials.Add("Stone");
            if (modOptions.CeramicToggle)
                AddMaterials.Add("Ceramic");
            else RemoveMaterials.Add("Ceramic");
            if (modOptions.ChainToggle)
                AddMaterials.Add("Chainmail");
            else RemoveMaterials.Add("Chainmail");
            CatalogCategory[] catalogCategoryArray = Catalog.data;
            for (int index = 0; index < catalogCategoryArray.Length; ++index)
            {
                CatalogCategory datas = catalogCategoryArray[index];
                if (datas.category == Category.DamageModifier)
                {
                    //Debug.Log("DamagerModifiers found!");
                    //Debug.Log(datas.catalogDatas.Count);
                    for (int i = 0; i < datas.catalogDatas.Count; ++i)
                    {
                        //Debug.Log("E");
                        DamageModifierData mData = datas.catalogDatas[i] as DamageModifierData;
                        //Debug.Log(mData.id);
                        ChangeCollisionIDs(mData);
                        mData = (DamageModifierData)null;
                    }
                    datas = (CatalogCategory)null;
                }
            }
            catalogCategoryArray = (CatalogCategory[])null;
            Catalog.Refresh();

        }


        public static DamageModifierData.Collision idealCollision;
        private static void ChangeCollisionIDs(DamageModifierData modifierData)
        {
            //filter out non-pierces
            bool isPierce = false;
            if (modifierData.damageType == DamageType.Pierce || modifierData.damageType == DamageType.Slash)
            {
                isPierce = true;
            }

            if (!isPierce)
            {
                //Debug.Log($"rejecting {modifierData.id} because can't pierce anything");
                return;
            }
            //Debug.Log($"ChangeCollisionIDs started on {modifierData.id}");

            

            foreach (DamageModifierData.Collision collision in modifierData.collisions)
            {
                foreach (string targetID in collision.targetMaterialIds)
                {
                    //Debug.Log($"testing if collision ID {targetID} is theone");
                    if (targetID == "Flesh")
                    {
                        //Debug.Log($"Flesh collision found for {modifierData.id}, saving");
                        idealCollision = collision as DamageModifierData.Collision;
                    }
                    else
                    {
                        //Debug.Log($"target list doesn't contain flesh, checking next:");
                        continue;
                    }
                }
            }
            if (modOptions.MaterialsEnabledValue == false)
            {
                foreach (string targetID in AddMaterials.ToList())
                {
                    //Debug.Log($"Switching {targetID} from add to remove list");
                    RemoveMaterials.Add(targetID);
                    AddMaterials.Remove(targetID);
                }
                foreach (string targetID in RemoveMaterials.ToList())
                {
                    if (idealCollision != null)
                    {
                        //Debug.Log($"removing {targetID} from ideal Collision (flesh)");
                        if (idealCollision.targetMaterialIds.Contains(targetID))
                        {
                            idealCollision.targetMaterialIds.Remove(targetID);
                        }

                    }
                }
            }
            foreach (string targetID in AddMaterials)
            {
                if (idealCollision != null)
                {
                    //Debug.Log($"adding {targetID} to ideal Collision (flesh)");
                    idealCollision.targetMaterialIds.Add(targetID);
                }
            }
            foreach (string targetID in RemoveMaterials.ToList())
            {
                if (idealCollision != null)
                {
                    //Debug.Log($"removing {targetID} from ideal Collision (flesh)");
                    if (idealCollision.targetMaterialIds.Contains(targetID))
                    {
                        idealCollision.targetMaterialIds.Remove(targetID);
                    }
                    
                }
            }
            foreach(string targetID in idealCollision.targetMaterialIds)
            {
                //Debug.Log($"{targetID} is in ideal collision");
            }
            idealCollision = null;
        }
    }
}