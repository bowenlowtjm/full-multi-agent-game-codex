using UnityEngine;

namespace Pully.Game
{
    public static class HapticsManager
    {
        public static void Hit()
        {
            if (PlayerPrefs.GetInt("pully.haptics", 1) == 0) return;
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        }

        public static void Miss()
        {
            if (PlayerPrefs.GetInt("pully.haptics", 1) == 0) return;
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        }
    }
}
