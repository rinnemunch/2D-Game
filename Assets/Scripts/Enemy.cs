using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private GameObject _laserPrefab;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    
    private bool _isDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("The animator is NULL.");
        }
    }

    
    void Update()
    {
        if (_isDestroyed)
        {
            return; 
        }

        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;

            Vector3 laserSpawnPosition = transform.position + Vector3.down * 0.5f;
            GameObject enemyLaser = Instantiate(_laserPrefab, laserSpawnPosition, Quaternion.identity);

            // Set tag for parent and all children (laser prefab)
            enemyLaser.tag = "EnemyLaser";
            foreach (Transform child in enemyLaser.transform)
            {
                child.tag = "EnemyLaser";
            }

            // Get and configure all laser components
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            foreach (Laser laser in lasers)
            {
                laser.AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // return will cause the enemylaser tag to not collide with enemy
        if (other.CompareTag("EnemyLaser"))
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            HandleEnemyDestruction();
        }
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            HandleEnemyDestruction();
        }
    }

    
    private void HandleEnemyDestruction()
    {
        _isDestroyed = true;
        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _audioSource.Play();
        Destroy(GetComponent<Collider2D>()); 
        Destroy(this.gameObject, 2.8f); 
    }

}
