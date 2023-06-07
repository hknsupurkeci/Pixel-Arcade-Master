using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Wave
{
    public GameObject enemyPrefab;   // D��man prefab�
    public int enemyCount;           // D��man say�s�
}

public class WaveCreator : MonoBehaviour
{
    public TextMeshProUGUI remaininEnemyCount;
    public GameObject textAnimation;
    public GameObject enemys;
    public Wave[] waves;              // Dalgalar�n listesi
    private int currentWaveIndex = 0;  // Mevcut dalga indeksi
    private int remainingEnemies;      // Kalan d��man say�s�
    private void Start()
    {
        textAnimation.GetComponent<TextAnimation>().WaveTextStart((currentWaveIndex + 1).ToString());
        StartNextWave();
    }

    private void Update()
    {
        remaininEnemyCount.text = enemys.transform.childCount.ToString();
        if (enemys.transform.childCount == 0)
        {
            currentWaveIndex++;
            textAnimation.GetComponent<TextAnimation>().WaveTextStart((currentWaveIndex + 1).ToString());
            StartNextWave();
        }
    }
    private void StartNextWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            // T�m dalgalar tamamland�ysa oyunu bitir veya istedi�iniz bir �ey yap�n
            return;
        }

        Wave currentWave = waves[currentWaveIndex];
        StartCoroutine(SpawnEnemies(currentWave));
    }

    private IEnumerator SpawnEnemies(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            GameObject enemy = Instantiate(wave.enemyPrefab, new Vector3(Random.Range(-20f, 30f), -1.282f, 0), Quaternion.identity);
            enemy.transform.SetParent(enemys.transform);
            yield return null; // D��manlar aras�ndaki bekleme s�resi
        }
    }
}
