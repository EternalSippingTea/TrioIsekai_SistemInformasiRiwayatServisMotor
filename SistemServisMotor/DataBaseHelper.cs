using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemServisMotor
{
    public static class DatabaseHelper
    {
        static string connstr = "Data Source=siptea-lt\\FAR;Initial Catalog=DBBengkel;Integrated Security=True";

        public static SqlConnection GetConn()
        {
            return new SqlConnection(connstr);
        }
    }
}
