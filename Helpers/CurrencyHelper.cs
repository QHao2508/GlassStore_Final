using System.Globalization;

namespace GlassStore.Helpers
{
    public static class CurrencyHelper
    {
        public static string ToVnd(decimal price)
        {
            return string.Format(new CultureInfo("vi-VN"), "{0:c0}", price);
        }
    }
}