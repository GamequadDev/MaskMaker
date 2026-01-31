using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    [Header("Elementy UI")]
    public SlotReel[] reels; // Pamiętaj, aby przypisać tu 3 obiekty z nowym skryptem SlotReel
    public Button spinButton;
    public TextMeshProUGUI resultText;

    public void OnSpinButtonClick()
    {
        // Zabezpieczenie: sprawdzamy czy gra nie toczy się już (choć guzik jest wyłączany, warto mieć double-check)
        if (spinButton.interactable)
        {
            StartCoroutine(StartSpinning());
        }
    }

    private IEnumerator StartSpinning()
    {
        spinButton.interactable = false;
        resultText.text = "Losowanie...";

        // 1. POBIERANIE LICZBY OWOCÓW AUTOMATYCZNIE
        // Zakładamy, że wszystkie bębny mają tyle samo owoców, więc pytamy pierwszego o długość tablicy.
        // Dzięki temu nie musisz zmieniać kodu, gdy dodasz więcej grafik w Inspektorze.
        int fruitCount = reels[0].fruitPrefabs.Length;

        if (fruitCount == 0)
        {
            Debug.LogError("Błąd! Nie przypisałeś prefabów owoców w skrypcie SlotReel!");
            yield break;
        }

        // 2. LOSOWANIE (0 do fruitCount)
        int res1 = Random.Range(0, fruitCount);
        int res2 = Random.Range(0, fruitCount);
        int res3 = Random.Range(0, fruitCount);

        // Debug dla pewności co wylosowało
        // Debug.Log($"Wylosowano: {res1} | {res2} | {res3}");

        // 3. URUCHAMIANIE BĘBNÓW (Kaskada)
        // Czas trwania (duration) musi być zsynchronizowany z logiką
        StartCoroutine(reels[0].Spin(res1, 2.0f));
        yield return new WaitForSeconds(0.2f); // Małe opóźnienie dla efektu

        StartCoroutine(reels[1].Spin(res2, 2.5f));
        yield return new WaitForSeconds(0.2f);

        // Czekamy na zakończenie ostatniego (najdłuższego) bębna
        yield return StartCoroutine(reels[2].Spin(res3, 3.0f));

        // 4. SPRAWDZANIE WYNIKU
        CheckWin(res1, res2, res3);

        spinButton.interactable = true;
    }

    private void CheckWin(int r1, int r2, int r3)
    {
        if (r1 == r2 && r2 == r3)
        {
            resultText.text = "WYGRANA!";
            resultText.color = Color.green; // Opcjonalnie zmiana koloru
        }
        else
        {
            resultText.text = "SPRÓBUJ PONOWNIE";
            resultText.color = Color.white;
        }
    }
}