using SlotGame.Symbols;
using UnityEngine;

namespace SlotGame.Machine
{
    public class SlotScoreEvaluator
    {
        private SlotMachine machine;
        private bool allowWilds;

        public SlotScoreEvaluator(SlotMachine machine, bool allowWilds = true)
        {
            this.machine = machine;
            this.allowWilds = allowWilds;
        }

        public int EvaluateScore()
        {
            int totalScore = 0;
            var matrix = machine.Matrix;
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);

            // Loop through each row
            for (int x = 0; x < rows; x++)
            {
                int streak = 1;
                SymbolType lastSymbol = matrix[x, 0].CurrentSymbol;

                for (int y = 1; y < cols; y++)
                {
                    var currentSymbol = matrix[x, y].CurrentSymbol;

                    if (SymbolsMatch(lastSymbol, currentSymbol))
                    {
                        streak++;
                    }
                    else
                    {
                        totalScore += GetScoreForMatch(lastSymbol, streak);
                        streak = 1;
                        lastSymbol = currentSymbol;
                    }
                }

                // Final streak at row end
                totalScore += GetScoreForMatch(lastSymbol, streak);
            }

            return totalScore;
        }

        private bool SymbolsMatch(SymbolType a, SymbolType b)
        {
            if (a == b) return true;
            if (!allowWilds) return false;
            return a == SymbolType.WILD || b == SymbolType.WILD;
        }

        private int GetScoreForMatch(SymbolType symbol, int streak)
        {
            if (streak < 2) return 0; // Minimum 2-in-a-row required for prize

            var data = MachineController.SymbolsMap.GetData(symbol);
            if (data == null) return 0;

            // Example scoring: base value * streak length
            int score = data.value * streak;

            // Optional: bonus multiplier for longer streaks
            if (streak >= 3) score *= 2;
            if (streak >= 5) score *= 3;
            
            return score;
        }
    }
}
