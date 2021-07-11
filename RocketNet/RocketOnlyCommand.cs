using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RocketNet
{
    public partial class Rocket
    {
        /// <summary>
        /// Geriye SqlDataReader döndürür ve "using" deyimi içerisinde kullanarak veri işlenebilir.
        /// Sadece Sql komutunu belirterek işlem yapabilirsiniz.
        /// Veritabanı bağlantısını açma ve kapatmaya ihtiyaç duymaz.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns>SqlDataReader</returns>
        /// <exception cref="SqlException"></exception>
        public SqlDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(commandText, CommandType.Text);
        }

        /// <summary>
        /// Geriye IEnumerable olarak dönüş yapar. Sadece komutu belirterek işlem yapabilirsiniz.
        /// Örnek Kullanım <example>List{user} userlist = ExecuteList{user}("select * from users").ToList()</example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public IEnumerable<T> ExecuteList<T>(string commandText) where T : new()
        {
            return ExecuteList<T>(commandText, CommandType.Text);
        }

        /// <summary>
        /// Geriye T bilinmeyen nesne tipi olarak dönüş yapar. Sadece komutu belirterek işlem yapabilirsiniz.
        /// Örnek Kullanım <example>var user = ExecuteSingle{user}("select * from users")</example>
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public T ExecuteSingle<T>(string commandText) where T : new()
        {
            return ExecuteSingle<T>(commandText, CommandType.Text);
        }

        /// <summary>
        /// Geriye int tipi olarak dönüş yapar. Dönen veri veritabanında etkilenen satır sayısını verir.
        /// Sadece komutu belirterek işlem yapabilirsiniz.
        /// Ek olarak SqlTransaction kullanarak veri kaybının önüne geçebilirsiniz. Bunun için useTransaction özelliğine true verin.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, CommandType.Text);
        }

        /// <summary>
        /// Geriye object türünde dönüş yapar. Veritabanı komutunuzda geriye dönen veri varsa bu sayede erişebilirsiniz.
        /// Sadece komutu belirterek işlem yapabilirsiniz.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(commandText, CommandType.Text);
        }

        /// <summary>
        /// Geriye DataTable türünde dönüş yapar.
        /// Sadece komutu belirterek işlem yapabilirsiniz.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public DataTable ExecuteDataTable(string commandText)
        {
            return ExecuteDataTable(commandText, CommandType.Text);
        }

        /// <summary>
        /// Geriye DataSet türünde dönüş yapar.
        /// Sadece komutu belirterek işlem yapabilirsiniz.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        /// <exception cref="SqlException"></exception>
        public DataSet ExecuteDataSet(string commandText)
        {
            return ExecuteDataSet(commandText, CommandType.Text);
        }
    }
}
