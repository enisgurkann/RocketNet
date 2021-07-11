using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace RocketNet
{
    /// <summary>
    /// Genişletilmiş C# methodları
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Değerin null olması durumunda geriye "true" döndürür.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }
        
        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        /// <summary>
        /// object türündeki nesneyi Int32 türüne çevirir.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToInt(this object obj)
        {
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// object türündeki nesneyi Int64 türüne çevirir.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Int64 ToInt64(this object obj)
        {
            return Convert.ToInt64(obj);
        }

        /// <summary>
        /// /object türündeki nesneyi Int16 türüne çevirir.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Int16 ToInt16(this object obj)
        {
            return Convert.ToInt16(obj);
        }

        /// <summary>
        /// object türündeki nesneyi Float türüne çevirir.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float ToFloat(this object obj)
        {
            return Convert.ToSingle(obj);
        }

        /// <summary>
        /// object türündeki nesneyi long türüne çevirir.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ToLong(this object obj)
        {
            return Convert.ToInt64(obj);
        }

        /// <summary>
        /// object türündeki nesneyi double türüne çevirir.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double ToDouble(this object obj)
        {
            return Convert.ToDouble(obj);
        }

        /// <summary>
        /// object türündeki nesneyi boolean türüne çevirir.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ToBoolean(this object obj)
        {
            return Convert.ToBoolean(obj);
        }

        /// <summary>
        /// tam sayı tipindeki nesne eğer 1 veya 1'den büyük bir sayıysa geriye "true" döndürür
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool ToBoolean(this int number)
        {
            if (number > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// object türündeki nesneyi decimal türüne çevirir.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object obj)
        {
            return Convert.ToDecimal(obj);
        }

        /// <summary>
        /// Nokta ile ayrılmış ondalık sayılarda noktayı virgüle çevirir ve decimal türünde geri döndürür.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string text)
        {
            return text.Trim() != "" ? (text.Trim().IndexOf('.') > -1 ? text.Trim().Replace(".", ",").ToDecimal() : 0) : 0;
        }

        /// <summary>
        /// object türündeki nesneyi datetime türüne çevirir.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object obj)
        {
            return !obj.IsNull() ? Convert.ToDateTime(obj) : DateTime.Now;
        }

        /// <summary>
        /// string türündeki veriyi datetime'a çevirir ve belirlenen formatta geriye döndürür.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string text, string dateFormat)
        {
            return DateTime.ParseExact(text, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        /// <summary>
        /// Belirtilen string parametrenin ilk harfini büyük karaktere çevirerek geriye döndürür.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToCapitalize(this string obj)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(obj);
        }

        /// <summary>
        /// Belirtilen string parametre eğer bir tam sayı ise geriye "true" döndürür.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNumber(this string obj)
        {
            return new Regex(@"^-*[0-9,\.]+$").IsMatch(obj);
        }

        public static string BytesToString(long byteCount)
        {
            try
            {
                string[] suf = { " Bayt", " KB", " MB", " GB", " TB", " PB", " EB" };
                if (byteCount == 0)
                    return "0" + suf[0];
                long bytes = Math.Abs(byteCount);
                int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                double num = Math.Round(bytes / Math.Pow(1024, place), 1);
                return (Math.Sign(byteCount) * num).ToString() + suf[place];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public static bool IsEmail(this string text)
        {
            return Regex.IsMatch(text, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public static string GeneratePassword()
        {
            return GeneratePassword(8);
        }

        public static string GeneratePassword(int length)
        {
            try
            {
                string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
                char[] chars = new char[length];
                Random rd = new Random();

                for (int i = 0; i < length; i++)
                {
                    chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
                }
                return new string(chars);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public static string Encrypt(this string clearText)
        {
            try
            {
                string EncryptionKey = "?E9QjrfA!P+GJVD3G^*Rw=*C2";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = rfc.GetBytes(32);
                    encryptor.IV = rfc.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Decrypt(this string cipherText)
        {
            try
            {
                string EncryptionKey = "?E9QjrfA!P+GJVD3G^*Rw=*C2";
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ToSHA1(this string text)
        {
            if (string.IsNullOrEmpty(text.Trim()))
                throw new Exception("Şifrelenecek string türünde nesne bulunamadı.");

            SHA1 sha1 = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(text)));
        }

        public static string ToSHA256(this string text)
        {
            if (string.IsNullOrEmpty(text.Trim()))
                throw new Exception("Şifrelenecek string türünde nesne bulunamadı.");

            SHA256 sha = new SHA256Managed();
            return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(text)));
        }

        public static string ToSHA512(this string text)
        {
            if (string.IsNullOrEmpty(text.Trim()))
                throw new Exception("Şifrelenecek string türünde nesne bulunamadı.");

            SHA512 sha = new SHA512Managed();
            return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(text)));
        }

        public static string ToMD5(this string text)
        {
            if (string.IsNullOrEmpty(text.Trim()))
                throw new Exception("Şifrelenecek string türünde nesne bulunamadı.");

            MD5 md5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(text)));
        }

        public static string ToBase64(this string text)
        {
            if (string.IsNullOrEmpty(text.Trim()))
                throw new Exception("Şifrelenecek string türünde nesne bulunamadı.");

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        public static string FromBase64(this string text)
        {
            if (string.IsNullOrEmpty(text.Trim()))
                throw new Exception("Çözülecek string türünde nesne bulunamadı.");

            return Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }
        
        /// <summary>
        /// IEnumerable tipindeki veriyi datatable'a aktarır ve geriye döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            try
            {
                DataTable dt = new DataTable();
                PropertyInfo[] propinfo = typeof(T).GetProperties();
                foreach (var item in propinfo)
                    dt.Columns.Add(item.Name, item.PropertyType);
                foreach (T item in collection)
                {
                    DataRow drow = dt.NewRow();
                    drow.BeginEdit();
                    foreach (var pi in propinfo)
                        drow[pi.Name] = pi.GetValue(item, null);
                    drow.EndEdit();
                    dt.Rows.Add(drow);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// DataTable türündeki veriyi belirtilen T tipindeki nesneye list olarak aktarıp geriye bu listeyi döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt) where T : new()
        {
            try
            {
                List<string> columns = new List<string>();
                foreach (DataColumn item in dt.Columns)
                    columns.Add(item.ColumnName);
                return dt.AsEnumerable().ToList().ConvertAll<T>(x => GetObject<T>(x, columns)).ToList<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static T GetObject<T>(DataRow row, List<string> columns) where T : new()
        {
            try
            {
                T obj = new T();
                foreach (PropertyInfo item in typeof(T).GetProperties())
                {
                    string name = columns.Find(x => x.ToLower() == item.Name.ToLower());
                    if (!string.IsNullOrEmpty(name))
                    {
                        string value = row[name].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(item.PropertyType) != null)
                            {
                                value = row[name].ToString().Replace("$", string.Empty).Replace(",", string.Empty);
                                item.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(item.PropertyType).ToString())), null);
                            }
                            else
                            {
                                value = row[name].ToString().Replace("%", "");
                                item.SetValue(obj, Convert.ChangeType(value, Type.GetType(item.PropertyType.ToString())), null);
                            }
                        }
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Belirtilen SqlCommand nesnesinin veritabanı bağlantısı "Kapalı" durumundaysa "Açık" konumuna getirir.
        /// </summary>
        /// <param name="cmd"></param>
        public static void Open(this SqlCommand cmd)
        {
            if (cmd != null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Open)
                cmd.Connection.Open();
        }

        /// <summary>
        /// Belirtilen SqlCommand nesnesinin veritabanı bağlantısı "Açık" durumundaysa "Kapalı" konumuna getirir.
        /// </summary>
        /// <param name="cmd"></param>
        public static void Close(this SqlCommand cmd)
        {
            if (cmd != null && cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed)
                cmd.Connection.Close();
        }
    }
}