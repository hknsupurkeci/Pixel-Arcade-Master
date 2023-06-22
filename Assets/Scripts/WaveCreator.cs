using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Wave
{
    public GameObject level;
    public GameObject enemyPrefab;   // Düþman prefabý
    public GameObject enemyPrefabTwo;   // Düþman prefabý
    public int enemyCount;           // Düþman sayýsý
}

public class WaveCreator : MonoBehaviour
{
    public TextMeshProUGUI remaininEnemyCount;
    public GameObject textAnimation;
    public GameObject enemys;
    public Wave[] waves;              // Dalgalarýn listesi
    private int currentWaveIndex = 0;  // Mevcut dalga indeksi
    private int remainingEnemies;      // Kalan düþman sayýsý
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
            // Tüm dalgalar tamamlandýysa oyunu bitir veya istediðiniz bir þey yapýn
            SceneManager.LoadScene(0);
            return;
        }

        Wave currentWave = waves[currentWaveIndex];
        if (currentWaveIndex != 0)
        {
            waves[currentWaveIndex - 1].level.SetActive(false);
        }
        currentWave.level.SetActive(true);
        StartCoroutine(SpawnEnemies(currentWave));
    }
    private IEnumerator SpawnEnemies(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            int random = Random.Range(0, 3);
            GameObject enemy = Instantiate(random==0 ? wave.enemyPrefabTwo:wave.enemyPrefab, new Vector3(Random.Range(-20f, 20f), random == 0 ? -1.05f : - 1.282f, 0), Quaternion.identity); 
            enemy.transform.SetParent(enemys.transform);
            yield return null; // Düþmanlar arasýndaki bekleme süresi
        }
    }
}
