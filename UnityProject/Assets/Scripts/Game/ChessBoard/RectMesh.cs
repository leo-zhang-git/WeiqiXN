using System;
using System.Collections.Generic;
using UnityEngine;
using XNClient.Logger;

namespace XNClient.ChessBoard
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class RectMesh : MonoBehaviour
    {
        public bool useMeshColor;

        private Mesh rectMesh;
        private MeshCollider meshCollider;

        [NonSerialized] private List<Vector3> vertices = new List<Vector3>();
        [NonSerialized] private List<int> triangles = new List<int>();
        [NonSerialized] private List<Color> colors = new List<Color>();

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = rectMesh = new Mesh();
            rectMesh.name = "RectMesh";
            meshCollider = GetComponent<MeshCollider>();
        }

        public void RefreshMesh()
        {
            rectMesh.SetVertices(vertices);
            rectMesh.SetTriangles(triangles, 0);
            if (useMeshColor) {
                if (vertices.Count != colors.Count) {
                    XNLogger.LogError("Rect mesh colors length invalid, set colors failed.");
                } else {
                    rectMesh.SetColors(colors);
                }
            }
            rectMesh.RecalculateNormals();

            meshCollider.sharedMesh = rectMesh;
        }

        public void ClearMesh()
        {
            rectMesh.Clear();

            vertices.Clear();
            triangles.Clear();
            colors.Clear();
        }

        // 注意顶点需要以使得三角面法线方向朝上的顺序（顺时针）传进来，影响正反面判定
        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        public void AddTriangleColor(Color c)
        {
            colors.Add(c);
            colors.Add(c);
            colors.Add(c);
        }

        public void AddTriangleColor(Color c1, Color c2, Color c3)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
        }

        // 注意这里是分别传入的quad两条边的端点，对应quad的 左下，右下，左上，右上
        public void AddQuad(Vector3 lineStart1, Vector3 lineEnd1, Vector3 lineStart2, Vector3 lineEnd2)
        {
            // 公用顶点切割成两个三角
            int vertexIndex = vertices.Count;
            vertices.Add(lineStart1);
            vertices.Add(lineEnd1);
            vertices.Add(lineStart2);
            vertices.Add(lineEnd2);

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);

            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }

        public void AddQuadColor(Color c)
        {
            colors.Add(c);
            colors.Add(c);
            colors.Add(c);
            colors.Add(c);
        }

        public void AddQuadColor(Color lineStartColor1, Color lineEndColor1, Color lineStartColor2, Color lineEndColor2)
        {
            colors.Add(lineStartColor1);
            colors.Add(lineEndColor1);
            colors.Add(lineStartColor2);
            colors.Add(lineEndColor2);
        }

        public void AddQuadColor(Color lineColor1, Color lineColor2)
        {
            colors.Add(lineColor1);
            colors.Add(lineColor1);
            colors.Add(lineColor2);
            colors.Add(lineColor2);
        }
    }
}
