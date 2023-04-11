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

        private ScreenContainer _screenContainer;
        private PopupContainer _popupContainer;

        /// <summary>
        /// Scene에서 UIManager를 초기화 해준다.
        /// </summary>
        public void InitOnScene(string initScreenName)
        {
            if (!FindContainers())
                return;

            _screenContainer.PreInit();
            _popupContainer.PreInit();

            ChangeScreen(initScreenName);
        }

        /// <summary>
        /// 스크린을 변경
        /// </summary>
        public void ChangeScreen(string screenName)
        {
            _screenContainer.ChangeScreen(screenName);
        }

        /// <summary>
        /// 팝업을 띄운다
        /// </summary>
        public void ShowPopup(string popupName)
        {
            _popupContainer.ShowPopup(popupName);
        }

        private static UIManager CreateManager()
        {
            var newManager = new GameObject("UIManager").AddComponent<UIManager>();
                    
            DontDestroyOnLoad(newManager);
            return newManager;
        }

        private bool FindContainers()
        {
            _screenContainer = FindObjectOfType<ScreenContainer>();
            _popupContainer = FindObjectOfType<PopupContainer>();

            if (_screenContainer != null && _popupContainer != null)
                return true;

            Debug.LogError("UImanager Init fail..");
            return false;
        }
    }
}