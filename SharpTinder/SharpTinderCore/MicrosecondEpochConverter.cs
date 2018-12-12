using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SharpTinder
{
    public class MicrosecondEpochConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTime)value - _epoch).TotalMilliseconds + "000");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return _epoch.AddMilliseconds(long.Parse(reader.Value.ToString()) / 1000d);
        }
    }

    public class MsOrDTSConverter : DateTimeConverterBase {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteRawValue(((DateTime)value - _epoch).TotalMilliseconds + "000");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            object o = reader.Value;
            if (o == null) { return null; }

            DateTime dt = DateTime.MinValue;
            long l = -1;
            string s = "";
            if(o.GetType() == typeof(DateTime)) {
                return (DateTime)o;
            }
            if(o.GetType() == typeof(DateTimeOffset)) {
                DateTimeOffset dto = (DateTimeOffset)o;
                return dto.Date;
            }
            if(o.GetType() == typeof(string)) {
                // Is a string
                s = (string)o;
                if (!long.TryParse(s, out l)) {
                    // String isn't a long, but may be a DateTime
                    if (!DateTime.TryParse(s, out dt)) {
                        // String wasn't a datetime, we don't know what this is so fail it.
                        throw new Exception("Failed to convert the datetime given by this json object");
                    }
                    else {
                        // String was a datetime string, return it
                        return dt;
                    }
                }
            }
            else if(o.GetType() == typeof(long) || o.GetType() == typeof(int)) {
                l = (long)o;
            }


            // fall through to long
            dt = new DateTime(l);
            if(dt <= _epoch) {
                // But was an invalid date
                dt = _epoch.AddMilliseconds(l / 1000d);
            }
            return dt;

        }
    }
}
