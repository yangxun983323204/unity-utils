using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace YX.Diagnostics
{
    public class Assert
    {
        public static void Catch<T>(Type type,string method, params object[] args)
            where T : System.Exception
        {
            if (!Application.isEditor)
                return;

            var minfo = GetMethod(type,method,args);
            Catch<T>(null, minfo, args);
        }

        public static void Catch<T>(object inst, string method, params object[] args)
            where T : System.Exception
        {
            if (!Application.isEditor)
                return;

            var minfo = GetMethod(inst.GetType(),method, args);
            Catch<T>(inst, minfo, args);
        }

        public static void Catch<T>(object inst, MethodInfo minfo, params object[] args)
            where T : System.Exception
        {
            if (!Application.isEditor)
                return;

            try
            {
                minfo.Invoke(inst, args);
            }
            catch (Exception e)
            {
                if (e.InnerException is T)
                    return;
                else
                    throw e.InnerException != null ? e.InnerException : e;
            }
        }

        public static void Catch<T>(Action action) where T : System.Exception
        {
            if (!Application.isEditor)
                return;

            try
            {
                action.Invoke();
            }
            catch (T)
            {
            }
        }

        public static void ExpectException<T>(Type type, string method, params object[] args)
            where T : System.Exception
        {
            if (!Application.isEditor)
                return;

            var minfo = GetMethod(type, method, args);
            ExpectException<T>(null, minfo, args);
        }

        public static void ExpectException<T>(object inst, string method, params object[] args)
            where T : System.Exception
        {
            if (!Application.isEditor)
                return;

            var minfo = GetMethod(inst.GetType(), method, args);
            ExpectException<T>(inst, minfo, args);
        }

        public static void ExpectException<T>(object inst, MethodInfo minfo, params object[] args)
            where T : System.Exception
        {
            if (!Application.isEditor)
                return;

            try
            {
                minfo.Invoke(inst, args);
                throw new System.NotImplementedException("期望产生异常:" + typeof(T).ToString());
            }
            catch (Exception e)
            {
                if (e.InnerException is T)
                    return;
                else
                    throw e.InnerException != null ? e.InnerException : e;
            }
        }

        public static void ExpectException<T>(Action action) where T : System.Exception
        {
            if (!Application.isEditor)
                return;

            try
            {
                action.Invoke();
                throw new System.NotImplementedException("期望产生异常:" + typeof(T).ToString());
            }
            catch (T)
            {
            }
        }

        private static MethodInfo GetMethod(Type type,string method,params object[] args)
        {
            Type[] argsType = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                argsType[i] = args[i].GetType();
            }

            var minfo = type.GetMethod(
                method,
                BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance|BindingFlags.Static,
                null, argsType,null);

            if (minfo == null)
            {
                StringBuilder sb = new StringBuilder(20);
                sb.Append($"找不到方法：{type.ToString()}.{method}(");
                foreach (var a in argsType)
                {
                    sb.Append(a.ToString());
                    sb.Append(',');
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(')');
                throw new System.NotSupportedException(sb.ToString());
            }
            return minfo;
        }
    }
}
