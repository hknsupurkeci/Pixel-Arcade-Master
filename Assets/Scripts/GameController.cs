using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject enemyPref;
    private void Start()
    {
        //StartCoroutine(EnemyCreator());
    }

    IEnumerator EnemyCreator()
    {
        while (true)
        {
            Instantiate(enemyPref, new Vector3(Random.Range(-20f, 30f), -1.282f, 0), Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
