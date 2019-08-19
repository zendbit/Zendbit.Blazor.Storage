using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;
using Zendbit.Tools.Encryption;

namespace Zendbit.Blazor.Storage
{
    /// <summary>
    /// Local storage class definition
    /// this is inherit from BaseLocalStorage class
    /// </summary>
    public class LocalStorage : BaseLocalStorage, ILocalStorage
    {
        public LocalStorage(IJSRuntime jsRuntime) : base(jsRuntime)
        {
            _storageIdentity = "blazor.components.storage";
            _localStorageType = "localStorage";
        }
    }

    /// <summary>
    /// Local session class definition
    /// this is inherit from BaseLocalStorage class
    /// </summary>
    public class LocalSession : BaseLocalStorage, ILocalSession
    {
        public LocalSession(IJSRuntime jsRuntime) : base(jsRuntime)
        {
            _storageIdentity = "blazor.components.session";
            _localStorageType = "sessionStorage";
        }
    }

    /// <summary>
    /// BaseLocalStorage class definition
    /// this class will be main class for handling data storage and session
    /// Implementation of html5 web storage and session
    /// </summary>
    public abstract class BaseLocalStorage
    {
        private readonly IJSRuntime _jsRuntime;
        protected string _storageIdentity = "blazor.components.storage";
        protected string _localStorageType = "localStorage";

        public string EncryptionKey { get; set; } = "S3cr3t!";

