using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Insight360.DAL;
using Insight360.Util;
using Insight360.BO;
using Insight360.Common;
using System.Data;
using System.Threading.Tasks;

namespace Insight360.Scheduling
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Scheduling" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Scheduling.svc or Scheduling.svc.cs at the Solution Explorer and start debugging.
    public class SchedulingInitiator
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string MineWeb()
        {
            DBUtil.ConnectionString = AppSettings.WebMiner_Common;
            //Task.Factory.StartNew(() => MineFacebook());

            logger.Info("**Mining Completed**");

            return "Mining Completed";
        }

        public void Process()
        {
            string query;
            query = CommonDAL.GetCommonStoredProcedureName(CommonDAL.StoredProcedures.Web_Processor);
            query = query.Replace("Common", "Airways");

            try
            {
                DBUtil.ExecuteNonQuery(query, null, null, CommandType.StoredProcedure);
                logger.Info("** Scheduling Service::::Process.......**");
            }
            catch (Exception ex)
            {
                logger.Error("** Exception::::::Scheduling Service::::Process.......**: " + ex.Message);
            }

            try
            {
                query = CommonDAL.GetCommonStoredProcedureName(CommonDAL.StoredProcedures.WebMiner_ProcessDashboard);
                query = query.Replace("Common", "Airways");
                logger.Info("** Scheduling Service::::Process.......**");

                DBUtil.ExecuteNonQuery(query, null, null, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                logger.Error("** Exception::::::Scheduling Service::::Dashboard.......**: " + ex.Message);
            }
        }
    }
}
