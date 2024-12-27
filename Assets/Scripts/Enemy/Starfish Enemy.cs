using System.Collections;
using UnityEngine;

public class StarfishEnemy : MonoBehaviour
{
    public ScoreManager scoreManager; // Referensi ke pengelola skor
    public Transform targetPoint; // Titik tujuan serangan
    public float jumpHeight = 2f; // Ketinggian lompatan
    public float jumpSpeed = 2f; // Kecepatan lompatan
    public Transform rod; // Referensi ke joran pemain

    [Header("Game Settings")]
    public float gameDuration = 60f; // Durasi permainan dalam detik

    [Header("Audio Settings")]
    public AudioClip attackSound; // Audio yang dimainkan saat bintang laut menyerang
    private AudioSource audioSource; // Komponen AudioSource untuk memainkan suara

    private int maxAttacks = 2; // Jumlah maksimum serangan yang dapat dilakukan
    private int attackCount = 0; // Jumlah serangan yang sudah dilakukan
    private bool isAttacking = false; // Status apakah bintang laut sedang menyerang

    private void Start()
    {
        // Mengecek apakah AudioSource ada, jika tidak, maka ditambahkan
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Mulai Coroutine untuk melakukan serangan
        StartCoroutine(AttackDuringGame());
    }

    private IEnumerator AttackDuringGame()
    {
        float elapsedTime = 0f;

        // Selama durasi permainan belum habis dan jumlah serangan belum mencapai batas
        while (elapsedTime < gameDuration && attackCount < maxAttacks)
        {
            // Tunggu waktu acak antara serangan berikutnya
            float randomDelay = Random.Range(10f, gameDuration / maxAttacks); // Waktu acak antara 10 detik dan sepertiga durasi permainan
            yield return new WaitForSeconds(randomDelay);

            // Jika target point ada dan bintang laut tidak sedang menyerang, mulai serangan
            if (targetPoint != null && !isAttacking)
            {
                Debug.Log("Starfish is preparing to attack!");
                yield return StartCoroutine(PerformAttack());
                attackCount++;
            }

            elapsedTime += randomDelay;
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true; // Set status serangan menjadi aktif
        Debug.Log("Starfish attack started!");

        // Memainkan suara serangan jika ada
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // Mulai animasi lompatan ke titik tujuan
        yield return StartCoroutine(JumpToTarget(targetPoint, jumpHeight, jumpSpeed));

        isAttacking = false; // Set status serangan selesai
        Debug.Log("Starfish attack completed!");
    }

    public IEnumerator JumpToTarget(Transform targetPoint, float jumpHeight, float jumpSpeed)
    {
        Vector3 startPosition = transform.position; // Posisi awal bintang laut
        Vector3 targetPosition = targetPoint.position; // Posisi tujuan serangan
        float jumpProgress = 0; // Kemajuan lompatan (antara 0 dan 1)

        // Selama kemajuan lompatan kurang dari 1
        while (jumpProgress < 1)
        {
            jumpProgress += Time.deltaTime * jumpSpeed; // Menambah kemajuan lompatan berdasarkan waktu

            float height = Mathf.Sin(Mathf.PI * jumpProgress) * jumpHeight; // Menghitung tinggi lompatan menggunakan fungsi sinus untuk membuat parabola

            // Update posisi bintang laut berdasarkan kurva parabola
            Vector3 midPosition = Vector3.Lerp(startPosition, targetPosition, jumpProgress);
            transform.position = new Vector3(midPosition.x, startPosition.y + height, midPosition.z);

            yield return null;
        }

        // Set posisi akhir bintang laut di titik tujuan
        transform.position = targetPosition;

        // Cek apakah joran bergerak jauh dari titik tujuan serangan (menghindar)
        if (rod != null && Vector3.Distance(rod.position, targetPoint.position) > 2f) // Sesuaikan threshold jika diperlukan
        {
            Debug.Log("Player avoided the attack!"); // Pemain menghindar dari serangan
            yield break; // Keluar dari coroutine tanpa mengurangi skor
        }

        // Jika serangan mengenai dan pemain tidak menghindar
        if (scoreManager != null)
        {
            scoreManager.AddScore(-1); // Kurangi skor pemain sebesar 1
            Debug.Log("Score reduced by 1 point due to starfish attack!"); // Menampilkan pesan bahwa skor berkurang
        }
        else
        {
            Debug.LogWarning("ScoreManager reference is missing. Unable to reduce score."); // Peringatan jika ScoreManager tidak ditemukan
        }
    }
}
