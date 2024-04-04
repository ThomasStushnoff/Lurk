using Controllers;
using Managers;
using UnityEngine;
namespace World
{
    public class SanityLight : MonoBehaviour
    {
        [SerializeField] [Tooltip("The range to check for the player")]
        private float range = 100.0f;

        private PlayerController _controller;
        private bool _isLit;
        private Transform _player;
        private LayerMask _playerLayer;
        private Light _light;
        private int _raysCount;
        private float _coneAngle;

        private void Start()
        {
            _controller = GameManager.Instance.localPlayer;
            _player = _controller.transform;
            _playerLayer = 1 << _player.gameObject.layer;
            _light = GetComponent<Light>();
            
            _raysCount = Mathf.Clamp(Mathf.RoundToInt(_light.intensity * 10), 10, 100);
            if (_light.type == LightType.Spot)
                _coneAngle = _light.spotAngle / 2;
            else
                _coneAngle = Mathf.Lerp(10f, 45f, _light.range / 100f);
        }

        private void Update()
        {
            CastRayTowardsPlayer();
        }
        
        private void CastRayTowardsPlayer()
        {
            _isLit = false;
            var directionToTarget = (_player.position - transform.position).normalized;
            var startAngle = Quaternion.AngleAxis(-_coneAngle / 2, Vector3.up);
            var rayDirection = startAngle * directionToTarget;

            for (var i = 0; i < _raysCount; i++)
            {
                var rotator = Quaternion.AngleAxis(_coneAngle / (_raysCount - 1) * i, Vector3.up);
                var rayVector = rotator * rayDirection;
                if (Physics.Raycast(transform.position, rayVector, out var hit, range, _playerLayer))
                {
                    if (hit.collider.IsPlayer())
                    {
                        Debug.DrawLine(transform.position, hit.point, Color.green);
                        _isLit = true;
                        break;
                    }
                }
            }
            
            if (_isLit)
            {
                LightManager.Instance.RegisterSanityLight(this);
            }
            else
            {
                LightManager.Instance.UnregisterSanityLight(this);
            }
        }
    }
}