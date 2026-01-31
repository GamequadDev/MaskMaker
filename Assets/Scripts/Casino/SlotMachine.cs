using System.Collections;
using TMPro; // Wymagane do obsługi TextMeshPro
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    [Header("Elementy UI")]
    // Tablica przechowująca skrypty trzech bębnów
    public SlotReel[] reels;
    public Button spinButton;
    public TextMeshProUGUI resultText;

    [Header("Ustawienia Gry")]
    // Czas kręcenia pierwszego bębna (kolejne będą kręcić się dłużej)
    public float baseSpinDuration = 2.0f;
    // Opóźnienie między startem kolejnych bębnów (efekt kaskady)
    public float delayBetweenReels = 0.3f;

    // Funkcja podpinana pod przycisk SPIN
    public void OnSpinButtonClick()
    {
        // Dodatkowe zabezpieczenie, żeby nie klikać w trakcie kręcenia
        if (spinButton.interactable)
        {
            StartCoroutine(StartSpinningRoutine());
        }
    }

    // Główna korutyna zarządzająca procesem gry
    private IEnumerator StartSpinningRoutine()
    {
        // 1. Przygotowanie UI
        spinButton.interactable = false; // Blokujemy przycisk
        resultText.text = "Waiting...";
        resultText.color = Color.yellow;

        // 2. Dynamiczne sprawdzenie liczby owoców
        // Pytamy pierwszy bęben, ile ma prefabów w tablicy.
        // Dzięki temu kod sam wie, czy losować 0-4, 0-6 itp.
        int fruitCount = reels[0].fruitPrefabs.Length;

        if (fruitCount == 0)
        {
            Debug.LogError("BŁĄD KRYTYCZNY: Nie przypisałeś prefabów owoców w skrypcie SlotReel!");
            spinButton.interactable = true;
            yield break; // Przerywamy korutynę
        }

        // 3. Losowanie wyników (Logika)
        // Losujemy 3 liczby (indeksy owoców), które mają wypaść na końcu
        int result1 = Random.Range(0, fruitCount);
        int result2 = Random.Range(0, fruitCount);
        int result3 = Random.Range(0, fruitCount);

        // Opcjonalnie: Debug w konsoli, żeby widzieć co wylosowało
        // Debug.Log($"Wylosowano: [{result1}] - [{result2}] - [{result3}]");

        // 4. Uruchamianie Bębnów (Wizualizacja)

        // Start bębna 1
        StartCoroutine(reels[0].Spin(result1, baseSpinDuration));
        yield return new WaitForSeconds(delayBetweenReels); // Czekamy chwilę

        // Start bębna 2 (kręci się trochę dłużej)
        StartCoroutine(reels[1].Spin(result2, baseSpinDuration + 0.5f));
        yield return new WaitForSeconds(delayBetweenReels);

        // Start bębna 3. WAŻNE: Tutaj używamy 'yield return StartCoroutine'.
        // To sprawia, że Manager CZEKA, aż trzeci bęben całkowicie skończy się kręcić,
        // zanim przejdzie do sprawdzania wyniku.
        yield return StartCoroutine(reels[2].Spin(result3, baseSpinDuration + 1.0f));

        // 5. Sprawdzenie Wyniku
        CheckWin(result1, result2, result3);

        // Odblokowujemy przycisk do ponownej gry
        spinButton.interactable = true;
    }

    // Funkcja porównująca wyniki
    private void CheckWin(int r1, int r2, int r3)
    {
        // Warunek wygranej: wszystkie trzy indeksy muszą być identyczne
        if (r1 == r2 && r2 == r3)
        {
            resultText.text = "MEGA BIG WIN!!!";
            resultText.color = Color.green;
            // Tutaj możesz dodać dźwięk wygranej lub efekt cząsteczkowy
        }
        else
        {
            resultText.text = "Try again!";
            resultText.color = Color.white;
        }
    }
}