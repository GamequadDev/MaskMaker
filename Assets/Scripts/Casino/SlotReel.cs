using UnityEngine;
using System.Collections;

// Skrypt odpowiedzialny za animację pojedynczego bębna
public class SlotReel : MonoBehaviour
{
    [Header("Konfiguracja Bębna")]
    // Tablica prefabów (Image UI) wszystkich dostępnych owoców
    public GameObject[] fruitPrefabs;
    // Wysokość jednego owocu - MUSI być taka sama jak wysokość kontenera w UI (np. 100)
    public float fruitHeight = 100f;
    // Czas trwania jednego przeskoku owocu (im mniej, tym szybsze kręcenie)
    public float spinSpeed = 0.08f;

    [Header("Stan Aktualny")]
    // Obiekt owocu, który aktualnie widzimy na środku
    public GameObject currentFruit;

    void Start()
    {
        // Na starcie gry, jeśli nie przypisaliśmy ręcznie owocu,
        // skrypt szuka pierwszego dziecka w kontenerze.
        if (currentFruit == null && transform.childCount > 0)
        {
            currentFruit = transform.GetChild(0).gameObject;
        }

        // Upewniamy się, że startowy owoc jest idealnie na środku
        if (currentFruit != null)
        {
            currentFruit.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    // Główna korutyna kręcenia - wywoływana przez Managera
    public IEnumerator Spin(int targetId, float duration)
    {
        float elapsedTotal = 0f;

        // --- FAZA 1: KRĘCENIE LOSOWE ---
        // Kręcimy pętlą, dopóki nie minie ustalony czas (duration)
        while (elapsedTotal < duration)
        {
            // Losujemy tymczasowy owoc tylko dla efektu wizualnego
            int randomId = Random.Range(0, fruitPrefabs.Length);

            // Wykonujemy jeden pełny ruch (spawn -> przesunięcie -> zniszczenie)
            yield return StartCoroutine(MoveOneStep(randomId, spinSpeed));

            elapsedTotal += spinSpeed;
        }

        // --- FAZA 2: ZATRZYMANIE NA WYNIKU ---
        // Wykonujemy ostatni ruch, tym razem spawniąc owoc docelowy (targetId).
        // Mnożymy czas razy 2f, żeby ostatni ruch był nieco wolniejszy (efekt hamowania).
        yield return StartCoroutine(MoveOneStep(targetId, spinSpeed * 2.5f));

        // Korekta końcowa - upewniamy się, że finalny owoc jest co do piksela na środku (0,0)
        currentFruit.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    // Korutyna wykonująca jeden "ząbek" ruchu taśmy
    private IEnumerator MoveOneStep(int nextFruitId, float timeForStep)
    {
        // 1. Spawnowanie nowego owocu NAD aktualnym
        GameObject nextFruit = Instantiate(fruitPrefabs[nextFruitId], transform);
        RectTransform nextRect = nextFruit.GetComponent<RectTransform>();
        RectTransform currentRect = currentFruit.GetComponent<RectTransform>();

        // Ustawiamy pozycję startową nowego owocu (np. Y = 100)
        nextRect.anchoredPosition = new Vector2(0, fruitHeight);

        // Upewniamy się, że stary jest na środku (Y = 0)
        currentRect.anchoredPosition = Vector2.zero;

        // 2. Animacja (Lerp) - przesuwamy oba w dół jednocześnie
        float elapsed = 0f;
        while (elapsed < timeForStep)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / timeForStep; // Postęp od 0 do 1

            // Używamy SmoothStep dla ładniejszego ruchu (zwalnia na początku i końcu ruchu)
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Stary idzie: 0 -> -100 (w dół poza ekran)
            currentRect.anchoredPosition = Vector2.Lerp(Vector2.zero, new Vector2(0, -fruitHeight), smoothT);

            // Nowy idzie: 100 -> 0 (z góry na środek)
            nextRect.anchoredPosition = Vector2.Lerp(new Vector2(0, fruitHeight), Vector2.zero, smoothT);

            yield return null; // Czekamy na następną klatkę
        }

        // 3. Sprzątanie
        Destroy(currentFruit); // Usuwamy ten, który zjechał na dół
        currentFruit = nextFruit; // Nowy staje się aktualnym
    }
}