using UnityEngine;

namespace Pully.Game
{
    public class MenuBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            if (FindFirstObjectByType<CoreLoopBootstrap>() == null)
            {
                var go = new GameObject("CoreLoop");
                go.AddComponent<CoreLoopBootstrap>();
            }

            if (FindFirstObjectByType<HUDManager>() == null)
            {
                var hud = new GameObject("HUD");
                hud.AddComponent<HUDManager>();
            }
        }
    }
}
