using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Monpl.UI
{
    public class ScreenContainer : MonoBehaviour
    {
        private Dictionary<string, ScreenBase> _screenDictionary;
        private Queue<string> _screenWaitingQueue;

        public string CurScreenType { get; private set; }
        public bool IsTransitioning { get; private set; }

        public ScreenBase GetCurScreen() => _screenDictionary[CurScreenType];

        public void PreInit()
        {
            _screenDictionary = new Dictionary<string, ScreenBase>();
            _screenWaitingQueue = new Queue<string>();

            FindAndAddScreens();

            CurScreenType = string.Empty;
            IsTransitioning = false;

            ScreenProcessRoutine().Forget();
        }

        private void FindAndAddScreens()
        {
            _screenDictionary.Clear();

            foreach (var child in transform.GetComponentsInChildren<ScreenBase>())
            {
                if (_screenDictionary.ContainsKey(child.name))
                    continue;

                child.PreInit();
                child.gameObject.SetActive(true);

                _screenDictionary.Add(child.name, child);
            }
        }

        public void ChangeScreen(string newScreenType, bool ignoreTransitionCheck = false)
        {
            if (IsTransitioning && !ignoreTransitionCheck)
                return;

            _screenWaitingQueue.Enqueue(newScreenType);
        }

        private async UniTaskVoid ScreenProcessRoutine()
        {
            while (true)
            {
                await UniTask.Yield();

                if (_screenWaitingQueue.Count == 0)
                    continue;

                var newScreenName = _screenWaitingQueue.Dequeue();

                if (string.IsNullOrEmpty(newScreenName) || !_screenDictionary.ContainsKey(newScreenName))
                    continue;

                if (newScreenName == CurScreenType || string.IsNullOrEmpty(CurScreenType))
                    await ShowFirstScreenRoutine(newScreenName);
                else
                    await ChangeScreenRoutine(newScreenName);

                await UniTask.Yield();
            }
        }

        private async UniTask ShowFirstScreenRoutine(string firstScreenType)
        {
            IsTransitioning = true;
            var firstScreen = _screenDictionary[firstScreenType];

            if (string.IsNullOrEmpty(CurScreenType))
                firstScreen.gameObject.SetActive(true);

            CurScreenType = firstScreenType;

            firstScreen.ShowWill();

            await firstScreen.ShowRoutine();
            IsTransitioning = false;

            firstScreen.ShowDone();
        }

        private async UniTask ChangeScreenRoutine(string newScreenType)
        {
            IsTransitioning = true;

            var curScreen = _screenDictionary[CurScreenType];
            var newScreen = _screenDictionary[newScreenType];

            CurScreenType = newScreenType;

            curScreen.DismissWill();

            await UniTask.WhenAll(curScreen.DismissRoutine(), newScreen.ShowRoutine());

            IsTransitioning = false;

            curScreen.DismissDone();
            newScreen.ShowDone();
        }
    }
}