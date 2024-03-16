using System.Timers;
using UnityEngine;

namespace Utility
{
    public class DebouncedSave : MonoBehaviour
    {
        public static DebouncedSave Instance { get; private set; }

        [Tooltip("Interval in seconds")]
        public float saveInterval = 2f;
        private readonly PrefixLogger logger = new("DebouncedSave");

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Save()
        {
            CancelInvoke(nameof(OnTimedEvent));
            Invoke(nameof(OnTimedEvent), saveInterval);
        }

        private void OnTimedEvent()
        {
            PlayerPrefs.Save();
            logger.Log("PlayerPrefs saved");
        }
    }
}
