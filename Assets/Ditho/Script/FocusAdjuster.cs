using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Ditho
{
    sealed class FocusAdjuster : MonoBehaviour
    {
        [SerializeField] Transform _target = null;

        void LateUpdate()
        {
            var distance = (_target.position - transform.position).magnitude;
            GetComponent<PostProcessVolume>().weight = (distance - 0.1f) / 9.9f;
        }
    }
}
