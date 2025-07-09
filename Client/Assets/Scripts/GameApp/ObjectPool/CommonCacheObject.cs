using UnityEngine;
using GameFramework;
using GameFramework.ObjectPool;

namespace GameApp
{
    public sealed class CommonCacheObject : ObjectBase
    {
        public static CommonCacheObject Create(string name, object target, bool locked)
        {
            CommonCacheObject _instanceObject = ReferencePool.Acquire<CommonCacheObject>();
            _instanceObject.Initialize(name, target, locked);
            return _instanceObject;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            GameObject _instance = (GameObject)Target;
            if (_instance == null)
            {
                return;
            }

            _instance.SelfSetActive(true);
        }

        protected override void OnUnspawn()
        {
            base.OnUnspawn();
            GameObject _instance = (GameObject)Target;
            if (_instance == null)
            {
                return;
            }

            _instance.transform.SetParent(GameEntry.ObjectPool.transform);
            _instance.SelfSetActive(false);
            _instance.transform.localPosition = Vector3.zero;
        }

        protected override void Release(bool isShutdown)
        {
            GameObject _instance = (GameObject)Target;
            if (_instance == null)
            {
                return;
            }

            Object.Destroy(_instance);
        }
    }
}