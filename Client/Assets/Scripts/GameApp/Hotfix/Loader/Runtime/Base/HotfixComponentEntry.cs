using System;
using System.Collections.Generic;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameApp.Hotfix
{
    public static class HotfixComponentEntry
    {
        private static readonly GameFrameworkLinkedList<HotfixComponent> s_GameHotfixComponents = new GameFrameworkLinkedList<HotfixComponent>();

        public static void Initialize()
        {
            foreach (HotfixComponent module in s_GameHotfixComponents)
            {
                module.OnInitialize();
            }
        }

        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (HotfixComponent module in s_GameHotfixComponents)
            {
                module.OnUpdate(elapseSeconds, realElapseSeconds);
            }
        }

        public static void Shutdown()
        {
            for (LinkedListNode<HotfixComponent> current = s_GameHotfixComponents.Last; current != null; current = current.Previous)
            {
                current.Value.OnShutdown();
            }

            s_GameHotfixComponents.Clear();
        }

        public static T GetComponent<T>() where T : HotfixComponent
        {
            return (T)GetComponent(typeof(T));
        }

        public static HotfixComponent GetComponent(Type type)
        {
            LinkedListNode<HotfixComponent> current = s_GameHotfixComponents.First;
            while (current != null)
            {
                if (current.Value.GetType() == type)
                {
                    return current.Value;
                }

                current = current.Next;
            }

            return null;
        }

        public static HotfixComponent GetComponent(string typeName)
        {
            LinkedListNode<HotfixComponent> current = s_GameHotfixComponents.First;
            while (current != null)
            {
                Type type = current.Value.GetType();
                if (type.FullName == typeName || type.Name == typeName)
                {
                    return current.Value;
                }

                current = current.Next;
            }

            return null;
        }

        public static void RegisterComponent<T>(T hotComponent) where T : HotfixComponent
        {
            if (hotComponent == null)
            {
                Log.Error("Hotfix component is invalid.");
                return;
            }

            Type type = hotComponent.GetType();

            LinkedListNode<HotfixComponent> current = s_GameHotfixComponents.First;
            while (current != null)
            {
                if (current.Value.GetType() == type)
                {
                    Log.Error("Hotfix component type '{0}' is already exist.", type.FullName);
                    return;
                }

                current = current.Next;
            }

            current = s_GameHotfixComponents.First;
            while (current != null)
            {
                if (hotComponent.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                s_GameHotfixComponents.AddBefore(current, hotComponent);
            }
            else
            {
                s_GameHotfixComponents.AddLast(hotComponent);
            }
        }
    }
}