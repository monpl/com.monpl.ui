using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Monpl.UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = CreateManager();

                return _instance;
            }
        }

        public static ScreenContainer ScreenContainer { get; private set; }
        public static PopupContainer PopupContainer { get; private set; }

        /// <summary>
        /// Scene에서 UIManager를 초기화 해준다.
        /// </summary>
        public async UniTask InitOnScene(string initScreenName)
        {
            if (!FindContainers())
                return;

            // ScreenSize를 정확히 찾기 위해선 한 프레임 쉬어야한다.
            await UniTask.Yield();
            ScreenContainer.PreInit();
            await PopupContainer.PreInit();

            ScreenContainer.ChangeScreen(initScreenName);
        }

        private static UIManager CreateManager()
        {
            var newManager = new GameObject("UIManager").AddComponent<UIManager>();

            DontDestroyOnLoad(newManager);
            return newManager;
        }

        private bool FindContainers()
        {
            ScreenContainer = FindObjectOfType<ScreenContainer>();
            PopupContainer = FindObjectOfType<PopupContainer>();

            if (ScreenContainer != null && PopupContainer != null)
                return true;

            Debug.LogError("UImanager Init fail..");
            return false;
        }
    }
}