using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace DepthCamFx
{
    class DepthToDisplace : MonoBehaviour
    {
        [SerializeField, Range(8, 1024)] int _columnCount = 256;
        [SerializeField, Range(8, 1024)] int _rowCount = 256;
        [SerializeField] Texture _sourceTexture = null;
        [SerializeField] Shader _shader = null;
        [SerializeField] float _depthScale = 1;

        Material _material;

        void Start()
        {
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = ConstructPlaneMesh();

            _material = new Material(_shader);
            _material.mainTexture = _sourceTexture;

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = _material;
        }

        void Update()
        {
            _material.SetFloat("_DepthScale", _depthScale);
        }

        Mesh ConstructPlaneMesh()
        {
            var width = _sourceTexture.width;
            var height = _sourceTexture.height;
            var aspect = (float)height / width;

            var mesh = new Mesh();

            var vertices  = new List<Vector3>();
            var normals   = new List<Vector3>();
            var texcoords = new List<Vector2>();

            for (var ri = 0; ri < _rowCount; ri++)
            {
                var v = (float)ri / (_rowCount - 1);
                var y = (v - 0.5f) * aspect;

                for (var ci = 0; ci < _columnCount; ci++)
                {
                    var u = (float)ci / (_columnCount - 1);
                    var x = u - 0.5f;

                    vertices.Add(new Vector3(x, y, 0));
                    normals.Add(new Vector3(0, 0, -1));
                    texcoords.Add(new Vector2(u, v));
                }
            }

            var indices = new int[(_rowCount - 1) * (_columnCount - 1) * 6];
            var i = 0;

            for (var ri = 0; ri < _rowCount - 1; ri++)
            {
                for (var ci = 0; ci < _columnCount - 1; ci++)
                {
                    var head = _columnCount * ri + ci;

                    indices[i++] = head;
                    indices[i++] = head + 1;
                    indices[i++] = head + _columnCount;

                    indices[i++] = head + 1;
                    indices[i++] = head + _columnCount + 1;
                    indices[i++] = head + _columnCount;
                }
            }

            mesh.name = "Depth To Displace";
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, texcoords);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.bounds = new Bounds(Vector3.zero, new Vector3(1, aspect, 10));
            mesh.UploadMeshData(true);

            return mesh;
        }
    }
}
