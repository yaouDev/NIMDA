using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;


// made with some insipriation from https://www.youtube.com/watch?v=tdSmKaJvCoA&ab_channel=Brackeys
public class ObjectPool : MonoBehaviour {
    [SerializeField] private List<Pool> pools;
    private Dictionary<string, Pool> poolDictionary;
    public static ObjectPool Instance;

    // Start is called before the first frame update
    void Start() {
        Instance ??= this;
        poolDictionary = new Dictionary<string, Pool>();
        foreach (Pool pool in pools) {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            pool.ObjectPool = objectPool;
            poolDictionary.Add(pool.Tag, pool);
        }
    }

    public GameObject GetFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null, bool poolable = false) {
        if (poolDictionary.ContainsKey(tag)) {
            GameObject objectToSpawn;
            if (poolDictionary[tag].ObjectPool.Count > 0) {
                objectToSpawn = poolDictionary[tag].ObjectPool.Dequeue();
            } else {
                objectToSpawn = Instantiate(poolDictionary[tag].Prefab);
            }
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            if (parent != null) objectToSpawn.transform.SetParent(parent);
            poolDictionary[tag].ActiveObjects++;
            poolDictionary[tag].InactiveObjects--;

            if (poolable) {
                IPoolable poolableObj = objectToSpawn.GetComponent<IPoolable>();
                if (poolableObj != null) poolableObj.OnSpawn();
            }
            return objectToSpawn;
        }
        Debug.LogWarning("Pool with tag " + tag + " does not exist");
        return null;
    }

    public void ReturnToPool(string tag, GameObject objectToReturn) {
        if (poolDictionary.ContainsKey(tag)) {
            poolDictionary[tag].ActiveObjects--;
            poolDictionary[tag].InactiveObjects++;

            objectToReturn.SetActive(false);
            poolDictionary[tag].ObjectPool.Enqueue(objectToReturn);
            return;
        }
        Debug.LogWarning("Pool with tag " + tag + " does not exist");
    }

    [System.Serializable]
    public class Pool {
        [SerializeField] private string tag;
        [SerializeField] private GameObject prefab;
        private int activeObjects;
        private int inactiveObjects;
        private Queue<GameObject> objectPool;

        public Queue<GameObject> ObjectPool {
            get { return objectPool; }
            set {
                if (value != null) {
                    objectPool = value;
                }
            }
        }
        public string Tag { get { return tag; } }

        public GameObject Prefab {
            get { return prefab; }
        }

        public int ActiveObjects {
            get { return activeObjects; }
            set {
                if (value >= 0) activeObjects = value;
                else activeObjects = 0;
            }
        }

        public int InactiveObjects {
            get { return inactiveObjects; }
            set {
                if (value >= 0) inactiveObjects = value;
                else inactiveObjects = 0;
            }
        }


    }

}
