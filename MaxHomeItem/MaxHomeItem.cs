using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace MaxHomeItem
{
    public class MaxItemsHome : ThunderScript
    {
        internal class modOptions
        {
            [ModOptionSlider]
            [ModOption(name = "Max Item Value", tooltip = "How many items can be saved to the home. Requires a level reload. Having too many items may cause performance issues and/or large save file sizes!", defaultValueIndex = 100, valueSourceName = nameof(ValueArray), order = 1)]
            public static int MaxItemValue;

            public static ModOptionInt[] ValueArray()
            {
                List<ModOptionInt> options = new List<ModOptionInt>();
                int val = 0;
                while (val <= 1000)
                {
                    options.Add(new ModOptionInt(val.ToString("0"), val));
                    val += 1;
                }
                return options.ToArray();
            }
        }


        private class MaxItemCode : ThunderScript
        {
            public override void ScriptEnable()
            {
                base.ScriptEnable();
                EventManager.onLevelLoad += EventManager_onLevelLoad;
            }

            private void EventManager_onLevelLoad(LevelData levelData, LevelData.Mode mode, EventTime eventTime)
            {
                Catalog.gameData.pc.maxHomeItem = modOptions.MaxItemValue;
                Debug.Log($"Set max saved items to {modOptions.MaxItemValue}");
            }
        }
    }
}
