using UnityEngine;
 
public class ObjectSpawner : MonoBehaviour
{
    public Transform plane;                 // 飛機
    public GameObject obstaclePrefab;       // 障礙物預製物
    public GameObject collectiblePrefab;    // 加分物預製物
 
    public float spawnInterval = 1f;        // 每隔幾秒生成一次
    public float spawnDistance = 80f;       // 生成在飛機前方多遠
    public float rangeUp = 15f;             // 上下隨機範圍
    public float rangeSide = 20f;           // 左右隨機範圍
    [Range(0f, 1f)]
    public float obstacleChance = 0.5f;     // 生成障礙物的機率(其餘為加分物)
    public float lifeTime = 15f;            // 物體存在幾秒後自動刪除
 
    float timer;
 
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }
 
    void Spawn()
    {
        // 飛機前方 = 機頭方向(X 軸),上下 = Y 軸,左右 = Z 軸
        Vector3 pos = plane.position
                    + plane.right * spawnDistance
                    + plane.up * Random.Range(-rangeUp, rangeUp)
                    + plane.forward * Random.Range(-rangeSide, rangeSide);
 
        GameObject prefab = Random.value < obstacleChance ? obstaclePrefab : collectiblePrefab;
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        Destroy(obj, lifeTime); // 避免場景中物體越積越多
    }
}