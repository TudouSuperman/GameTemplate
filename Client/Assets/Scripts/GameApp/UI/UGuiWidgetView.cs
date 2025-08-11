using UnityEngine;

namespace GameApp
{
    [DisallowMultipleComponent]
    public abstract class UGuiWidgetView : MonoBehaviour
    {
        public abstract void OnInit();
    }
}