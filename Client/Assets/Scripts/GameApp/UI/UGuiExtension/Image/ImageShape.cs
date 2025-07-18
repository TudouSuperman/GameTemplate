using UnityEngine;
using UnityEngine.UI;

namespace GameApp.UI.Extension
{
    public sealed class ImageShape : Image
    {
        public Vector3 Offset;

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);

            UIVertex _vertex = new UIVertex();
            toFill.PopulateUIVertex(ref _vertex, 1);
            _vertex.position += Offset;
            toFill.SetUIVertex(_vertex, 1);

            _vertex = new UIVertex();
            toFill.PopulateUIVertex(ref _vertex, 2);
            _vertex.position += Offset;
            toFill.SetUIVertex(_vertex, 2);

            _vertex = new UIVertex();
            toFill.PopulateUIVertex(ref _vertex, 3);
            _vertex.position -= Offset;
            toFill.SetUIVertex(_vertex, 3);

            _vertex = new UIVertex();
            toFill.PopulateUIVertex(ref _vertex, 0);
            _vertex.position -= Offset;
            toFill.SetUIVertex(_vertex, 0);
        }
    }
}