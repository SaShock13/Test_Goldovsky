using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
    public List<ParticleSystem> _particles;

    [Header("Позиция на канвасе")]
    [Tooltip("Вьюпорт (маска скролла). Если задан — партиклы проигрываются в центре вьюпорта.")]
    [SerializeField] private RectTransform _viewport;

    private void Start()
    {
        _particles = new List<ParticleSystem>();
        _particles = GetComponentsInChildren<ParticleSystem>().ToList();
    }

    public void PlayFX()
    {
        if (_viewport != null)
            transform.position = _viewport.TransformPoint(Vector3.zero);
        foreach (var particle in _particles)
        {
            particle.Play();

        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayFX();
        }

    }
}
