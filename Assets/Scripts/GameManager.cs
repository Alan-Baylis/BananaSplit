using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int _maxNumDrops = 3;
    [SerializeField]
    private Transform _splittableParent = null;
    [SerializeField]
    private GameObject[] _fruitPrefabs = null;
    [SerializeField]
    private float _spawnTime = 5f;
    [SerializeField]
    private float _spawnSize = 10f;

    public static GameManager Instance { get; set; }
    private bool _gameRunning = true;

    private int _numSplits = 0;
    private int _numDropped = 0;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Splitable.OnSplit += SplitOccured;
        DeathTrigger.OnDeathTrigger += FruitDropped;

        StartCoroutine(SpawnCoroutine());
    }

    private void OnDestroy()
    {
        Splitable.OnSplit -= SplitOccured;
    }

    private IEnumerator SpawnCoroutine()
    {
        while (_gameRunning)
        {
            yield return new WaitForSeconds(_spawnTime);

            // Make sure we didnt lose during the wait
            if (!_gameRunning)
                break;

            int idx = Random.Range(0, _fruitPrefabs.Length);
            var go = Instantiate(_fruitPrefabs[idx]);

            float x = Random.Range(-_spawnSize, _spawnSize);
            Vector3 pos = transform.position;
            pos.x += x;

            var trans = go.transform;
            trans.position = pos;
            trans.parent = _splittableParent;
            trans.rotation = Random.rotation;

            var impulse = Random.onUnitSphere * Random.Range(1f, 4f);
            var body = go.GetComponent<Rigidbody>();
            body.angularVelocity = impulse;

            var split = go.GetComponent<Splitable>();
            split.FireSplitEvent = true;
        }
    }

    private void SplitOccured()
    {
        _numSplits++;
    }

    private void FruitDropped()
    {
        _numDropped++;
        if (_numDropped == _maxNumDrops)
        {
            _gameRunning = false;
            Debug.Log("You lose");
        }
    }
}
