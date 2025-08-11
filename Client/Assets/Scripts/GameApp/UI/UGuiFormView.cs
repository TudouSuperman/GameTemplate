using UnityEngine;

namespace GameApp
{
    [DisallowMultipleComponent]
    public abstract class UGuiFormView : MonoBehaviour
    {
        public abstract void OnInit();
    }
}