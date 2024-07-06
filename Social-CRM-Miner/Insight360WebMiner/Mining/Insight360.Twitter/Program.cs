using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Insight360.Common;

namespace Insight360.Twitter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //load the connection string from configuration file
            //DAL.DBUtil.ConnectionString = AppSettings.WebMiner_Common;
            DAL.DBUtil.ConnectionString = "Data Source=192.168.4.73;Initial Catalog=WebMiner_Common;user id=nestleclearview;password=n3$tle;";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
