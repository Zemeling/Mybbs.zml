using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace My.Common.Exceptions.Extension
{
    public static class TypeExtensions
    {
        public static bool IsNumeric(this Type type)
        {
            return type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(sbyte) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong) || type == typeof(decimal) || type == typeof(double) || type == typeof(float);
        }

        public static string ToDescription(this MemberInfo member, bool inherit = false)
        {
            DescriptionAttribute desc = member.GetAttribute<DescriptionAttribute>(inherit);
            return (desc == null) ? null : desc.Description;
        }

        public static bool AttributeExists<T>(this MemberInfo memberInfo, bool inherit) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherit).Any((object m) => (T)(m as T) != null);
        }

        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit) where T : Attribute
        {
            return (T)(memberInfo.GetCustomAttributes(typeof(T), inherit).SingleOrDefault() as T);
        }

        public static T[] GetAttributes<T>(this MemberInfo memberInfo, bool inherit) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherit).Cast<T>().ToArray();
        }

        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != (Type)null && toCheck != typeof(object))
            {
                Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static bool IsSubclassOfRawClass(this Type toCheck, Type generic)
        {
            if (toCheck.IsSameOrSubclass(generic))
            {
                return true;
            }
            if (toCheck.IsSubclassOfRawGeneric(generic))
            {
                return true;
            }
            Type genRawClass = TypeExtensions.GetRawClassOfGenericType(generic);
            if (genRawClass == (Type)null)
            {
                return false;
            }
            Type curGenericDef = generic.IsGenericType ? generic.GetGenericTypeDefinition() : generic;
            while (toCheck != (Type)null && toCheck != typeof(object))
            {
                Type curDef = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                Type chkRawClass = TypeExtensions.GetRawClassOfGenericType(toCheck);
                if (curDef.IsSubclassOfRawGeneric(curGenericDef) && chkRawClass != (Type)null && chkRawClass.IsSameOrSubclass(genRawClass))
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        private static Type GetRawClassOfGenericType(Type generic)
        {
            if (!generic.IsGenericType)
            {
                return null;
            }
            if (generic.GetGenericArguments().Any())
            {
                return generic.GetGenericArguments().FirstOrDefault();
            }
            return null;
        }

        public static bool IsSameOrSubclass(this Type potentialDescendant, Type potentialBase)
        {
            return potentialDescendant.IsSubclassOf(potentialBase) || potentialDescendant == potentialBase;
        }

        public static bool InheritsOrImplements(this Type child, Type parent)
        {
            parent = TypeExtensions.ResolveGenericTypeDefinition(parent);
            Type currentChild = child.IsGenericType ? child.GetGenericTypeDefinition() : child;
            bool result;
            while (true)
            {
                if (currentChild != typeof(object))
                {
                    if (parent == currentChild || TypeExtensions.HasAnyInterfaces(parent, currentChild))
                    {
                        result = true;
                        break;
                    }
                    currentChild = ((currentChild.BaseType != (Type)null && currentChild.BaseType.IsGenericType) ? currentChild.BaseType.GetGenericTypeDefinition() : currentChild.BaseType);
                    if (currentChild == (Type)null)
                    {
                        return false;
                    }
                    continue;
                }
                return false;
            }
            return result;
        }

        private static bool HasAnyInterfaces(Type parent, Type child)
        {
            return child.GetInterfaces().Any(delegate (Type childInterface)
            {
                Type left = childInterface.IsGenericType ? childInterface.GetGenericTypeDefinition() : childInterface;
                return left == parent;
            });
        }

        private static Type ResolveGenericTypeDefinition(Type parent)
        {
            bool shouldUseGenericType = true;
            if (parent.IsGenericType && parent.GetGenericTypeDefinition() != parent)
            {
                shouldUseGenericType = false;
            }
            if (parent.IsGenericType && shouldUseGenericType)
            {
                parent = parent.GetGenericTypeDefinition();
            }
            return parent;
        }
    }

}
