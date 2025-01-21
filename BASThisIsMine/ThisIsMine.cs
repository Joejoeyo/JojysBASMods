using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BASThisIsMine
{
    public class ThisIsMine : ThunderScript
    {
        public class modOptions
        {
            [ModOptionButton]
            [ModOption(name = "This is mine", tooltip = "press to own all held items", valueSourceName = nameof(applySource))]
            private static void ThisIsMineButton(bool value)
            {
                ThisIsMineCode.ChangeOwnership();
            }

            public static ModOptionBool[] applySource = new[]
            {
                new ModOptionBool("own-ify", "Default.Apply", true)
            };
        }


        public class ThisIsMineCode : ThunderScript
        {
            public override void ScriptEnable()
            {
                base.ScriptEnable();
            }


            public static void ChangeOwnership()
            {
                Player player = Player.currentCreature.player;
                if (player.handLeft.ragdollHand.grabbedHandle != null)
                {
                    player.handLeft.ragdollHand.grabbedHandle.item.SetOwner(Item.Owner.Player);
                }
                if (player.handRight.ragdollHand.grabbedHandle != null)
                {
                    player.handRight.ragdollHand.grabbedHandle.item.SetOwner(Item.Owner.Player);
                }
            }

        }
    }
}
