using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{
    [System.Serializable]
    public class SpawnInfo
    {
        public GameObject prefab;
        public float delayTime;
        public bool ownRotate = true;
    }

    public SpawnInfo start;
    public SpawnInfo enable;
    public SpawnInfo disable;
    public SpawnInfo destroy;

    // Start is called before the first frame update
    private void Start()
    {
        SpwanPrefab(start);
    }

    private void OnEnable()
    {
        SpwanPrefab(enable);
    }

    private void OnDisable()
    {
        SpwanPrefab(disable);
    }

    public void OnDestroy()
    {
        SpwanPrefab(destroy);
    }

    private void SpwanPrefab(SpawnInfo spawnInfo)
    {
        if (spawnInfo.prefab == null)
            return;

        if (spawnInfo.delayTime <= 0)
        {
            Instantiate(spawnInfo);
        }
        else
        {
            CoroutineManager.DelayCoroutine(spawnInfo.delayTime, () =>
            {
                Instantiate(spawnInfo);
            });
        }
    }

    private void Instantiate(SpawnInfo spawnInfo)
    {
        //폭발 이미지 좌우 반전 안되게
        Instantiate(spawnInfo.prefab, transform.position,
            spawnInfo.ownRotate ? Quaternion.identity : transform.rotation);
    }
}