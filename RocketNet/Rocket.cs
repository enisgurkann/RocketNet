using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace RocketNet
{
    /// <summary>
    /// Rocket.Net Ado.Net Data Access Library
    /// </summary>
    public partial class Rocket
    {
        private SqlConnection con = null;
        private ParamCollection parameters = new ParamCollection();
        private bool AutoClearParameters = true;

        /// <summary>
        /// Bu şekilde kullanım önerilmez. Herhangi bir bağlantı belirtilmediği için Exception alırsınız.
        /// </summary>
        public Rocket()
        {
            throw new Exception("Herhangi bir bağlantı dizesi algılanamadı.");
        }

        /// <summary>
        /// String tipinde veritabanı bağlantı cümlenizi belirtebilirsiniz.
        /// </summary>
        /// <param name="conString"></param>
        public Rocket(string conString)
        {
            this.con = new SqlConnection(conString.Trim());
        }

        /// <summary>
        /// SqlConnection tipinde bağlantınızı vererek işlem yapabilirsiniz.
        /// </summary>
        /// <param name="connection"></param>
        public Rocket(SqlConnection connection)
        {
            this.con = connection;
        }

        /// <summary>
        /// String tipinde veritabanı bağlantı cümlenizi belirtebilirsiniz.
        /// Ek olarak gönderilen parametrelerin işlem sonrasında otomatik temizlenmesini isterseniz parametersAutoClear özelliniğine true verebilirsiniz. Default değeri : true
        /// </summary>
        /// <param name="conString"></param>
        /// <param name="parametersAutoClear"></param>
        public Rocket(string conString, bool parametersAutoClear)
        {
            this.con = new SqlConnection(conString.Trim());
            this.AutoClearParameters = parametersAutoClear;
        }

        /// <summary>
        /// SqlConnection tipinde bağlantınızı belirterek işlem verebilirsiniz.
        /// Ek olarak gönderilen parametrelerin işlem sonrasında otomatik temizlenmesini isterseniz ; parametersAutoClear özelliniğine "true" verebilirsiniz. Default değeri : true
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="parametersAutoClear"></param>
        public Rocket(SqlConnection connection, bool parametersAutoClear)
        {
            this.con = connection;
            this.AutoClearParameters = parametersAutoClear;
        }

        /// <summary>
        /// Komut nesnesi oluştur
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        private SqlCommand GetCommand(string commandText, CommandType commandType)
        {
            SqlCommand cmd = null;
            try
            {
                cmd = new SqlCommand();
                cmd.Connection = this.con;
                cmd.CommandText = commandText.Trim();
                cmd.CommandType = commandType;

                if (parameters.Count > 0)
                    cmd.Parameters.AddRange(parameters.ToArray());

                if (AutoClearParameters)
                    RemoveParameters();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return cmd;
        }
        
        /// <summary>
        /// Geriye SqlDataReader döndürür ve "using" deyimi içerisinde kullanarak veri işlenebilir.
        /// Sql komunutu ve komut tipini belirtebilirsiniz.
        /// Ek olarak veritabanı bağlantısını açma ve kapatmaya ihtiyaç duymaz.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns>SqlDataReader</returns>
        /// <exception cref="SqlException"></exception>
        public SqlDataReader ExecuteReader(string commandText, CommandType commandType)
        {
            SqlDataReader dr = null;
            try
            {
                using (SqlCommand cmd = GetCommand(commandText, commandType))
                {
                    cmd.Open();
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return dr;
        }
        
        /// <summary>
        /// Geriye IEnumerable olarak dönüş yapar. Komutu ve komut tipini belirterek işlem yapabilirsiniz.
        /// Örnek Kullanım <example>List{user} userlist = ExecuteList{user}('select * from users').ToList()</example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns>IEnumerable</returns>
        /// <exception cref="SqlException"></exception>
        public IEnumerable<T> ExecuteList<T>(string commandText, CommandType commandType) where T : new()
        {
            List<T> list = new List<T>();
            try
            {
                using (SqlDataReader dr = ExecuteReader(commandText, commandType))
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
        }
        
        /// <summary>
        /// Geriye T bilinmeyen nesne tipi olarak dönüş yapar. Komut ve komut tipini belirterek işlem yapabilirsiniz.
        /// Örnek Kullanım <example>var user = ExecuteSingle{user}("select * from users")</example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns>T</returns>
        /// <exception cref="SqlException"></exception>
        public T ExecuteSingle<T>(string commandText, CommandType commandType) where T : new()
        {
            T item = default(T);
            try
            {
                item = new T();
                using (SqlDataReader dr = ExecuteReader(commandText, commandType))
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
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            int effectedrow = 0;
            SqlCommand cmd = null;
            try
            {
                using (cmd = GetCommand(commandText, commandType))
                {
                    cmd.Open();
                    effectedrow = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Close();
            }
            return effectedrow;
        }
        
        /// <summary>
        /// Geriye object türünde dönüş yapar. Veritabanı komutunuzda geriye dönen veri varsa bu sayede erişebilirsiniz.
        /// Komut ve komut tipini belirterek işlem yapabilirsiniz.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public object ExecuteScalar(string commandText, CommandType commandType)
        {
            object item = null;
            SqlCommand cmd = null;
            try
            {
                using (cmd = GetCommand(commandText, commandType))
                {
                    cmd.Open();
                    item = cmd.ExecuteScalar();
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Close();
            }
            return item;
        }
        
        /// <summary>
        /// Geriye DataTable türünde dönüş yapar.
        /// Komut ve komut tipini belirterek işlem yapabilirsiniz.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public DataTable ExecuteDataTable(string commandText, CommandType commandType)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = GetCommand(commandText, commandType);
                SqlDataAdapter dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return dt;
        }
        
        /// <summary>
        /// Geriye DataSet türünde dönüş yapar.
        /// Komut ve komut tipini belirterek işlem yapabilirsiniz.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public DataSet ExecuteDataSet(string commandText, CommandType commandType)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = GetCommand(commandText, commandType);
                SqlDataAdapter dap = new SqlDataAdapter(cmd);
                dap.Fill(ds);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return ds;
        }
    }
}