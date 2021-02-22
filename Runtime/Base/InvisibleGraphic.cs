using UnityEngine.UI;

namespace Monpl.UI
{
    /// <summary>
    /// 투명한 영역을 터치하고 싶을때 Image대신 사용
    /// </summary>
    public class InvisibleGraphic : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}