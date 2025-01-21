using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace BaSSharpStab2
{
    internal class modOptions
    {
        [ModOptionCategory("Butter Stabs", 1)]
        [ModOption(name = "Enabled", tooltip = "Enable/Disable Butter Stabs functions - Will ONLY apply to newly spawned items, not existing ones!", defaultValueIndex = 1, order = 1)]
        private static bool Enabled(bool value)
        {
            //ButterStabs.GetAllDamagers();
            EnabledValue = value;
            return value;
        }
        public static bool EnabledValue = true;
        [ModOptionCategory("Butter Stabs", 1)]
        [ModOptionSlider]
        [ModOption(name = "Damper In Multiplier", tooltip = "How easy should things slide IN", defaultValueIndex = 3, valueSourceName = nameof(damperMultiplier), order = 2)]
        public static float DamperInMult;
        [ModOptionCategory("Butter Stabs", 1)]
        [ModOptionSlider]
        [ModOption(name = "Damper Out Multiplier", tooltip = "How easy should things slide OUT", defaultValueIndex = 3, valueSourceName = nameof(damperMultiplier), order = 3)]
        public static float DamperOutMult;
        [ModOptionCategory("Butter Stabs", 1)]
        [ModOptionButton]
        [ModOption(name = "Apply", tooltip = "Apply Changes - Will ONLY apply to newly spawned items, not existing ones!", valueSourceName = nameof(applySource), order = 4)]
        private static void ApplyMultChanges(bool value)
        {
            ButterStabs.GetAllDamagers();
        }

        //BUTTER MATERIALS OPTIONS
        [ModOptionCategory("Butter Materials", 2)]
        [ModOption(name = "Butter Materials", tooltip = "Toggle if the following materials are easily pierceable", order = 1, defaultValueIndex = 1)]
        private static bool MaterialsEnabled(bool value)
        {
            MaterialsEnabledValue = value;
            return value;
        }
        public static bool MaterialsEnabledValue = true;
        [ModOptionCategory("Butter Materials", 2)]
        [ModOption("Toggle Metal", order = 3, defaultValueIndex = 1)]
        public static bool MetalToggle;
        [ModOptionCategory("Butter Materials", 2)]
        [ModOption("Toggle Wood", order = 4, defaultValueIndex = 1)]
        public static bool WoodToggle;
        [ModOptionCategory("Butter Materials", 2)]
        [ModOption("Toggle Chainmail", order = 5, defaultValueIndex = 1)]
        public static bool ChainToggle;
        [ModOptionCategory("Butter Materials", 2)]
        [ModOption("Toggle Leather", order = 6, defaultValueIndex = 1)]
        public static bool LeatherToggle;
        [ModOptionCategory("Butter Materials", 2)]
        [ModOption("Toggle Stone", order = 7, defaultValueIndex = 1)]
        public static bool StoneToggle;
        [ModOptionCategory("Butter Materials", 2)]
        [ModOption("Toggle Glass", order = 8, defaultValueIndex = 1)]
        public static bool GlassToggle;
        [ModOptionCategory("Butter Materials", 2)]
        [ModOption("Toggle Ceramic", order = 9, defaultValueIndex = 1)]
        public static bool CeramicToggle;
        [ModOptionCategory("Butter Materials", 2)]
        [ModOption("Toggle Plate", order = 10, defaultValueIndex = 1)]
        public static bool PlateToggle;
        [ModOptionCategory("Butter Materials", 2)]
        [ModOptionButton]
        [ModOption(name = "Apply", tooltip = "Apply Changes - Will apply to newly spawned items and existing ones!", valueSourceName = nameof(applySource), order = 11)]
        private static void ApplyMaterialChanges(bool value)
        {
            ButterMaterials.GetAllModifiers();
        }

        public static ModOptionFloat[] damperMultiplier()
        {
            List<ModOptionFloat> options = new List<ModOptionFloat>();
            float val = 0.0f;
            while (val <= 3)
            {
                options.Add(new ModOptionFloat(val.ToString("0.0"), val));
                val += 0.1f;
            }
            return options.ToArray();
        }
        public static ModOptionBool[] applySource = new[] 
        {
            new ModOptionBool("Apply", "Default.Apply", true)
        };


    }
}
