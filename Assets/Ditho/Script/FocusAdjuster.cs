using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Ditho
{
    sealed class FocusAdjuster : MonoBehaviour
    {
        [SerializeField] Transform _target = null;
        [SerializeField] float _weightScale = 1;

        void LateUpdate()
        {
            var distance = (_target.position - transform.position).magnitude;
            GetComponent<PostProcessVolume>().weight = _weightScale * distance;
        }
    }
}
