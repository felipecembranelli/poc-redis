using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace redis_poc
{
    internal class RedisHelper
    {

        protected IDatabase RedisDatabase { get; }
        protected ConnectionMultiplexer Connection { get; }

        public RedisHelper(string redisServer, string password = "", int redisDatabaseId = 0)
        {
            var config = new ConfigurationOptions
            {
                EndPoints = { redisServer },
                Password = password ?? "",
                DefaultDatabase = redisDatabaseId,
                AllowAdmin = true,
                SyncTimeout = 20000,
            };
            Connection = ConnectionMultiplexer.Connect(config);
            RedisDatabase = Connection.GetDatabase(redisDatabaseId);
        }

        #region Collection

        public bool SetCollection<T>(string key, IEnumerable<(string field, T value)> cacheObject, TimeSpan expiryTime)
        {
            RedisDatabase.HashSet(
                key,
                cacheObject
                    .Select(itm => new HashEntry(itm.field, JsonConvert.SerializeObject(itm.value)))
                    .ToArray());
            return RedisDatabase.KeyExpire(key, expiryTime);
        }

        public IEnumerable<T> GetCollection<T>(string key)
        {
            var result = RedisDatabase.HashGetAll(key);
            if (result.Length > 0)
            {
                var items = result.Where(e => e.Value.HasValue)
                  .Select(e => JsonConvert.DeserializeObject<T>(e.Value))
                  .ToList();

                return items;
            }
            return Array.Empty<T>(); ;
        }

        public IEnumerable<T> GetItemsFromCollection<T>(string key, params string[] fields)
        {
            var hashResult = RedisDatabase.HashGet(
                key,
                fields.Select(key => (RedisValue)key).ToArray());

            var items = hashResult.Where(e => e.HasValue)
                .Select(e => JsonConvert.DeserializeObject<T>(e))
                .ToList();

            return items;
        }

        #endregion


        #region String

        public void StringSet(string cacheKey, string value)
        {
            RedisDatabase.StringSet(cacheKey, value, TimeSpan.FromMinutes(10));
        }

        public string StringGet(string cacheKey)
        {
            return RedisDatabase.StringGet(cacheKey);
        }


        #endregion String


        #region Set
        public void Set(string cacheKey, string value)
        {
            RedisDatabase.SetAdd(cacheKey, value);
        }


        /// <summary>
        /// Return a list of set members
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public string[] SetMembers(string cacheKey)
        {
            return RedisDatabase.SetMembers(cacheKey).ToStringArray();
        }

        public bool IsSetMember(string setKey, string memberKey)
        {
            return RedisDatabase.SetContains(setKey, memberKey);
        }

        #endregion

        #region Hash

        public void HashSet<T>(string key, T cacheObject)
        {
            RedisDatabase.HashSet(
                key,
                RedisHelper.ToHashEntries(cacheObject)
                );
        }

        public T HashGet<T>(string key)
        {
            var result = RedisDatabase.HashGetAll(key);

            return RedisHelper.ConvertFromRedis<T>(result);
        }

        #endregion Hash

        public void Close()
        {
            Connection.Close();
        }


        #region Helper

        public static HashEntry[] ToHashEntries(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties
                .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
                .Select
                (
                      property =>
                      {
                          object propertyValue = property.GetValue(obj);
                          string hashValue;

                          // This will detect if given property value is 
                          // enumerable, which is a good reason to serialize it
                          // as JSON!
                          if (propertyValue is IEnumerable<object>)
                          {
                              // So you use JSON.NET to serialize the property
                              // value as JSON
                              hashValue = JsonConvert.SerializeObject(propertyValue);
                          }
                          else
                          {
                              hashValue = propertyValue.ToString();
                          }

                          return new HashEntry(property.Name, hashValue);
                      }
                )
                .ToArray();
        }

        public static T ConvertFromRedis<T>(HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return (T)obj;
        }

        #endregion

    }

}
