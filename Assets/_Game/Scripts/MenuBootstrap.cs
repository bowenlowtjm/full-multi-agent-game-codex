using UnityEngine;

namespace Pully.Game
{
    public class MenuBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            CoreLoopBootstrap core = FindFirstObjectByType<CoreLoopBootstrap>();
            if (core == null)
            {
                var go = new GameObject("CoreLoop");
                core = go.AddComponent<CoreLoopBootstrap>();
            }

            if (FindFirstObjectByType<HUDManager>() == null)
            {
                var hud = new GameObject("HUD");
                hud.AddComponent<HUDManager>();
            }

            if (FindFirstObjectByType<SceneFlowController>() == null)
            {
                var flow = new GameObject("SceneFlow");
                flow.AddComponent<SceneFlowController>();
            }
        }
    }
}
