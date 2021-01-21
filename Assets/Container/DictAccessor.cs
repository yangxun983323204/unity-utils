using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace YX
{
    /// <summary>
    /// 字典的lazy存取封装
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    class DictAccessor<TKey,TValue>
    {
        public IDictionary<TKey, TValue> Dict { get; set; }
        /// <summary>
        /// 获取字典中key的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public TValue Get(TKey key,TValue def = default(TValue))
        {
            if (Dict == null)
                return def;

            Dict.TryGetValue(key, out def);
            return def;
        }
        public TValue GetOrNew(TKey key,Func<TKey,TValue> ctor)
        {
            if (Dict == null)
                Dict = new Dictionary<TKey, TValue>();

            TValue val;
            if(!Dict.TryGetValue(key, out val))
            {
                val = ctor(key);
                Dict.Add(key, val);
            }

            return val;
        }
        /// <summary>
        /// 设置字典的键和值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Set(TKey key,TValue val)
        {
            if (Dict == null)
                Dict = new Dictionary<TKey, TValue>(4);

            if (Dict.ContainsKey(key))
                Dict[key] = val;
            else
                Dict.Add(key, val);
        }

        /// <summary>
        /// 移除key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKey key)
        {
            if (Dict == null)
                return;
            else
            {
                if(Dict.ContainsKey(key))
                    Dict.Remove(key);
            }
        }
    }

    class DictAccessor
    {
        public System.Collections.IDictionary Dict { get; set; }
        /// <summary>
        /// 获取字典中key的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public object Get(object key, object def = default(object))
        {
            if (Dict == null)
                return def;
            if (Dict.Contains(key))
                def = Dict[key];
            return def;
        }
        public object GetOrNew(object key, Func<object, object> ctor)
        {
            if (Dict == null)
                Dict = new Dictionary<object, object>();

            object val;
            if (Dict.Contains(key))
            {
                val = Dict[key];
            }
            else
            {
                val = ctor(key);
                Dict.Add(key, val);
            }

            return val;
        }
        /// <summary>
        /// 设置字典的键和值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Set(object key, object val)
        {
            if (Dict == null)
                Dict = new Dictionary<object,object>(4);

            if (Dict.Contains(key))
                Dict[key] = val;
            else
                Dict.Add(key, val);
        }

        /// <summary>
        /// 移除key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(object key)
        {
            if (Dict == null)
                return;
            else
            {
                if (Dict.Contains(key))
                    Dict.Remove(key);
            }
        }
    }
}
