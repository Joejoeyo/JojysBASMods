using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace ClassicWavesPlus
{
    public class ClassicWavePlus : ThunderScript
    {
        internal class modOptions
        {
            public static string selectedTable1;
            public static string selectedTable2;
            public static ModOptionCatalogData[] CreatureTableDatas()
            {
                List<ModOptionCatalogData> options = new List<ModOptionCatalogData>();
                List<CreatureTable.Drop> allDrops = new List<CreatureTable.Drop>();
                foreach (CreatureTable table in Catalog.GetDataList<CreatureTable>())
                {
                    table.drops.ForEach(drops =>
                    {
                        if (drops.overrideContainerID != null && !drops.overrideContainerID.Contains("Rogue") && !drops.overrideContainerID.Equals("Empty"))
                        {
                            allDrops.Add(drops);
                        }
                    });
                };
                foreach (CatalogData catalogData in Catalog.GetDataList(Category.CreatureTable))
                {
                    if (catalogData is CreatureTable creatureTable && !creatureTable.id.Contains("Dungeon") && !creatureTable.id.Contains("Chicken"))
                    {
                        options.Add(new ModOptionCatalogData(creatureTable.id, creatureTable));
                    }
                }
                return options.ToArray();
            }
            [ModOption(name = "Select CreatureTable 1", tooltip = "Selects the first creatureTable for custom waves (friends)", interactionType = ModOption.InteractionType.Slider, valueSourceName = nameof(CreatureTableDatas))]
            private static void CreatureTableOption1(CatalogData value)
            {
                if (value is CreatureTable creatureTable)
                {
                    selectedTable1 = creatureTable.id;
                }
            }
            [ModOption(name = "Select CreatureTable 2", tooltip = "Selects the second creatureTable for custom waves (enemies)", interactionType = ModOption.InteractionType.Slider, valueSourceName = nameof(CreatureTableDatas))]
            private static void CreatureTableOption2(CatalogData value)
            {
                if (value is CreatureTable creatureTable)
                {
                    selectedTable2 = creatureTable.id;
                }
            }
            [ModOptionButton]
            [ModOption(name = "Apply", tooltip = "Apply Changes", valueSourceName = nameof(applySource), order = 3)]
            private static void ApplyCreatureChanges(bool value)
            {
                Debug.Log("Starting wave update from apply button");
                ClassicWaveCode.UpdateCreatureTables();
            }
            public static ModOptionBool[] applySource = new[]
            {
            new ModOptionBool("Apply", "Default.Apply", true)
        };
           
        }


        private class ClassicWaveCode : ThunderScript
        {
            public override void ScriptEnable()
            {
                base.ScriptEnable();
                EventManager.onPossess += EventManager_onPossess;
            }

            private void EventManager_onPossess(Creature creature, EventTime eventTime)
            {
                if (eventTime == EventTime.OnStart) return;
                Debug.Log("Starting wave update from possess");
                UpdateCreatureTables();
            }

            
            public static void UpdateCreatureTables()
            {
                // Find CustomJoj Waves

                CatalogCategory[] catalogCategoryArray = Catalog.data;
                for (int index = 0; index < catalogCategoryArray.Length; ++index)
                {
                    CatalogCategory datas = catalogCategoryArray[index];
                    if (datas.category == Category.Wave)
                    {
                        //Debug.Log($"Waves found! {datas.catalogDatas.Count}");
                        for (int i = 0; i < datas.catalogDatas.Count; ++i)
                        {
                            //Debug.Log("E");
                            WaveData mData = datas.catalogDatas[i] as WaveData;
                            if (mData.id.Contains("CustomJoj")) 
                            {
                                foreach (var group in mData.groups)
                                {
                                    if (group.factionID == 2 || group.factionID == 4)
                                    {
                                        group.referenceID = modOptions.selectedTable1;
                                        //Debug.Log($"set a faction 2/4 group to {modOptions.selectedTable1}");
                                    }
                                    if (group.factionID == 3)
                                    {
                                        group.referenceID = modOptions.selectedTable2;
                                        //Debug.Log($"set a faction 3 group to {modOptions.selectedTable2}");
                                    }
                                    //Debug.Log($"Table Updating Complete for {mData.id}");
                                }
                            }
                            if (mData.id.Contains("FFACustom"))
                            {
                                foreach (var group in mData.groups)
                                {
                                    
                                    if (group.factionID == 0)
                                    {
                                        group.referenceID = modOptions.selectedTable2;
                                        //Debug.Log($"set a faction 3 group to {modOptions.selectedTable2}");
                                    }
                                    //Debug.Log($"Table Updating Complete for {mData.id}");
                                }
                            }
                            //Debug.Log($"cool wave found, {mData.id}");

                            mData = (WaveData)null;
                        }
                        datas = (CatalogCategory)null;

                    }
                }
                Debug.Log("All custom waves updated");
            }
        }
    }
}
