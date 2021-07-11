using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RocketNet
{
    public partial class Rocket
    {
        /// <summary>
        /// Geriye SqlDataReader döndürür ve "using" deyimi içerisinde kullanarak veri işlenebilir.
        /// Sql komutunu ve komut tipini belirtebilirsiniz.
        /// Veritabanı bağlantısını açma ve kapatmaya ihtiyaç duymaz.
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string commandText, CommandType commandType, bool useTransaction)
        {
            SqlDataReader dr = null;
            SqlTransaction transaction = null;
            try
            {
                using (SqlCommand cmd = GetCommand(commandText, commandType))
                {
                    cmd.Open();
                    if (useTransaction)
                    {
                        transaction = cmd.Connection.BeginTransaction();
                        cmd.Transaction = transaction;
                    }

                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    if (transaction != null && useTransaction)
                        transaction.Commit();
                }
            }
            catch (SqlException ex)
            {
                if (transaction != null && useTransaction)
                    transaction.Rollback();
                throw ex;
            }
            return dr;
        }

        /// <summary>
        /// Geriye SqlDataReader döndürür ve "using" deyimi içerisinde kullanarak veri işlenebilir.
        /// Sadece Sql komunutu belirterek işlem yapabilirsiniz.
        /// Veritabanı bağlantısını açma ve kapatmaya ihtiyaç duymaz.
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="useTransaction"></param>
        /// <returns>SqlDataReader</returns>
        /// <exception cref="SqlException"></exception>
        public SqlDataReader ExecuteReader(string commandText, bool useTransaction)
        {
            return ExecuteReader(commandText, CommandType.Text, useTransaction);
        }

        /// <summary>
        /// Geriye IEnumerable olarak dönüş yapar. Komutu ve komut tipini belirterek işlem yapabilirsiniz.
        /// Örnek Kullanım <example>List{user} userlist = ExecuteList{user}("select * from users").ToList()</example>
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>T
        /// <param name="useTransaction"></param>
        /// <returns>IEnumerable</returns>
        /// <exception cref="SqlException"></exception>
        public IEnumerable<T> ExecuteList<T>(string commandText, CommandType commandType, bool useTransaction) where T : new()
        {
            List<T> list = new List<T>();
            try
            {
                using (SqlDataReader dr = ExecuteReader(commandText, commandType, useTransaction))
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            T item = new T();
                            for (int i = 0; i < dr.VisibleFieldCount; i++)
                            {
                                if (dr.GetValue(i) != DBNull.Value)
                                {
                                    PropertyInfo propinfo = typeof(T).GetProperty(dr.GetName(i));
                                    if (propinfo != null)
                                    {
                                        propinfo.SetValue(item, dr.GetValue(i), null);
                                    }
                                }
                            }
                            list.Add(item);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return list;
        }f

        /// <summary>
        /// Geriye IEnumerable olarak dönüş yapar. Sadece komutu belirterek işlem yapabilirsiniz.
        /// Örnek Kullanım <example>List{user} userlist = ExecuteList{user}("select * from users").ToList()</example>
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="useTransaction"></param>
        /// <returns>IEnumerable</returns>
        /// <exception cref="SqlException"></exception>
        public IEnumerable<T> ExecuteList<T>(string commandText, bool useTransaction) where T : new()
        {
            return ExecuteList<T>(commandText, useTransaction);
        }

        /// <summary>
        /// Geriye T bilinmeyen nesne tipi olarak dönüş yapar. Sadece komutu belirterek işlem yapabilirsiniz.
        /// Örnek Kullanım <example>var user = ExecuteSingle{user}("select * from users")</example>
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public T ExecuteSingle<T>(string commandText, bool useTransaction) where T : new()
        {
            return ExecuteSingle<T>(commandText, CommandType.Text, useTransaction);
        }

        /// <summary>
        /// Geriye T bilinmeyen nesne tipi olarak dönüş yapar. Komut ve komut tipini belirterek işlem yapabilirsiniz.
        /// Örnek Kullanım <example>var user = ExecuteSingle{user}("select * from users")</example>
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <returns>T</returns>
        /// <exception cref="SqlException"></exception>
        public T ExecuteSingle<T>(string commandText, CommandType commandType, bool useTransaction) where T : new()
        {
            T item;
            try
            {
                item = new T();
                using (SqlDataReader dr = ExecuteReader(commandText, commandType, useTransaction))
                {
                    if (dr.Read() && dr.HasRows)
                    {
                        for (int i = 0; i < dr.VisibleFieldCount; i++)
                        {
                            if (dr.GetValue(i) != DBNull.Value)
                            {
                                PropertyInfo propinfo = typeof(T).GetProperty(dr.GetName(i));
                                if (propinfo != null)
                                {
                                    propinfo.SetValue(item, dr.GetValue(i), null);
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return item;
        }

        /// <summary>
        /// Geriye int tipi olarak dönüş yapar. Dönen veri veritabanında etkilenen satır sayısını verir.
        /// Komut ve komut tipini belirterek işlem yapabilirsiniz.
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public int ExecuteNonQuery(string commandText, CommandType commandType, bool useTransaction)
        {
            int effectedrow = 0;
            SqlTransaction transaction = null;
            SqlCommand cmd = null;
            try
            {
                using (cmd = GetCommand(commandText, commandType))
                {
                    cmd.Open();
                    if (useTransaction)
                    {
                        transaction = cmd.Connection.BeginTransaction();
                        cmd.Transaction = transaction;
                    }

                    effectedrow = cmd.ExecuteNonQuery();
                    if (transaction != null && useTransaction)
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (transaction != null && useTransaction)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                cmd.Close();
            }
            return effectedrow;
        }

        /// <summary>
        /// Geriye int tipi olarak dönüş yapar. Dönen veri veritabanında etkilenen satır sayısını verir.
        /// Sadece komutu belirterek işlem yapabilirsiniz.
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public int ExecuteNonQuery(string commandText, bool useTransaction)
        {
            return ExecuteNonQuery(commandText, CommandType.Text, useTransaction);
        }

        /// <summary>
        /// Geriye object türünde dönüş yapar. Veritabanı komutunuzda geriye dönen veri varsa bu sayede erişebilirsiniz.
        /// Komut ve komut tipini belirterek işlem yapabilirsiniz.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public object ExecuteScalar(string commandText, CommandType commandType, bool useTransaction)
        {
            object item = null;
            SqlTransaction transaction = null;
            SqlCommand cmd = null;
            try
            {
                using (cmd = GetCommand(commandText, commandType))
                {
                    cmd.Open();
                    if (useTransaction)
                    {
                        transaction = cmd.Connection.BeginTransaction();
                        cmd.Transaction = transaction;
                    }

                    item = cmd.ExecuteScalar();
                    if (transaction != null && useTransaction)
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (transaction != null && useTransaction)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                cmd.Close();
            }
            return item;
        }

        /// <summary>
        /// Geriye object türünde dönüş yapar. Veritabanı komutunuzda geriye dönen veri varsa bu sayede erişebilirsiniz.
        /// Sadece komutu belirterek işlem yapabilirsiniz.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public object ExecuteScalar(string commandText, bool useTransaction)
        {
            return ExecuteScalar(commandText, CommandType.Text, useTransaction);
        }

        /// <summary>
        /// Geriye DataTable türünde dönüş yapar.
        /// Komut ve komut tipini belirterek işlem yapabilirsiniz.
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public DataTable ExecuteDataTable(string commandText, CommandType commandType, bool useTransaction)
        {
            DataTable dt = new DataTable();
            SqlTransaction transaction = null;
            SqlCommand cmd = null;
            try
            {
                using (cmd = GetCommand(commandText, commandType))
                {
                    if (useTransaction)
                    {
                        cmd.Open();
                        transaction = cmd.Connection.BeginTransaction();
                        cmd.Transaction = transaction;
                    }

                    SqlDataAdapter dap = new SqlDataAdapter(cmd);
                    dap.Fill(dt);

                    if (transaction != null && useTransaction)
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (transaction != null && useTransaction)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                cmd.Close();
            }
            return dt;
        }

        /// <summary>
        /// Geriye DataTable türünde dönüş yapar.
        /// Sadece komutu belirterek işlem yapabilirsiniz.
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public DataTable ExecuteDataTable(string commandText, bool useTransaction)
        {
            return ExecuteDataTable(commandText, CommandType.Text, useTransaction);
        }

        /// <summary>
        /// Geriye DataSet türünde dönüş yapar.
        /// Komut ve komut tipini belirterek işlem yapabilirsiniz.
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public DataSet ExecuteDataSet(string commandText, CommandType commandType, bool useTransaction)
        {
            DataSet ds = new DataSet();
            SqlTransaction transaction = null;
            SqlCommand cmd = null;
            try
            {
                using (cmd = GetCommand(commandText, commandType))
                {
                    if (useTransaction)
                    {
                        cmd.Open();
                        transaction = cmd.Connection.BeginTransaction();
                        cmd.Transaction = transaction;
                    }

                    SqlDataAdapter dap = new SqlDataAdapter(cmd);
                    dap.Fill(ds);

                    if (transaction != null && useTransaction)
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (transaction != null && useTransaction)
                    transaction.Commit();
                throw ex;
            }
            finally
            {
                cmd.Close();
            }
            return ds;
        }

        /// <summary>
        /// Geriye DataSet türünde dönüş yapar.
        /// Sadece komutu belirterek işlem yapabilirsiniz.
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public DataSet ExecuteDataSet(string commandText, bool useTransaction)
        {
            return ExecuteDataSet(commandText, CommandType.Text, useTransaction);
        }
    }
}
