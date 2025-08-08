using UnityEngine;
using GameFramework;
using GameFramework.Event;

namespace GameApp
{
    public sealed class SceneCameraEnableEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(SceneCameraEnableEventArgs).GetHashCode();

        public override int Id => EventId;

        public Camera SceneCamera { get; private set; }

        public override void Clear()
        {
            SceneCamera = null;
        }

        public static SceneCameraEnableEventArgs Create(Camera sceneCamera)
        {
            SceneCameraEnableEventArgs eventArgs = ReferencePool.Acquire<SceneCameraEnableEventArgs>();
            eventArgs.SceneCamera = sceneCamera;
            return eventArgs;
        }
    }
}