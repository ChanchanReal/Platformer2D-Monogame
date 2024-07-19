using System;

namespace Fallen_Knight.src.Score
{
    public static class Score
    {
        private static int score = 0;

        public static void AddScore(int addScore)
        {
            score += addScore;
        }
        public static int GetScore() => score;

        public static void SubtractScore(int newScore)
        {
            score -= newScore;
            score = Math.Clamp(score , 0, int.MaxValue);
        }
    }
}
