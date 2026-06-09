using UnityEngine;

namespace Pully.Game
{
    /// <summary>
    /// Haptics controller for mobile vibration feedback.
    /// Wraps platform-specific vibration APIs.
    /// </summary>
    public class HapticsManager : MonoBehaviour
    {
        public static HapticsManager Instance { get; private set; }

        [Header("Settings")]
        public bool hapticsEnabled = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSettings();
        }

        private void LoadSettings()
        {
            hapticsEnabled = PlayerPrefs.GetInt("Pully_HapticsEnabled", 1) == 1;
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetInt("Pully_HapticsEnabled", hapticsEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetEnabled(bool enabled)
        {
            hapticsEnabled = enabled;
            SaveSettings();
        }

        public void Toggle()
        {
            hapticsEnabled = !hapticsEnabled;
            SaveSettings();
        }

        // Vibration patterns
        public void HitFeedback()
        {
            if (!hapticsEnabled) return;
            Vibrate(50); // Short light tap
        }

        public void MissFeedback()
        {
            if (!hapticsEnabled) return;
            Vibrate(100); // Longer buzz for miss
        }

        public void ComboFeedback()
        {
            if (!hapticsEnabled) return;
            Vibrate(30); // Very light
        }

        public void GameOverFeedback()
        {
            if (!hapticsEnabled) return;
            Vibrate(200); // Strong feedback
        }

        public void ButtonPressFeedback()
        {
            if (!hapticsEnabled) return;
            Vibrate(20); // Micro-feedback
        }

        private void Vibrate(long milliseconds)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
            {
                if (AndroidJavaObjectHelper.GetSDKInt() >= 26)
                {
                    // Use VibrationEffect for Android 8.0+
                    using (AndroidJavaClass vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect"))
                    {
                        AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, 128);
                        vibrator.Call("vibrate", effect);
                    }
                }
                else
                {
                    vibrator.Call("vibrate", milliseconds);
                }
            }
#elif UNITY_IOS && !UNITY_EDITOR
            Handheld.Vibrate();
#else
            // Editor fallback - log only
            // Debug.Log($"[Haptics] Vibrate {ms}ms");
#endif
        }
    }

    // Helper for Android SDK version
    public static class AndroidJavaObjectHelper
    {
        public static int GetSDKInt()
        {
            using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
        }
    }
}
