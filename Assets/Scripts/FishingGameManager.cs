using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingGameManager : MonoBehaviour
{
    public int amountFishStart;
    public Transform fishContainer;
    public float newFishSpawnRate = 4;
    private float fishSpawnMaxY = 2;
    private float fishSpawnMinY = -3;
    private float[] fishSpawnX = {-9f, 9f};
    // Start is called before the first frame update
    void Start()
    {
        SpawnStartFish();
        StartCoroutine(SpawnNewFish());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void SpawnStartFish(){
        for (int i = 0; i < amountFishStart; i++){
            Instantiate(PickRandomFish(), new Vector2(Random.Range(-9f, 9f), Random.Range(fishSpawnMinY, fishSpawnMaxY)), Quaternion.identity, fishContainer);
        }
    }
    private IEnumerator SpawnNewFish(){
        yield return new WaitForSeconds(newFishSpawnRate);
        while(true){
            Instantiate(PickRandomFish(), new Vector2(PickRandomXSpawn(), Random.Range(fishSpawnMinY, fishSpawnMaxY)), Quaternion.identity, fishContainer);
            yield return new WaitForSeconds(newFishSpawnRate);
        }
    }
    private GameObject PickRandomFish(){
        if(GameManager.Instance.typesOfFish.Count < 1){
            Debug.Log("no fish in list");
            return null;
        }
        return GameManager.Instance.typesOfFish[Random.Range(0, GameManager.Instance.typesOfFish.Count)];
    }
    private float PickRandomXSpawn(){
        return fishSpawnX[Random.Range(0, fishSpawnX.Length)];
    }
}