        public BaseLocalStorage(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// This method for clear local storage from the web storage
        /// </summary>
        /// <returns></returns>
        public async Task ClearStorage()
        {
            await _jsRuntime.InvokeAsync<object>(
                    string.Format("{0}.removeItem", _localStorageType),
                    _storageIdentity
                );
        }

        /// <summary>
        /// Deseriaization of data storage
        /// this small helper for deserialize data from the local web storage
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<string, object>> DeserializeStorage()
        {
            var storage =
                await _jsRuntime.InvokeAsync<object>(
                        string.Format("{0}.getItem", _localStorageType),
                        _storageIdentity
                    );

            try
            {
                return JsonSerializer
                    .Deserialize<Dictionary<string, object>>(
                        XORCrypt.New().Decode(
                            storage.ToString(),
                            EncryptionKey
                        )
                    );
            }
            catch (Exception)
            {
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Serialize and write data to storage
        /// this helper will save data to web storage
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        private async Task SerializeStorage(
            Dictionary<string, object> storage
        )
        {
            await _jsRuntime.InvokeAsync<string>(
                    string.Format("{0}.setItem", _localStorageType),
                    _storageIdentity,
                    XORCrypt.New().Encode(
                        JsonSerializer.Serialize(storage),
                        EncryptionKey
                    )
                );
        }

        /// <summary>
        /// Check if data type of object is valid for data storage
        /// </summary>
        /// <param name="type">parameter type of object
        /// this will only accept string, int, bool, uint, double,
        /// float, decimal, and DateTime.</param>
        /// <returns></returns>
        private (bool IsSupported, string ErrMsg) IsTypeSupported(Type type)
        {
            if (
                !new List<Type>
                {
                    typeof(string),
                    typeof(int),
                    typeof(bool),
                    typeof(uint),
                    typeof(double),
                    typeof(float),
                    typeof(decimal),
                    typeof(DateTime)
                }.Contains(type)
            )
            {
                return (false, "Value not supported by local storage.");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// This function will add key value pair into the data
        /// if key already exist data value will overwrite
        /// </summary>
        /// <param name="key">key of data</param>
        /// <param name="value">value of data</param>
        /// <returns></returns>
        public async Task<(bool IsSuccess, string ErrMsg)> Add(
            string key, object value
        )
        {
            var supported = IsTypeSupported(value.GetType());
            if (!supported.IsSupported) return supported;

            var storage = await DeserializeStorage();
            var insertedValue =
                JsonSerializer.Serialize(
                    new KeyValuePair<string, string>(
                        value.ToString(),
                        value.GetType().ToString()
                    )
                );
            if (storage.ContainsKey(key))
                storage[key] = insertedValue;

            else
                storage.Add(key, insertedValue);

            await SerializeStorage(storage);

            return (true, string.Empty);
        }

        /// <summary>
        /// Check if storage contain key
        /// </summary>
        /// <param name="key">key data tobe checked</param>
        /// <returns></returns>
        public async Task<bool> Contain(string key)
        {
            return (
                await GetJson(key)
            ).IsExist;
        }

        /// <summary>
        /// This helper will convert from json string to json element
        /// </summary>
        /// <param name="key">key to be check</param>
        /// <returns>return value is tupple
        /// (IsExist, JsonElement) IsExist will true if key found</returns>
        private async Task<(bool IsExist, JsonElement JsonElement)> GetJson(
            string key
        )
        {
            var storage = await DeserializeStorage();
            if (
                storage.TryGetValue(key, out object value)
                && value is JsonElement jsonElement
            )
            {
                return (true, jsonElement);
            }

            return (false, new JsonElement());
        }

        /// <summary>
        /// Remove key value from data with given key
        /// </summary>
        /// <param name="key">key to be remove</param>
        /// <returns>return value is tuple
        /// (IsSuccess, Value) IsSuccess will return true and the dynamic
        /// value will return deleted value.</returns>
        public async Task<(bool IsSuccess, dynamic Value)> Remove(string key)
        {
            var (isSuccess, value) = await GetValue(key);
            var storage = await DeserializeStorage();
            if (
                storage.ContainsKey(key)
                && isSuccess
            )
            {
                storage.Remove(key);
                await SerializeStorage(storage);
                return (true, value);
            }

            return (false, null);
        }

        /// <summary>
        /// Get value with given key
        /// </summary>
        /// <param name="key">key data to be retrieve</param>
        /// <returns>return value is tuple
        /// (IsSuccess, Value) IsSuccess will true if get value success
        /// and will return the data value.</returns>
        public async Task<(bool IsSuccess, dynamic Value)> GetValue(
            string key
        )
        {
            var (isExist, jsonElement) = await GetJson(key);
            if (isExist)
            {
                var valueMap =
                    JsonSerializer.Deserialize<KeyValuePair<string, string>>(
                        jsonElement.GetString()
                    );

                object data = null;

                if (typeof(string).ToString().Equals(valueMap.Value))
                    data = valueMap.Key;

                if (typeof(int).ToString().Equals(valueMap.Value))
                    data = int.Parse(valueMap.Key);

                if (typeof(bool).ToString().Equals(valueMap.Value))
                    data = bool.Parse(valueMap.Key);

                if (typeof(uint).ToString().Equals(valueMap.Value))
                    data = uint.Parse(valueMap.Key);

                if (typeof(double).ToString().Equals(valueMap.Value))
                    data = double.Parse(valueMap.Key);

                if (typeof(float).ToString().Equals(valueMap.Value))
                    data = float.Parse(valueMap.Key);

                if (typeof(decimal).ToString().Equals(valueMap.Value))
                    data = decimal.Parse(valueMap.Key);

                if (typeof(DateTime).ToString().Equals(valueMap.Value))
                    data = DateTime.Parse(valueMap.Key);

                return (true, data);
            }

            return (false, null);
        }
    }

    /// <summary>
    /// This Interface for IBaseStorage Will expose on DepedencyInjection
    /// </summary>
    public interface IBaseStorage
    {
        Task<(bool IsSuccess, string ErrMsg)> Add(string key, object value);
        Task<(bool IsSuccess, dynamic Value)> GetValue(string key);
        Task<(bool IsSuccess, dynamic Value)> Remove(string key);
        Task ClearStorage();
        Task<bool> Contain(string key);
    }

    public interface ILocalStorage : IBaseStorage
    {

    }

    public interface ILocalSession : IBaseStorage
    {

    }

    /// <summary>
    /// Register Service Collection for dependency injection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static void AddStorage(this IServiceCollection col, Action<BaseLocalStorage> setup = null)
        {
            col.AddScoped<ILocalStorage, LocalStorage>(
                (ctx) =>
                {
                    // callback setup
                    var jsRuntime = ctx.GetService<IJSRuntime>();
                    var localStorage = new LocalStorage(jsRuntime);
                    if (!(setup is null))
                        setup(localStorage);
                    return localStorage;
                }
            );

            col.AddScoped<ILocalSession, LocalSession>(
                (ctx) =>
                {
                    // callback setup
                    var jsRuntime = ctx.GetService<IJSRuntime>();
                    var localSession = new LocalSession(jsRuntime);
                    if (!(setup is null))
                        setup(localSession);
                    return localSession;
                }
            );
        }
    }
}
