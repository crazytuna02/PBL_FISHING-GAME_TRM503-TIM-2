using UnityEngine;
using System.Collections;

public class SandalMovement : MonoBehaviour
{
    public float speed = 2f;                      // Kecepatan gerakan sandal
    public float stuckDuration = 25f;            // Durasi sandal tersangkut
    public float targetPointDuration = 5f;       // Durasi menetap di titik tujuan
    public Transform hookPosition;               // Titik posisi kail
    public Transform targetPoint;                // Titik tujuan lain
    private bool isStuck = false;                // Status apakah sandal tersangkut
    private bool isMovingToHook = false;         // Status apakah sandal sedang bergerak ke kail
    private bool isMovingToTarget = false;       // Status apakah sandal sedang bergerak ke target point

    void Start()
    {
        // Tidak perlu menyimpan posisi awal, karena sandal akan bergerak secara bebas
    }

    void Update()
    {
        // Jika sandal sedang tersangkut, hentikan gerakan
        if (isStuck) return;

        if (isMovingToHook && hookPosition != null)
        {
            Debug.Log("Sandal sedang bergerak menuju kail...");
            MoveToHook();
        }
        else if (isMovingToTarget && targetPoint != null)
        {
            Debug.Log("Sandal sedang bergerak menuju titik tujuan...");
            MoveToTargetPoint();
        }
        else
        {
            // Gerakan bebas jika tidak menuju kail atau target point
            MoveRandomly();
        }
    }

    private void MoveRandomly()
    {
        // Gerakkan sandal secara acak di area tertentu
        float x = Mathf.Sin(Time.time) * 5f;  // Gerakan horizontal
        float z = Mathf.Cos(Time.time) * 5f;  // Gerakan vertikal
        transform.position += new Vector3(x, 0, z) * speed * Time.deltaTime;

        Debug.Log("Sandal bergerak secara acak di area.");
    }

    private void MoveToHook()
    {
        // Gerakkan sandal langsung ke posisi kail
        Vector3 direction = (hookPosition.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Jika sandal mencapai posisi kail, tetapkan status tersangkut
        if (Vector3.Distance(transform.position, hookPosition.position) < 0.1f)
        {
            Debug.Log("Sandal mencapai kail!");
            GetStuck();
        }
    }

    private void MoveToTargetPoint()
    {
        // Gerakkan sandal langsung ke target point
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Jika sandal mencapai target point, mulai durasi menetap
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            Debug.Log("Sandal mencapai titik tujuan!");
            StartCoroutine(StayAtTargetPoint());
        }
    }

    // Fungsi untuk memulai gerakan sandal ke kail
    public void StartMovingToHook()
    {
        if (!isStuck)
        {
            isMovingToHook = true;
            isMovingToTarget = false;
            Debug.Log("Perintah diberikan: Sandal mulai bergerak ke kail.");
        }
    }

    // Fungsi untuk memulai gerakan sandal ke target point
    public void StartMovingToTargetPoint()
    {
        if (!isStuck)
        {
            isMovingToTarget = true;
            isMovingToHook = false;
            Debug.Log("Perintah diberikan: Sandal mulai bergerak ke titik tujuan.");
        }
    }

    private IEnumerator StayAtTargetPoint()
    {
        isMovingToTarget = false; // Hentikan gerakan menuju target point
        Debug.Log("Sandal berhenti di titik tujuan selama " + targetPointDuration + " detik.");
        yield return new WaitForSeconds(targetPointDuration); // Tunggu selama targetPointDuration detik
        Debug.Log("Sandal selesai berhenti di titik tujuan dan kembali bergerak bebas.");
    }

    // Fungsi untuk membuat sandal tersangkut
    private void GetStuck()
    {
        isStuck = true;
        isMovingToHook = false; // Hentikan pergerakan ke kail
        Debug.Log("Sandal tersangkut di kail.");
        StartCoroutine(ReleaseAfterDelay());
    }

    // Coroutine untuk melepaskan sandal setelah waktu tertentu
    private IEnumerator ReleaseAfterDelay()
    {
        Debug.Log("Sandal akan tersangkut selama " + stuckDuration + " detik.");
        yield return new WaitForSeconds(stuckDuration); // Tunggu selama `stuckDuration` detik
        isStuck = false; // Kembalikan status agar sandal bisa bergerak lagi
        Debug.Log("Sandal bebas dan kembali bergerak!");
    }
}
