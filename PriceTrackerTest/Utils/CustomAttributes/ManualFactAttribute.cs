namespace PriceTrackerTest.Utils.CustomAttributes
{
    public class ManualFactAttribute : FactAttribute
    {
        public ManualFactAttribute()
        {
            if (!TestConfigs.ManualTestsEnabled)
            {
                Skip = "Ручной тест выключен по умолчанию.";
            }
        }

    }
}
