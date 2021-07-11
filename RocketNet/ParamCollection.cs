using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RocketNet
{
    internal class ParamCollection : IEnumerable
    {
        private List<SqlParameter> parameters;
        public int Count { get; private set; }

        internal ParamCollection()
        {
            this.parameters = new List<SqlParameter>();
            this.Count = this.parameters.Count;
        }

        internal void Add(SqlParameter item)
        {
            this.parameters.Add(item);
            this.Count = this.parameters.Count;
        }

        internal void Remove(SqlParameter item)
        {
            this.parameters.Remove(item);
            this.Count = this.parameters.Count;
        }

        internal void Remove(string parameterName)
        {
            SqlParameter parameter = parameters.Find(x => x.ParameterName == parameterName.Trim());
            this.parameters.Remove(parameter);
            this.Count = this.parameters.Count;
        }

        internal void RemoveAt(int index)
        {
            this.parameters.RemoveAt(index);
            this.Count = this.parameters.Count;
        }

        internal void Clear()
        {
            this.parameters.Clear();
            this.Count = this.parameters.Count;
        }

        internal SqlParameter Find(string parameterName)
        {
            return this.parameters.Find(x => x.ParameterName == parameterName.Trim());
        }

        internal List<SqlParameter> FindAll(string parameterName)
        {
            return this.parameters.FindAll(x => x.ParameterName == parameterName.Trim());
        }

        internal SqlParameter this[int i]
        {
            get { return this.parameters[i]; }
            set { this.parameters[i] = value; }
        }
        
        internal SqlParameter[] ToArray()
        {
            return this.parameters.ToArray();
        }

        public IEnumerator GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(",", parameters.Select(x => x.ParameterName).ToArray());
        }
    }
}