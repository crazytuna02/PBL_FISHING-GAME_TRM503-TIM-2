using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingBehavior : MonoBehaviour
{
    public Transform detectionPoint; // Titik objek untuk mendeteksi ikan makan umpan
    public List<Transform> fishes; // Daftar objek ikan
    public Transform bait; // Objek Umpan
    public Transform hook; // Objek Kail
    public Transform fishingRod; // Objek Joran
    public float holdDuration = 3f; // Durasi menahan di titik deteksi
    public float followDuration = 2f; // Durasi mengikuti pergerakan joran

    private Dictionary<Transform, Vector3> initialPositions; // Menyimpan posisi awal setiap objek (ikan, umpan, kail)

    private bool isHolding = false;
    private bool isFollowing = false;

    void Start()
    {
        // Simpan posisi awal semua objek
        initialPositions = new Dictionary<Transform, Vector3>();

        // Simpan posisi awal ikan
        foreach (var fish in fishes)
        {
            initialPositions[fish] = fish.position;
        }

        // Simpan posisi awal umpan dan kail
        initialPositions[bait] = bait.position;
        initialPositions[hook] = hook.position;
    }

    void Update()
    {
        if (isHolding)
        {
            HoldObjectsAtDetectionPoint();
        }

        if (isFollowing)
        {
            FollowFishingRod();
        }
    }

    public void OnFishBite()
    {
        // Dipanggil saat salah satu ikan mendeteksi umpan
        StartCoroutine(FishingRoutine());
    }

    private IEnumerator FishingRoutine()
    {
        // Tahan ikan, umpan, dan kail di titik deteksi
        isHolding = true;
        yield return new WaitForSeconds(holdDuration);

        // Lepas dan ikuti pergerakan joran
        isHolding = false;
        isFollowing = true;
        yield return new WaitForSeconds(followDuration);

        // Kembali ke posisi awal
        isFollowing = false;
        ResetObjectsToInitialPosition();
    }

    private void HoldObjectsAtDetectionPoint()
    {
        // Pindahkan semua ikan, umpan, dan kail ke titik deteksi
        foreach (var fish in fishes)
        {
            fish.position = detectionPoint.position;
        }
        bait.position = detectionPoint.position;
        hook.position = detectionPoint.position;
    }

    private void FollowFishingRod()
    {
        // Semua ikan, umpan, dan kail mengikuti pergerakan joran
        foreach (var fish in fishes)
        {
            fish.position = Vector3.Lerp(fish.position, fishingRod.position, Time.deltaTime * 5f);
        }
        bait.position = Vector3.Lerp(bait.position, fishingRod.position, Time.deltaTime * 5f);
        hook.position = Vector3.Lerp(hook.position, fishingRod.position, Time.deltaTime * 5f);
    }

    private void ResetObjectsToInitialPosition()
    {
        // Kembalikan semua ikan, umpan, dan kail ke posisi awal
        foreach (var fish in fishes)
        {
            if (initialPositions.ContainsKey(fish))
            {
                fish.position = initialPositions[fish];
            }
        }
        bait.position = initialPositions[bait];
        hook.position = initialPositions[hook];
    }
}
