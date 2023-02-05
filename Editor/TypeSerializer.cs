using System;
using System.IO;
using UnityEngine;

namespace OverrideEditors.Editor
{
    internal static class TypeSerializer
    {
        public static bool TryGetType(string data, out Type type)
        {
            if (string.IsNullOrEmpty(data))
            {
                type = null;
                return false;
            }

            bool result;
            try
            {
                var bytes = Convert.FromBase64String(data);

                using var ms = new MemoryStream(bytes);
                using var br = new BinaryReader(ms);

                var genericArgumentsCount = int.Parse(br.ReadString());
                var typeName = br.ReadString();
                type = Type.GetType(typeName);

                if (genericArgumentsCount == 0)
                    return true;

                var genericArguments = new Type[genericArgumentsCount];
                for (var i = 0; i < genericArgumentsCount; i++)
                    if (TryGetType(br.ReadString(), out var genericType))
                        genericArguments[i] = genericType;

                type = type?.MakeGenericType(genericArguments);

                result = type != null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                type = null;
                result = false;
            }

            return result;
        }

        public static bool TryGetData(Type type, out string data)
        {
            if (type == null)
            {
                data = null;
                return false;
            }

            try
            {
                Type[] genericArguments;
                (type, genericArguments) = type.IsConstructedGenericType
                    ? (type.GetGenericTypeDefinition(), type.GetGenericArguments())
                    : (type, null);

                using var ms = new MemoryStream();
                using var bw = new BinaryWriter(ms);

                var genericArgumentsCount = genericArguments?.Length ?? 0;

                bw.Write(genericArgumentsCount.ToString());
                if (string.IsNullOrEmpty(type.AssemblyQualifiedName))
                    throw new InvalidDataException($"Assembly qualified name for type {type} is null or empty!");

                bw.Write(type.AssemblyQualifiedName);

                if (genericArguments != null)
                    for (var i = 0; i < genericArgumentsCount; i++)
                        if (TryGetData(genericArguments[i], out var genericTypeData))
                            bw.Write(genericTypeData);

                data = Convert.ToBase64String(ms.ToArray());
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                data = null;
                return false;
            }
        }
    }
}