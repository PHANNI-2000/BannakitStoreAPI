using Newtonsoft.Json.Converters;
using System.Globalization;

namespace BannakitStoreApi
{
    public static class JsonConverterExtension
    {
        public class CustomDateConverter : IsoDateTimeConverter
        {
            public CustomDateConverter()
            {
                base.DateTimeFormat = "dd/MM/yyyy";
                base.Culture = CultureInfo.InvariantCulture;
            }
        }

        public class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "dd/MM/yyyy HH:mm";
                base.Culture = CultureInfo.InvariantCulture;
            }
        }

        public class CustomDateTimeSecConverter : IsoDateTimeConverter
        {
            public CustomDateTimeSecConverter()
            {
                base.DateTimeFormat = "dd/MM/yyyy HH:mm:ss";
                base.Culture = CultureInfo.InvariantCulture;
            }
        }
    }
}
