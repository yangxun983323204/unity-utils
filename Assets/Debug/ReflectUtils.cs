using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace YX
{
    public static class ReflectUtils
    {
        static Dictionary<string, Type> _typeDict = new Dictionary<string, Type>();
        static Dictionary<string, PropertyInfo> _propDict = new Dictionary<string, PropertyInfo>();
        static Dictionary<string, MethodInfo> _funcDict = new Dictionary<string, MethodInfo>();

        public static void ClearCache()
        {
            _typeDict.Clear();
            _propDict.Clear();
            _funcDict.Clear();
        }

        public static string GetFullQualifiedName(string assemblyName,string typeFullName)
        {
            return $"{typeFullName}, {assemblyName}";
        }

        public static object New(string fullyQualifiedType, params object[] parameters)
        {
            if (parameters == null)
                parameters = new object[] { };

            var t = Type.GetType(fullyQualifiedType);
            return Activator.CreateInstance(t, parameters);
        }

        public static object GetProperty(string fullyQualifiedType,string prop, object inst, bool isPublic)
        {
            var p = GetProp(fullyQualifiedType, prop, inst, isPublic);
            return p.GetValue(inst);
        }

        public static void SetProperty(string fullyQualifiedType, string prop, object inst, bool isPublic,object val)
        {
            var p = GetProp(fullyQualifiedType, prop, inst, isPublic);
            p.SetValue(inst, val);
        }

        public static object Call(string fullyQualifiedType, string func, object inst, bool isPublic, params object[] parameters)
        {
            var f = GetFunc(fullyQualifiedType, func, inst, isPublic, parameters);
            return f.Invoke(inst, parameters);
        }

        public static T Call<T>(string fullyQualifiedType,string func,object inst,bool isPublic, params object[] parameters)
        {
            var f = GetFunc(fullyQualifiedType, func, inst, isPublic, parameters);
            return (T)f.Invoke(inst, parameters);
        }

        public static Type GetType(string fullyQualifiedType)
        {
            if (!_typeDict.ContainsKey(fullyQualifiedType))
            {
                _typeDict.Add(fullyQualifiedType, Type.GetType(fullyQualifiedType));
            }

            return _typeDict[fullyQualifiedType];
        }

        public static PropertyInfo GetProp(string fullyQualifiedType, string prop, object inst, bool isPublic)
        {
            var key = GetFuncKey(fullyQualifiedType, prop, null);
            if (!_propDict.ContainsKey(key))
            {
                var t = GetType(fullyQualifiedType);
                BindingFlags flag;
                if (inst != null)
                    flag = BindingFlags.Instance;
                else
                    flag = BindingFlags.Static;

                if (isPublic)
                    flag |= BindingFlags.Public;
                else
                    flag |= BindingFlags.NonPublic;

                var p = t.GetProperty(prop, flag);

                _propDict.Add(key, p);
            }

            return _propDict[key];
        }

        public static MethodInfo GetFunc(string fullyQualifiedType,string func,object inst,bool isPublic, params object[] parameters)
        {
            var key = GetFuncKey(fullyQualifiedType,func,parameters);

            if (!_funcDict.ContainsKey(key))
            {
                var t = GetType(fullyQualifiedType);
                BindingFlags flag;
                if (inst != null)
                    flag = BindingFlags.Instance;
                else
                    flag = BindingFlags.Static;

                if (isPublic)
                    flag |= BindingFlags.Public;
                else
                    flag |= BindingFlags.NonPublic;

                var f = t.GetMethod(func, flag, null, GetTypes(parameters), null);

                _funcDict.Add(key, f);
            }

            return _funcDict[key];
        }

        static string GetFuncKey(string qualified, string func, params object[] parameters)
        {
            var sb = new StringBuilder($"{qualified}.{func}");
            sb.Append("(");
            if (parameters!=null && parameters.Length>0)
            {
                
                foreach (var i in parameters)
                {
                    sb.Append(i.GetType().FullName);
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append(")");

            return sb.ToString();
        }

        static Type[] GetTypes(params object[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                var types = new Type[parameters.Length];
                int idx = 0;
                foreach (var i in parameters)
                {
                    types[idx] = i.GetType();
                    idx++;
                }

                return types;
            }
            else
                return new Type[] { };
        }
    }
}
