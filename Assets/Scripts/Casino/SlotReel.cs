using UnityEngine;
using UnityEngine.UI; // Wymagane dla UI
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class SlotReel : MonoBehaviour
{
    [Header("Konfiguracja")]
    public GameObject[] fruitPrefabs; // Tablica z prefabami owoców (Image)
    public float fruitHeight = 100f;  // Wysokość owocu (musi pasować do UI)
    public float spinSpeed = 0.1f;    // Czas jednego przeskoku (im mniej tym szybciej)

    [Header("Stan początkowy")]
    public GameObject currentFruit;   // Przypisz owoc, który widać na starcie
    public Transform targetT;
    float elapsedTotal = 0f;

    void Start()
    {

        // Zabezpieczenie: jeśli nie przypisano startowego owocu, szukamy pierwszego dziecka
        if (currentFruit == null && transform.childCount > 0)
        {
            currentFruit = transform.GetChild(0).gameObject;
        }
    }

    public IEnumerator Spin(int targetId, float duration)
    {
        

        // PĘTLA GŁÓWNA (Kręcenie losowe)
        // Kręcimy dopóki nie minie czas 'duration'
        while (elapsedTotal < duration)
        {
            // Losujemy byle jaki owoc do animacji przelotowej
            int randomId = Random.Range(0, fruitPrefabs.Length);

            // Wykonujemy jeden cykl "przesunięcia w dół"
            yield return StartCoroutine(MoveOneStep(randomId, spinSpeed));

            elapsedTotal += spinSpeed;
        }

            // FAZA KOŃCOWA (Zatrzymanie na wyniku)
            // Zwalniamy trochę na koniec dla efektu (opcjonalnie)
            yield return StartCoroutine(MoveOneStep(targetId, spinSpeed * 2f));

        // Upewniamy się, że finalny owoc jest idealnie na środku (korekta błędów float)
        currentFruit.GetComponent<Transform>().position = targetT.position;
    }

    // Ta funkcja odpowiada za stworzenie nowego owocu i przesunięcie obu w dół
    private IEnumerator MoveOneStep(int nextFruitId, float timeForStep)
    {
        // 1. Tworzymy nowy owoc NAD aktualnym
        GameObject nextFruit = Instantiate(fruitPrefabs[nextFruitId], transform);
        Transform nextRect = nextFruit.GetComponent<Transform>();
        Transform currentRect = currentFruit.GetComponent<Transform>();

        // Ustawiamy pozycję startową nowego owocu (na górze)
        nextRect.position = new Vector2(targetT.position.x, fruitHeight);

        // Upewniamy się, że stary jest na środku
        currentRect.position = targetT.position;

        // 2. Animacja (Lerp) - przesuwamy oba w dół
        float elapsed = 0f;
        while (elapsed < timeForStep)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / timeForStep;

            // Prosty easing (wygląda płynniej niż liniowy ruch)
            // Jeśli chcesz idealnie liniowo, użyj po prostu: float progress = t;
            float progress = t;

            // Stary idzie: 0 -> -100 (w dół poza ekran)
            currentRect.position = Vector2.Lerp(Vector2.zero, new Vector2(0, -fruitHeight), progress);

            // Nowy idzie: 100 -> 0 (z góry na środek)
            nextRect.position = Vector2.Lerp(new Vector2(0, fruitHeight), Vector2.zero, progress);

            yield return null;
        }

        // 3. Sprzątanie po ruchu
        Destroy(currentFruit); // Usuwamy ten co zjechał na dół
        currentFruit = nextFruit; // Nowy staje się aktualnym
    }
}