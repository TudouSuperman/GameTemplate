using UnityEngine;

namespace GameApp
{
    public static partial class Constant
    {
        /// <summary>
        /// 层。
        /// </summary>
        public static class Layer
        {
            public const string Default_Layer_Name = "Default";
            public static readonly int Default_Layer_Id = LayerMask.NameToLayer(Default_Layer_Name);

            public const string UI_Layer_Name = "UI";
            public static readonly int UI_Layer_Id = LayerMask.NameToLayer(UI_Layer_Name);

            public const string Targetable_Object_Layer_Name = "Targetable Object";
            public static readonly int Targetable_Object_Layer_Id = LayerMask.NameToLayer(Targetable_Object_Layer_Name);
        }
    }
}