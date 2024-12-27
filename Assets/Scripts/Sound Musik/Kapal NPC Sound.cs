using UnityEngine;

public class NPCSound : MonoBehaviour
{
    public GameObject npcBoat; // Objek perahu NPC yang dapat diassign melalui Inspector
    public AudioClip npcAudioClip; // File audio (WAV) yang dapat diassign melalui Inspector
    public float startDelay = 20f; // Waktu tunda sebelum suara diputar, dalam detik
    public float movementThreshold = 0.1f; // Ambang batas pergerakan perahu agar suara mulai dimainkan

    private AudioSource npcAudioSource; // AudioSource yang akan diatur otomatis
    private float timer = 0f; // Penghitung waktu
    private Vector3 lastPosition; // Posisi terakhir kapal
    private bool audioStarted = false; // Untuk memastikan audio hanya dimainkan setelah waktu tunda

    void Start()
    {
        // Pastikan npcBoat telah diassign
        if (npcBoat == null)
        {
            Debug.LogError("NPC Boat is not assigned in the Inspector.");
            return;
        }

        // Tambahkan AudioSource ke GameObject jika belum ada
        npcAudioSource = gameObject.AddComponent<AudioSource>();
        npcAudioSource.clip = npcAudioClip;
        npcAudioSource.playOnAwake = false;

        // Simpan posisi awal kapal
        lastPosition = npcBoat.transform.position;
    }

    void Update()
    {
        // Hitung waktu sejak game dimulai
        timer += Time.deltaTime;

        // Periksa apakah waktu tunda telah tercapai untuk memulai audio
        if (timer >= startDelay && !audioStarted)
        {
            audioStarted = true; // Tandai bahwa audio telah dimulai
            npcAudioSource.Play(); // Putar suara setelah delay
        }

        // Deteksi apakah kapal bergerak setelah audio dimulai
        if (audioStarted)
        {
            // Deteksi apakah kapal sedang bergerak
            if (Vector3.Distance(lastPosition, npcBoat.transform.position) > movementThreshold)
            {
                if (!npcAudioSource.isPlaying)
                {
                    npcAudioSource.Play(); // Putar suara jika belum diputar
                }
            }
            else
            {
                if (npcAudioSource.isPlaying)
                {
                    npcAudioSource.Stop(); // Hentikan suara jika kapal berhenti bergerak
                }
            }

            // Update posisi terakhir kapal
            lastPosition = npcBoat.transform.position;
        }
    }
}
