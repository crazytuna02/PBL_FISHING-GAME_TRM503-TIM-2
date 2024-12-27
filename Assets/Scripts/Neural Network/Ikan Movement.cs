using UnityEngine;
using UnityEngine.AI;

public class FishMovement : MonoBehaviour
{
    [Header("References")]
    public Transform bait; // Posisi umpan
    public Transform initialPosition; // Posisi awal ikan
    public Transform rod; // Posisi joran
    public ScoreManager scoreManager; // Referensi ke ScoreManager
    public FishingStatsDisplay fishingStatsDisplay; // Referensi ke script FishingStatsDisplay
    public PasangUmpan pasangUmpan; // Referensi ke script PasangUmpan

    [Header("Movement Settings")]
    public float moveSpeed = 3.5f; // Kecepatan gerak ikan
    public float rotationSpeed = 2.0f; // Kecepatan rotasi
    public float detectionRadius = 5.0f; // Radius deteksi umpan
    public float randomMovementRadius = 10f; // Radius pergerakan acak
    public float separationRadius = 3.0f; // Radius untuk menjaga jarak antar ikan
    public float reactionDelay = 3.0f; // Waktu jeda sebelum ikan bereaksi terhadap umpan

    [Header("Hook Settings")]
    public float hookWaitDuration = 3f; // Durasi ikan di kail sebelum dilepas
    public float fishResistanceStrength = 3f; // Kekuatan perlawanan ikan saat melawan tarikan
    public float rodPullSpeed = 5f; // Kecepatan pergerakan ikan menuju joran
    public float resistanceMultiplier = 2f; // Penguat perlawanan

    private NavMeshAgent agent;
    private bool isMovingToBait = false;
    private bool isHooked = false;
    private float hookTimer = 0f;
    private float reactionTimer = 0f; // Timer untuk menghitung waktu jeda reaksi ikan

    private static Transform currentTargetFish; // Ikan yang saat ini mendekati umpan
    private static bool isBaitOccupied = false;

    // Neural Network untuk perilaku ikan
    private NeuralNetwork neuralNetwork;

    void Start()
    {
        Debug.Log($"{gameObject.name}: Memulai FishMovement...");

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"{gameObject.name}: NavMeshAgent tidak ditemukan!");
            return;
        }
        agent.speed = moveSpeed;

        // Neural Network dengan 3 input, 1 hidden layer (5 neuron), dan 1 output
        neuralNetwork = new NeuralNetwork(new int[] { 3, 5, 1 });
    }

    void Update()
    {
        if (agent == null) return;

        if (isHooked)
        {
            HandleHookedState();
        }
        else
        {
            if (!isMovingToBait)
            {
                PerformRandomMovement();
            }
            else
            {
                MoveTowardsBait();
            }
        }

        RotateTowardsMovement();
    }

    void PerformRandomMovement()
    {
        if (agent.remainingDistance < 0.5f)
        {
            Vector3 randomTarget = Random.insideUnitSphere * randomMovementRadius + initialPosition.position;
            if (NavMesh.SamplePosition(randomTarget, out NavMeshHit hit, randomMovementRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        ApplySeparation();
        DetectBait();
    }

    void ApplySeparation()
    {
        Collider[] nearbyFish = Physics.OverlapSphere(transform.position, separationRadius, LayerMask.GetMask("Fish"));
        Vector3 separationForce = Vector3.zero;

        foreach (var fish in nearbyFish)
        {
            if (fish.transform != transform)
            {
                Vector3 awayFromFish = transform.position - fish.transform.position;
                separationForce += awayFromFish.normalized / awayFromFish.magnitude;
            }
        }

        if (separationForce != Vector3.zero)
        {
            Vector3 newTarget = transform.position + separationForce.normalized * randomMovementRadius;
            if (NavMesh.SamplePosition(newTarget, out NavMeshHit hit, randomMovementRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    void DetectBait()
    {
        if (!isBaitOccupied && currentTargetFish == null && Vector3.Distance(transform.position, bait.position) <= detectionRadius)
        {
            reactionTimer += Time.deltaTime; // Hitung waktu jeda reaksi ikan

            if (reactionTimer < reactionDelay)
            {
                Debug.Log($"{gameObject.name}: Masih menunggu reaksi...");
                return; // Jangan lanjutkan logika jika waktu jeda belum tercapai
            }

            float distanceToBait = Vector3.Distance(transform.position, bait.position) / detectionRadius;
            float normalizedSpeed = moveSpeed / 10f; // Normalisasi kecepatan
            float fishType = gameObject.CompareTag("Tuna") ? 1f : 0f;

            float[] inputs = { distanceToBait, normalizedSpeed, fishType };
            float[] output = neuralNetwork.FeedForward(inputs);

            Debug.Log($"{gameObject.name}: Input NN: DistanceToBait={distanceToBait}, Speed={normalizedSpeed}, Type={fishType}");
            Debug.Log($"{gameObject.name}: Output NN: {output[0]}");

            if (output[0] > 0.5f)
            {
                isMovingToBait = true;
                isBaitOccupied = true;
                currentTargetFish = transform;
                Debug.Log($"{gameObject.name}: Mendekati umpan.");
            }
            else
            {
                MoveAwayFromBait();
                Debug.Log($"{gameObject.name}: Mengabaikan umpan.");
            }

            reactionTimer = 0f; // Reset reactionTimer setelah keputusan dibuat
        }
    }

    void MoveAwayFromBait()
    {
        Vector3 awayFromBait = transform.position - bait.position;
        Vector3 newTargetPosition = transform.position + awayFromBait.normalized * randomMovementRadius;

        if (NavMesh.SamplePosition(newTargetPosition, out NavMeshHit hit, randomMovementRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void MoveTowardsBait()
    {
        agent.SetDestination(bait.position);

        if (Vector3.Distance(transform.position, bait.position) <= 1.0f)
        {
            HookFish();
        }
    }

    void HookFish()
    {
        isHooked = true;
        hookTimer = 0f;
        isMovingToBait = false;

        agent.ResetPath();
        agent.enabled = false;

        transform.position = bait.position;
        transform.parent = bait;

        scoreManager.AddScore(gameObject.CompareTag("Tuna") ? 10 : 5);
        fishingStatsDisplay.FishCaught();

        Debug.Log(gameObject.CompareTag("Tuna") ? "Tuna caught!" : "Bawal caught!");
    }

    void HandleHookedState()
    {
        hookTimer += Time.deltaTime;

        FishResistance();

        if (hookTimer >= hookWaitDuration)
        {
            ResetFish();
        }
    }

    void FishResistance()
    {
        Vector3 pullDirection = rod.position - transform.position;
        transform.position += pullDirection.normalized * rodPullSpeed * Time.deltaTime;

        Vector3 resistanceDirection = (transform.position - rod.position).normalized;
        transform.position += resistanceDirection * fishResistanceStrength * resistanceMultiplier * Time.deltaTime;

        if (Vector3.Distance(transform.position, rod.position) < 0.5f)
        {
            transform.position = rod.position;
        }
    }

    void RotateTowardsMovement()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void ResetFish()
    {
        isHooked = false;
        hookTimer = 0f;
        isMovingToBait = false;
        isBaitOccupied = false;

        if (currentTargetFish == transform)
        {
            currentTargetFish = null;
        }

        transform.position = initialPosition.position;
        transform.parent = null;

        agent.enabled = true;

        pasangUmpan.ResetBaitPosition();
        reactionTimer = 0f;
    }
}
