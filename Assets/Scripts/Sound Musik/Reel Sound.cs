using UnityEngine;

public class ReelSoundManager : MonoBehaviour
{
    public AudioSource reelAudio;

    void Update()
    {
        // Deteksi input tarikan reel, misalnya menggunakan Spacebar
        if (Input.GetKey(KeyCode.Space)) // Ganti dengan input reel Anda
        {
            if (!reelAudio.isPlaying) // Hindari suara berulang
            {
                reelAudio.Play();
            }
        }
        else
        {
            if (reelAudio.isPlaying) // Hentikan suara jika tidak menarik
            {
                reelAudio.Stop();
            }
        }
    }
}
