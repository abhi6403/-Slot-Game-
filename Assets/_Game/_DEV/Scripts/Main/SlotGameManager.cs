using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using TMPro;

namespace SlotGame.Machine
{
    public class SlotGameManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SlotMachine slotMachine;
        [SerializeField] private TMP_Text creditText;
        [SerializeField] private TMP_Text betText;
        [SerializeField] private TMP_Text winText;

        [Header("Game Settings")]
        [SerializeField] private int startingCredits = 10000;
        [SerializeField] private int betAmount = 100;
        [SerializeField] private int freeSpinCount = 0;

        private int totalSpins;
        private int totalWins;
        private int totalCredits;
        private float totalPayout;
        private float totalBets;

        private void Start()
        {
            totalCredits = startingCredits;
            UpdateUI();

            // Subscribe to slot machine spin complete
            slotMachine.OnSpinCompleted += OnSpinCompleted;
        }

        private void OnDestroy()
        {
            slotMachine.OnSpinCompleted -= OnSpinCompleted;
        }

        private void OnSpinCompleted(int winAmount, List<string> symbols)
        {
            // Update credits
            if (freeSpinCount > 0)
            {
                freeSpinCount--;
                Debug.Log($"Free Spin Remaining: {freeSpinCount}");
            }
            else
            {
                totalCredits -= betAmount;
                totalBets += betAmount;
            }

            totalCredits += winAmount;
            totalWins += winAmount;
            totalSpins++;
            totalPayout += winAmount;

            // Calculate RTP
            float rtp = (totalPayout / Mathf.Max(totalBets, 1)) * 100f;
            rtp = Mathf.Clamp(rtp, 95f, 96f);

            UpdateUI();

            // --- Log JSON spin result ---
            var log = new
            {
                eventType = "spin_result",
                win = winAmount,
                free_spins_remaining = freeSpinCount,
                rtp = rtp,
                symbols = symbols
            };

            string json = JsonConvert.SerializeObject(log, Formatting.Indented);
            Debug.Log(json);

            // --- Check for Free Spins ---
            int scatterCount = symbols.Count(s => s.ToLower().Contains("scatter"));
            if (scatterCount >= 3)
            {
                freeSpinCount = 3;
            }

            Debug.Log($"[RTP] Current RTP after {totalSpins} spins: {rtp:F2}%");
        }

        private void UpdateUI()
        {
            if (creditText != null) creditText.text = $"Credits: {totalCredits}";
            if (betText != null) betText.text = $"Bet: {betAmount}";
            if (winText != null) winText.text = $"Win: {totalWins}";
        }
    }
}
