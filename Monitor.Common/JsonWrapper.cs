using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Monitor.Common
{
 
    /// <summary>
    /// Обертка над json логом, помогает формировать нужные строки для логов, месенджеров, 
    /// </summary>
    public class JsonWrapper
    {
        readonly JObject? jObj;
        public JsonWrapper(string json)
        {
            jObj = JObject.Parse(json);
        }

        public string GetValue(string key)
        {
            var jp =  jObj?.Property(key) ?? null;
            return jp?.Value.ToString() ?? string.Empty;
        }
        

        public List<Nullable<T>> GetValuesFromJArray1<T>(string arrayName, string propertyName) where T : struct, IConvertible
        {          
            
            if (jObj == null)
                return new List<Nullable<T>>();
            var details = jObj[arrayName];
            if(details.IsNullOrEmpty())
                return new List<Nullable<T>>();

            try
            {
                var values = details.Select(x => x.SelectToken(propertyName)).ToList();
                return values.Where(x=> x != null).Select(x =>(Nullable<T>)Convert.ChangeType(x, typeof(T))).ToList();
            }
            catch
            {                
                return new List<Nullable<T>>();
            }            
        }
        public List<T> GetValuesFromJArray<T>(string arrayName, string propertyName) where T :  IConvertible
        {          
            
            if (jObj == null)
                return new List<T>();
            var details = jObj[arrayName];
            if(details.IsNullOrEmpty())
                return new List<T>();

            try
            {
                var values = details.Select(x => x.SelectToken(propertyName)).ToList();
                return values.Where(x=> x != null).Select(x =>(T)Convert.ChangeType(x, typeof(T))).ToList();
            }
            catch
            {                
                return new List<T>();
            }            
        }

        public IEnumerable<Nullable<T>> GetValuesFromIEnumerable1<T>(string arrayName, string propertyName) where T : struct, IConvertible
        {
            Nullable<T> result = default(T);
            if (jObj == null)
                yield break;
            var details = jObj[arrayName];
            if (details.IsNullOrEmpty())
                yield break;
            foreach (var val in details.Select(x => x.SelectToken(propertyName)))
            {                
                try
                {
                    result = (Nullable<T>)Convert.ChangeType(val, typeof(T));
                }
                catch 
                {
                    result = default(T);
                }
                yield return result;
            }
        }

        public IEnumerable<T> GetValuesFromIEnumerable<T>(string arrayName, string propertyName) where T :  IConvertible
        {
            T result = default(T);
            if (jObj == null)
                yield break;
            var details = jObj[arrayName];
            if (details.IsNullOrEmpty())
                yield break;
            foreach (var val in details.Select(x => x.SelectToken(propertyName)))
            {
                try
                {
                    result = (T)Convert.ChangeType(val, typeof(T));
                }
                catch
                {
                    result = default(T);
                }
                yield return result;
            }
        }
    }
}