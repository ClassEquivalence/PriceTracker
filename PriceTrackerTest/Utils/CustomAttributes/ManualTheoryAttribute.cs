namespace PriceTrackerTest.Utils.CustomAttributes
{
    public class ManualTheoryAttribute : TheoryAttribute
    {
        public ManualTheoryAttribute()
        {
            if (!TestConfigs.ManualTestsEnabled)
            {
                Skip = "Ручной тест выключен по умолчанию.";
            }
        }
    }
}
