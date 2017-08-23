using UnityEngine;

namespace IntelligentMachine.Twitch.IRC
{
    public class ManagerBase<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if(_instance == null)
                {
                    var obj = FindObjectOfType<T>();
                    if (obj != null)
                        _instance = obj;
                    else
                    {
                        GameObject go = new GameObject("[IntelliManager]");
                        _instance = go.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        public virtual void Awake()
        {
            if(_instance == null)
            {
                _instance = this as T;
            }

            DontDestroyOnLoad(Instance);
        }
    }
}

