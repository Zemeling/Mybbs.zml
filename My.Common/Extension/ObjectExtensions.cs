using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace My.Common.Extension
{
    public static class ObjectExtensions
    {
        public static T CastTo<T>(this object value)
        {
            Type type = typeof(T);
            object result;
            try
            {
                result = ((!type.IsEnum) ? ((!(type == typeof(Guid))) ? Convert.ChangeType(value, type) : ((object)Guid.Parse(value.ToString()))) : Enum.Parse(type, value.ToString()));
            }
            catch
            {
                result = default(T);
            }
            return (T)result;
        }

        public static T CastTo<T>(this object value, T defaultValue)
        {
            Type type = typeof(T);
            object result;
            try
            {
                result = (type.IsEnum ? Enum.Parse(type, value.ToString()) : Convert.ChangeType(value, type));
            }
            catch
            {
                result = defaultValue;
            }
            return (T)result;
        }

        public static bool IsDate(this object date)
        {
            if (date.IsNullOrEmpty())
            {
                return false;
            }
            string strdate = date.ToString();
            DateTime dateTime;
            try
            {
                dateTime = Convert.ToDateTime(date);
                date = dateTime.ToString("d");
                return true;
            }
            catch
            {
                if (strdate.Length == 8)
                {
                    string year4 = strdate.Substring(0, 4);
                    string month3 = strdate.Substring(4, 2);
                    string day = strdate.Substring(6, 2);
                    if (Convert.ToInt32(year4) < 1900 || Convert.ToInt32(year4) > 2100)
                    {
                        return false;
                    }
                    if (Convert.ToInt32(month3) > 12 || Convert.ToInt32(day) > 31)
                    {
                        return false;
                    }
                    dateTime = Convert.ToDateTime(year4 + "-" + month3 + "-" + day);
                    date = dateTime.ToString("d");
                    return true;
                }
                if (strdate.Length == 6)
                {
                    string year4 = strdate.Substring(0, 4);
                    string month3 = strdate.Substring(4, 2);
                    if (Convert.ToInt32(year4) < 1900 || Convert.ToInt32(year4) > 2100)
                    {
                        return false;
                    }
                    if (Convert.ToInt32(month3) > 12)
                    {
                        return false;
                    }
                    dateTime = Convert.ToDateTime(year4 + "-" + month3);
                    date = dateTime.ToString("d");
                    return true;
                }
                if (strdate.Length == 5)
                {
                    string year4 = strdate.Substring(0, 4);
                    string month3 = strdate.Substring(4, 1);
                    if (Convert.ToInt32(year4) < 1900 || Convert.ToInt32(year4) > 2100)
                    {
                        return false;
                    }
                    date = year4 + "-" + month3;
                    return true;
                }
                if (strdate.Length == 4)
                {
                    string year4 = strdate.Substring(0, 4);
                    if (Convert.ToInt32(year4) < 1900 || Convert.ToInt32(year4) > 2100)
                    {
                        return false;
                    }
                    dateTime = Convert.ToDateTime(year4);
                    date = dateTime.ToString("d");
                    return true;
                }
                return false;
            }
        }

        public static bool IsDecimal(this object obj)
        {
            try
            {
                Convert.ToDecimal(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsInt(this object obj)
        {
            try
            {
                Convert.ToInt32(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsBoolean(this object obj)
        {
            try
            {
                Convert.ToBoolean(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNullOrEmpty(this object data)
        {
            if (data == null)
            {
                return true;
            }
            if (data.GetType() == typeof(string) && string.IsNullOrEmpty(data.ToString().Trim()))
            {
                return true;
            }
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }
            if (data is IEnumerable)
            {
                return ((IEnumerable)data).AsQueryable().Count() == 0;
            }
            return false;
        }

        public static bool ObjToBool(this object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.Equals(DBNull.Value))
            {
                return false;
            }
            bool flag = default(bool);
            return bool.TryParse(obj.ToString(), out flag) && flag;
        }

        public static DateTime? ObjToDateNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        public static DateTime ObjToDate(this object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj is null.");
            }
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
        }

        public static int ObjToInt(this object obj)
        {
            if (obj != null)
            {
                if (obj.Equals(DBNull.Value))
                {
                    return 0;
                }
                int num = default(int);
                if (int.TryParse(obj.ToString(), out num))
                {
                    return num;
                }
            }
            return 0;
        }

        public static long ObjToLong(this object obj)
        {
            if (obj != null)
            {
                if (obj.Equals(DBNull.Value))
                {
                    return 0L;
                }
                long num = default(long);
                if (long.TryParse(obj.ToString(), out num))
                {
                    return num;
                }
            }
            return 0L;
        }

        public static int? ObjToIntNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.Equals(DBNull.Value))
            {
                return null;
            }
            return obj.ObjToInt();
        }

        public static string ObjToStr(this object obj)
        {
            if (obj == null)
            {
                return "";
            }
            if (obj.Equals(DBNull.Value))
            {
                return "";
            }
            return Convert.ToString(obj);
        }

        public static decimal ObjToDecimal(this object obj)
        {
            if (obj == null)
            {
                return 0m;
            }
            if (obj.Equals(DBNull.Value))
            {
                return 0m;
            }
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0m;
            }
        }

        public static decimal? ObjToDecimalNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.Equals(DBNull.Value))
            {
                return null;
            }
            return obj.ObjToDecimal();
        }

        public static bool ObjIsInt(this object number)
        {
            if (number.IsNullOrEmpty())
            {
                return false;
            }
            string strNum = number.ToString().Trim();
            string pattern = "^[0-9]+[0-9]*$";
            return Regex.IsMatch(strNum, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsValidInput(this object objInput)
        {
            try
            {
                if (objInput.IsNullOrEmpty())
                {
                    return false;
                }
                string input2 = objInput.ToString();
                input2 = input2.Replace("'", "''").Trim();
                string testString = "and |or |exec |insert |select |delete |update |count |chr |mid |master |truncate |char |declare ";
                string[] testArray = testString.Split('|');
                string[] array = testArray;
                foreach (string testStr in array)
                {
                    if (input2.ToLower().IndexOf(testStr) != -1)
                    {
                        input2 = "";
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static T DeepClone<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Position = 0L;
                return (T)bf.Deserialize(ms);
            }
        }
    }

}
