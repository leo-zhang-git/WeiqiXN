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
        public bool useMeshUV1;
        public bool useMeshUV2;

        private Mesh rectMesh;
        private MeshCollider meshCollider;

        [NonSerialized] private List<Vector3> vertices = new List<Vector3>();
        [NonSerialized] private List<int> triangles = new List<int>();
        [NonSerialized] private List<Color> colors = new List<Color>();
        [NonSerialized] private List<Vector4> uv1s = new List<Vector4>();
        [NonSerialized] private List<Vector4> uv2s = new List<Vector4>();

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
            if (useMeshUV1) {
                if (vertices.Count != uv1s.Count) {
                    XNLogger.LogError("Rect mesh uv1s length invalid, set uv1s failed.");
                } else {
                    rectMesh.SetUVs(1, uv1s);
                }
            }
            if (useMeshUV2) {
                if (vertices.Count != uv2s.Count) {
                    XNLogger.LogError("Rect mesh uv2s length invalid, set uv2s failed.");
                } else {
                    rectMesh.SetUVs(2, uv2s);
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
            uv1s.Clear();
            uv2s.Clear();
        }

        // Vertices must be provided in clockwise order so the face normal points upward.
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

        public void AddTriangleUV1(Vector4 uv)
        {
            uv1s.Add(uv);
            uv1s.Add(uv);
            uv1s.Add(uv);
        }

        public void AddTriangleUV1(Vector4 uv1_1, Vector4 uv1_2, Vector4 uv1_3)
        {
            uv1s.Add(uv1_1);
            uv1s.Add(uv1_2);
            uv1s.Add(uv1_3);
        }

        // Quad vertices are passed as two edge endpoints: lower-left, lower-right, upper-left, upper-right.
        public void AddQuad(Vector3 lineStart1, Vector3 lineEnd1, Vector3 lineStart2, Vector3 lineEnd2)
        {
            // Split the quad into two triangles while sharing the four quad vertices.
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

        public void AddQuadUV1(Vector4 uv)
        {
            uv1s.Add(uv);
            uv1s.Add(uv);
            uv1s.Add(uv);
            uv1s.Add(uv);
        }

        public void AddQuadUV1(Vector4 lineStartUV1_1, Vector4 lineEndUV1_1, Vector4 lineStartUV1_2, Vector4 lineEndUV1_2)
        {
            uv1s.Add(lineStartUV1_1);
            uv1s.Add(lineEndUV1_1);
            uv1s.Add(lineStartUV1_2);
            uv1s.Add(lineEndUV1_2);
        }

        public void AddQuadUV1(Vector4 lineUV1_1, Vector4 lineUV1_2)
        {
            uv1s.Add(lineUV1_1);
            uv1s.Add(lineUV1_1);
            uv1s.Add(lineUV1_2);
            uv1s.Add(lineUV1_2);
        }
    }
}
