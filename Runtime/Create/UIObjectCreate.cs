#if UNITY_EDITOR
using Monpl.Utils;
using Monpl.Utils.Extensions;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Monpl.UI
{
    public static class UIObjectCreate
    {
        #region Buttons

        [MenuItem("GameObject/MPGameLib/Button/PressingButton", false, 2)]
        static void CreatePressingButton()
        {
            // ------- Create code ---------
            var pressingButtonTrs =
                EditorUtil.CreateRectTransformNewObject("PressingButton", Selection.activeTransform);
            var pressingButtonObj = pressingButtonTrs.gameObject;
            var pressingButtonComponent = pressingButtonObj.AddComponent<PressingButton>();

            var scaleButton = pressingButtonObj.GetComponent<Button>();
            scaleButton.targetGraphic = pressingButtonComponent;
            scaleButton.transition = Selectable.Transition.None;
            // ------------------------------

            EditorUtil.RegisterObject(pressingButtonObj, "Create Pressing Button");
        }

        [MenuItem("GameObject/MPGameLib/Button/ScaleButton", false, 2)]
        static void CreateScaleButton()
        {
            // ------- Create code ---------
            var scaleButtonTrs = EditorUtil.CreateRectTransformNewObject("ScaleButton", Selection.activeTransform);
            var scaleButtonObj = scaleButtonTrs.gameObject;

            var scaleButtonComponent = scaleButtonObj.AddComponent<ScaleButton>();
            var scaleButton = scaleButtonObj.GetComponent<Button>();

            scaleButton.targetGraphic = scaleButtonComponent;
            scaleButton.transition = Selectable.Transition.None;
            // ------------------------------

            EditorUtil.RegisterObject(scaleButtonObj, "Create Scale Button");
        }

        #endregion

        #region Popups

        [MenuItem("GameObject/MPGameLib/Popup/PopupBase", false, 2)]
        static void CreateNewEmptyPopup()
        {
            // -----------------
            // Base Popup
            // Trs
            var popupBaseRectTrs = EditorUtil.CreateRectTransformNewObject("NewPopup", Selection.activeTransform);
            var popupBaseObj = popupBaseRectTrs.gameObject;
            popupBaseRectTrs.SetStretchAll();
            popupBaseObj.AddComponent<PopupBase>();

            // -----------------
            // Dimming
            // Trs
            var dimmingRectTrs = EditorUtil.CreateRectTransformNewObject("Dimming", popupBaseRectTrs);
            dimmingRectTrs.SetStretchAll();

            var dimmingImg = dimmingRectTrs.gameObject.AddComponent<Image>();
            dimmingImg.color = new Color(0, 0, 0, 0.5f);
            dimmingImg.enabled = false;

            // -----------------
            // Popup Child
            var popupRectTrs = EditorUtil.CreateRectTransformNewObject("Popup", popupBaseRectTrs);
            popupRectTrs.ResetLocalPosition();
            popupRectTrs.sizeDelta = new Vector2(800, 800);
            popupRectTrs.gameObject.AddComponent<CanvasRootObject>();

            // Bg
            var bg = EditorUtil.CreateRectTransformNewObject("PopupBg", popupRectTrs);
            bg.SetStretchAll();
            bg.gameObject.AddComponent<Image>();

            Selection.activeGameObject = popupBaseObj;
            Undo.RegisterCreatedObjectUndo(popupBaseObj, "Create Popup");
            Undo.RecordObject(popupBaseObj, "Create Popup");
        }

        #endregion

        #region Screens

        [MenuItem("GameObject/MPGameLib/Screen/ScreenBase", false, 2)]
        static void CreateScreenBase()
        {
            // AddSortingLayer("UI");

            var screenRectTrs = EditorUtil.CreateRectTransformNewObject("NewScreen", Selection.activeTransform);
            var screenObj = screenRectTrs.gameObject;

            var screenCanvas = screenObj.AddComponent<Canvas>();
            var screenScaler = screenObj.AddComponent<CanvasScaler>();
            screenObj.AddComponent<GraphicRaycaster>();
            screenObj.AddComponent<ScreenBase>();

            screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            screenScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            screenScaler.referenceResolution = new Vector2(1080f, 1920f);
            screenScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            screenScaler.matchWidthOrHeight = 1f;

            Selection.activeGameObject = screenObj;

            Undo.RegisterCreatedObjectUndo(screenObj, "Create Screen");
            Undo.RecordObject(screenObj, "Create Screen");
        }

        static void AddSortingLayer(string addSortString)
        {
            var tagsAndLayersManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var sortingLayers = tagsAndLayersManager.FindProperty("m_SortingLayers");
            sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);

            var layerName = addSortString;
            for (int i = 0; i < sortingLayers.arraySize; i++)
            {
                if (sortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue.Equals(layerName))
                    return;
            }

            sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);
            var newLayer = sortingLayers.GetArrayElementAtIndex(sortingLayers.arraySize - 1);
            newLayer.FindPropertyRelative("name").stringValue = layerName;
            newLayer.FindPropertyRelative("uniqueID").intValue = layerName.GetHashCode();

            tagsAndLayersManager.ApplyModifiedProperties();
        }

        #endregion

        #region ScrollView

        [MenuItem("GameObject/MPGameLib/ScrollView/SnapScrollView_Horizontal")]
        static void CreateSnapScrollView()
        {
            // ------- Create code ---------
            var snapScrollViewTrs =
                EditorUtil.CreateRectTransformNewObject("SnapScrollView_Horizontal", Selection.activeTransform);
            var snapScrollViewObj = snapScrollViewTrs.gameObject;

            var snapScrollViewComponent = snapScrollViewObj.AddComponent<HorizontalSnapScrollView>();
            var scrollRect = snapScrollViewComponent.ScrollRect;
            var viewPortTrs = EditorUtil.CreateRectTransformNewObject("Viewport", snapScrollViewTrs);
            var contentTrs = EditorUtil.CreateRectTransformNewObject("Content", viewPortTrs);
            var dotRootTrs = EditorUtil.CreateRectTransformNewObject("Dots", snapScrollViewTrs);
            var exampleDotTrs = EditorUtil.CreateRectTransformNewObject("Dot", snapScrollViewTrs);
            var pageSampleTrs = EditorUtil.CreateRectTransformNewObject("Page1", contentTrs);

            snapScrollViewTrs.sizeDelta = new Vector2(500f, 500f);

            viewPortTrs.gameObject.AddComponent<RectMask2D>();
            viewPortTrs.SetStretchAll();
            contentTrs.SetStretchLeft(100f);

            var dotImage = exampleDotTrs.gameObject.AddComponent<Image>();
            dotImage.sprite = Resources.Load<Sprite>("unity_builtin_extra/Knob");
            exampleDotTrs.sizeDelta = new Vector2(50, 50);
            exampleDotTrs.gameObject.SetActive(false);

            snapScrollViewComponent.contentRoot = contentTrs;
            snapScrollViewComponent.dotsRoot = dotRootTrs;
            snapScrollViewComponent.dotPrefab = dotImage;

            scrollRect.vertical = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            UnityEventTools.AddPersistentListener(scrollRect.onValueChanged, snapScrollViewComponent.OnValueChanged);
            scrollRect.viewport = viewPortTrs;
            scrollRect.content = contentTrs;

            pageSampleTrs.SetStretchLeft(snapScrollViewTrs.sizeDelta.x);
            pageSampleTrs.SetAnchoredPositionX(0f);
            // ------------------------------

            EditorUtil.RegisterObject(snapScrollViewObj, "Create Snap Scroll View");
        }

        #endregion

        #region UI Objects

        [MenuItem("GameObject/UI/Image_NoneRay", false, 2002)]
        static void CreateImage_NoneRay()
        {
            var imageRectTrs = EditorUtil.CreateRectTransformNewObject("Image", Selection.activeTransform);
            var imageObj = imageRectTrs.gameObject;
            var image = imageObj.AddComponent<Image>();

            image.raycastTarget = false;

            EditorUtil.RegisterObject(imageObj, "Create Image");
        }

        [MenuItem("GameObject/UI/Text_NoneRay", false, 2000)]
        static void CreateText_NoneRay()
        {
            var textRectTrs = EditorUtil.CreateRectTransformNewObject("Text", Selection.activeTransform);
            var textObj = textRectTrs.gameObject;
            var text = textObj.AddComponent<Text>();

            text.raycastTarget = false;

            EditorUtil.RegisterObject(textObj, "Create Text");
        }

        #endregion
    }
}

#endif