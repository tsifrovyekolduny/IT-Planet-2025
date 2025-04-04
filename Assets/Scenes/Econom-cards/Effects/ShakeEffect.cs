using System.Collections;
using UnityEngine;
using Assets.Scenes.Econom_cards.Interfaces;
public class ShakeEffect : MonoBehaviour, IShakeable
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.5f; // ������������ ������
    public float shakeMagnitude = 0.1f; // ��������� ������

    public IEnumerator Shake()
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPosition; // ���������� ������ �� �������� �������
    }

    void IShakeable.StartShake()
    {
        StartCoroutine(Shake());
    }
}