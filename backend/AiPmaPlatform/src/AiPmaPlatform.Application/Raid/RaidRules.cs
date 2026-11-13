using AiPmaPlatform.Domain.Enums;

namespace AiPmaPlatform.Application.Raid
{
    public static class RaidRules
    {
        // An issue has already happened, so its probability is certain.
        public const int IssueProbability = 5;

        public const string LevelLow = "Low";
        public const string LevelMedium = "Medium";
        public const string LevelHigh = "High";
        public const string LevelCritical = "Critical";

        public static int Score(int impact, int probability) => impact * probability;

        // 5x5 matrix: 1-4 Low, 5-9 Medium, 10-16 High, 20-25 Critical.
        public static string Level(int score) =>
            score >= 20 ? LevelCritical :
            score >= 10 ? LevelHigh :
            score >= 5 ? LevelMedium :
            LevelLow;

        public static bool IsActive(RaidStatus status) =>
            status == RaidStatus.Open || status == RaidStatus.InProgress;

        public static string? Clean(string? text) =>
            string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }
}