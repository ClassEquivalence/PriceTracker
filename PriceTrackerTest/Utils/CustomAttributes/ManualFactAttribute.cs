using PriceTrackerTest.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
