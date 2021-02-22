using DG.Tweening;
using Monpl.Utils.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Monpl.UI
{
    public interface ISnapListen
    {
        void OnListen(int index);
    }
    
    [RequireComponent((typeof(ScrollRect)))]
    public class HorizontalSnapScrollView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float snappingTime = 0.25f; // snapping 되는 시간
        [SerializeField] private float _changeContentThreshold = 100f;
        
        // CONTENT
        [SerializeField] private float dotSpace = 100; // dot 간격
        public RectTransform contentRoot;

        // DOT
        public Transform dotsRoot;
        public Image dotPrefab;
        [SerializeField] private Color highlightDotColor = Color.black;
        [SerializeField] private Color commonDotColor = Color.white;

        private ISnapListen _snapListener;

        private float _contentSpace; // content 간격
        private Image[] _dotImages;
        private float[] _wayPoints;
        private int _currIdx;
        private int _nextIdx;
        private float _currVal;
        private Vector2 _pointerDownMousePos;
        
        private ScrollRect _scrollRect;
        private float _contentMaxX;
        private Vector2 _contentStartVec;
        private Vector2 _contentEndVec;

        public ScrollRect ScrollRect
        {
            get
            {
                if (_scrollRect == null) _scrollRect = GetComponent<ScrollRect>();

                return _scrollRect;
            }
        }
        
        public virtual void PreInit(ISnapListen snapListener)
        {
            _snapListener = snapListener;
            int contentCnt = contentRoot.childCount;
            
            if(contentCnt == 0)
                return;

            _contentSpace = contentRoot.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
            _contentMaxX = _contentSpace * (contentCnt - 1);

            SetWayPoints(contentCnt);
            CreateDots(contentCnt);
            BatchContents();
        }

        public virtual void Init(int startIndex, bool isLerping = false)
        {
            SetPageForIndex(startIndex, isLerping);
        }
        
        public void SetPageForIndex(int index, bool isLerping = false)
        {
            if (index >= _dotImages.Length)
            {
                Debug.LogError($"page is Invalid..!, index: {index}");
                return;
            }

            _currIdx = index;
            _nextIdx = index;
            
            if (isLerping)
            {
                StartLerpingContent(index);
            }
            else
            {
                contentRoot.anchoredPosition =
                    new Vector2(_contentMaxX * _wayPoints[index] * -1, contentRoot.anchoredPosition.y);

                UpdateDotsColor(_nextIdx);
            }
        }
        
        // ----------------
        // Event Methods
        // ----------------
        public virtual void OnValueChanged(Vector2 val)
        {
            _currVal = val.x;
        }
        
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            StopLerping();
            
            _pointerDownMousePos = Input.mousePosition;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            _nextIdx = IsChangeContent() ? GetClampedNextIndex(_nextIdx) : _currIdx;
            
            StartLerpingContent(_nextIdx);
        }

        // ----------------
        // Private Methods
        // ----------------
        private void SetWayPoints(int contentCnt)
        {
            _wayPoints = new float[contentCnt];

            for (int i = 0; i < contentCnt; ++i)
            {
                _wayPoints[i] = (float) i / Mathf.Max(1, (contentCnt - 1));
            }
        }

        private void StartLerpingContent(int index)
        {
            StopLerping();
            
            ScrollRect.velocity = Vector2.zero;
            _snapListener?.OnListen(index);
            UpdateDotsColor(index);
            StartLerping(index);
        }
        
        private void CreateDots(int createCount)
        {
            _dotImages = new Image[createCount];
            var startX = -dotSpace * (createCount * 0.5f - 0.5f);
            
            for (var i = 0; i < createCount; ++i)
            {
                _dotImages[i] = Instantiate(dotPrefab).GetComponent<Image>();
                var curDot = _dotImages[i];
                var curDotTrs = curDot.transform;
                
                curDot.gameObject.SetActive(true);
                curDot.color = commonDotColor;
                curDotTrs.SetParent(dotsRoot);
                curDotTrs.SetScaleXYZ(1f);
                curDotTrs.localPosition = new Vector3(startX + dotSpace * i, 0);
            }
        }
        
        private void BatchContents()
        {
            var childCount = contentRoot.childCount;
            
            contentRoot.SetSizeDeltaX(_contentSpace * childCount);
            
            for (var i = 0; i < childCount; ++i)
            {
                contentRoot.GetChild(i).GetComponent<RectTransform>().SetAnchoredPositionX(i * _contentSpace);
            }
        }

        private void UpdateDotsColor(int highlightDotIndex)
        {
            for (int i = 0; i < _dotImages.Length; ++i)
                _dotImages[i].color = highlightDotIndex == i ? highlightDotColor : commonDotColor;
        }

        private bool IsChangeContent()
        {
            var xGap = Mathf.Abs(_pointerDownMousePos.x - Input.mousePosition.x);
            return xGap >= _changeContentThreshold;
        }

        private int GetClampedNextIndex(int pivotIndex)
        {
            return Mathf.Clamp((_pointerDownMousePos.x >= Input.mousePosition.x
                    ? pivotIndex + 1
                    : pivotIndex - 1), 0, _wayPoints.Length - 1);
        }

        private void StartLerping(int targetIndex)
        {
            _currVal = _wayPoints[targetIndex];
            _contentStartVec = contentRoot.anchoredPosition;
            _contentEndVec = new Vector2(_contentMaxX * _currVal * -1, _contentStartVec.y);

            contentRoot.DOAnchorPosX(_contentEndVec.x, snappingTime).SetEase(Ease.InOutQuad);
        }

        private void StopLerping()
        {
            contentRoot.DOKill();
            _currIdx = _nextIdx;
        }
    }
}