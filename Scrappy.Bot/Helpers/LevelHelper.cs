namespace Scrappy.Bot.Helpers;

public static class LevelHelper
{
    public static int CalculateLevelForXp (long totalXp)
    {
        // You need at least 155 xp for level 1
        if (totalXp < 155) return 0;
        
        // ABC formula for 5L^2 + 50L + (100 - totalXp) = 0
        double discriminant = 2500 - 20 * (100d - totalXp); // Calculate with double to prevent overflow
        double result = (-50 + Math.Sqrt(discriminant)) / 10;
        
        return (int)Math.Floor(result);
    }

    public static long GetRequiredXpForLevel(int level)
    {
        if (level <= 0) return 0;
        
        // 5L^2 + 50L + 100
        return (long)(5 * Math.Pow(level, 2) + 50 * level + 100);
    }

    public static string GetProgressBar(long currentXp, int currentLevel, int totalBlocks)
    {
        // Get bounds
        long minXp = GetRequiredXpForLevel(currentLevel);
        long maxXp = GetRequiredXpForLevel(currentLevel + 1);
        
        // How much xp do we need to earn inside this level?
        long xpInLevel = currentXp - minXp;
        long xpRequiredForNextLevel = maxXp - minXp;

        double percentage = (double)xpInLevel / xpRequiredForNextLevel;
        percentage = Math.Clamp(percentage, 0.0, 1.0);
        
        // Build the bar
        int filledBlocks = (int)Math.Round(percentage * totalBlocks);
        int emptyBlocks = totalBlocks - filledBlocks;
        
        emptyBlocks = Math.Max(0, emptyBlocks);
        
        return $"[{new string('█', filledBlocks)}{new string('░', emptyBlocks)}] {Math.Round(percentage * 100)}%";
    }
}