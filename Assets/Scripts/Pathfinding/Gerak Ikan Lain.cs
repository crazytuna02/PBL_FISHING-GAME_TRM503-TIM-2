using UnityEngine;

public class OtherFishMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f; // Kecepatan ikan
    public float rotationSpeed = 200f; // Kecepatan rotasi
    public float detectionRadius = 2f; // Radius deteksi untuk menghindari tabrakan
    public float avoidForce = 5f; // Gaya untuk menghindari tabrakan

    private Vector3 randomDirection; // Arah gerakan acak
    private float directionChangeInterval = 3f; // Interval waktu untuk mengganti arah
    private float directionChangeTimer = 0f; // Timer untuk pergantian arah

    void Start()
    {
        ChooseRandomDirection(); // Pilih arah acak saat memulai
    }

    void Update()
    {
        // Perbarui arah gerakan secara berkala
        directionChangeTimer += Time.deltaTime;
        if (directionChangeTimer >= directionChangeInterval)
        {
            ChooseRandomDirection();
            directionChangeTimer = 0f;
        }

        // Deteksi tabrakan di sekitar ikan
        Collider[] obstacles = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var obstacle in obstacles)
        {
            if (obstacle.gameObject != gameObject) // Hindari diri sendiri
            {
                Vector3 avoidDirection = (transform.position - obstacle.transform.position).normalized;
                randomDirection += avoidDirection * avoidForce; // Tambahkan gaya menghindar
            }
        }

        // Gerakkan ikan
        MoveFish();
    }

    void ChooseRandomDirection()
    {
        // Pilih arah acak dalam ruang 3D
        randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.1f, 0.1f), // Pergerakan vertikal lebih kecil
            Random.Range(-1f, 1f)
        ).normalized;
    }

    void MoveFish()
    {
        // Rotasi ke arah tujuan
        Quaternion targetRotation = Quaternion.LookRotation(randomDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Gerak maju
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        // Gambar radius deteksi di editor untuk debugging
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
