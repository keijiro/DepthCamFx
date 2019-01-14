using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace Ditho
{
    [ExecuteInEditMode]
    sealed class Surface : MonoBehaviour, ITimeControl, IPropertyPreview
    {
        #region Editable attributes

        [SerializeField, Range(8, 512)] int _columnCount = 256;
        [SerializeField, Range(8, 512)] int _rowCount = 256;

        [SerializeField] Texture _sourceTexture = null;
        [SerializeField] float _depth = 0.487f;
        [SerializeField] float _cutoff = 0.1f;
        [SerializeField] float _noiseAmplitude = 0;
        [SerializeField] float _noiseAnimation = 1;

        enum RenderMode { Opaque, Transparent }
        [SerializeField] RenderMode _renderMode = RenderMode.Opaque;

        [SerializeField, ColorUsage(false, true)] Color _lineColor = Color.white;
        [SerializeField] float _lineRepeat = 200;

        [SerializeField, ColorUsage(false, true)] Color _sparkleColor = Color.white;
        [SerializeField, Range(0, 1)] float _sparkleDensity = 0.5f;

        [SerializeField] Shader _shader = null;

        #endregion

        #region Private members

        Mesh _mesh;
        Material _material;
        float _controlTime = -1;

        float LocalTime { get {
            if (_controlTime < 0)
                return Application.isPlaying ? Time.time : 0;
            else
                return _controlTime;
        } }

        void LazyInitialize()
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.hideFlags = HideFlags.DontSave;
                _mesh.name = "Depth To Displace";
                _mesh.indexFormat = IndexFormat.UInt32;
                ReconstructMesh();
            }
        }

        #endregion

        #region Internal methods

        internal void ReconstructMesh()
        {
            var vertices = new List<Vector3>();

            for (var ri = 0; ri < _rowCount; ri++)
            {
                var v = (float)ri / (_rowCount - 1);
                for (var ci = 0; ci < _columnCount; ci++)
                {
                    var u = (float)ci / (_columnCount - 1);
                    vertices.Add(new Vector3(u, v, 0));
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
                    indices[i++] = head + _columnCount;
                    indices[i++] = head + 1;

                    indices[i++] = head + 1;
                    indices[i++] = head + _columnCount;
                    indices[i++] = head + _columnCount + 1;
                }
            }

            _mesh.Clear();
            _mesh.SetVertices(vertices);
            _mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            _mesh.bounds = new Bounds(Vector3.zero, new Vector3(1, 1, 10));
            _mesh.UploadMeshData(true);
        }

        #endregion

        #region ITimeControl implementation

        public void OnControlTimeStart()
        {
        }

        public void OnControlTimeStop()
        {
            _controlTime = -1;
        }

        public void SetTime(double time)
        {
            _controlTime = (float)time;
        }

        #endregion

        #region IPropertyPreview implementation

        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
        }

        #endregion

        #region MonoBehaviour implementation

        void OnDestroy()
        {
            Utility.Destroy(_mesh);
            Utility.Destroy(_material);
        }

        void Update()
        {
            LazyInitialize();

            _material.mainTexture = _sourceTexture;

            _material.SetVector("_DepthParams", new Vector2(
                _depth, _cutoff
            ));

            _material.SetVector("_NoiseParams", new Vector2(
                _noiseAmplitude, _noiseAnimation
            ));

            if (_renderMode == RenderMode.Opaque)
            {
                _material.SetInt("_ZWrite", 1);   // On
                _material.SetInt("_Cull", 2);     // Back
                _material.SetInt("_SrcBlend", 1); // One
                _material.SetInt("_DstBlend", 0); // Zero
                _material.renderQueue = 2450;     // AlphaTest
            }
            else
            {
                _material.SetInt("_ZWrite", 0);   // Off
                _material.SetInt("_Cull", 0);     // Off
                _material.SetInt("_SrcBlend", 1); // One
                _material.SetInt("_DstBlend", 1); // One
                _material.renderQueue = 3000;     // Transparent
            }

            _material.SetColor("_LineColor", _lineColor);
            _material.SetFloat("_LineRepeat", _lineRepeat);

            _material.SetColor("_SparkleColor", _sparkleColor);
            _material.SetFloat("_SparkleDensity", _sparkleDensity);

            _material.SetFloat("_LocalTime", LocalTime + 10);

            Graphics.DrawMesh(
                _mesh, transform.localToWorldMatrix,
                _material, gameObject.layer
            );
        }

        #endregion
    }
}
