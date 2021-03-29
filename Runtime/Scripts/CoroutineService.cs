using UnityEngine;

namespace CZToolKit.GOAP
{
    public class CoroutineService : MonoBehaviour
    {
        private static CoroutineService instance;

        public static CoroutineService Instance
        {
            get
            {
                if (instance == null && (instance = GameObject.FindObjectOfType<CoroutineService>()) == null)
                    instance = new GameObject("GOAP CoroutineService").AddComponent<CoroutineService>();
                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
