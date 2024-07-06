using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Trg.Tech.Framework.Classes.Logging;
using System.Data.SqlClient;
using System.Collections;
using System.Diagnostics;
using System.IO;
using log4net.Config;
using log4net;

namespace Insight360.Util
{
    public static class Log
    {
        static string GetMethodName()
        {
            var stackFrame=new StackTrace(true).GetFrame(2);
            var methodInfo = stackFrame.GetMethod();

            return methodInfo.DeclaringType.FullName 
                + "."
                + methodInfo.Name
                + "-" 
                + stackFrame.GetFileLineNumber()
                
                ;
        }

        static Log()
        {
            XmlConfigurator.Configure();
        }
        
        //public static void WriteDBException(Exception ex, string query, List<SqlParameter> parameters)
        //{
        //    if (parameters == null)
        //        WriteDatabaseException(GetMethodName(), ex, query, (SqlParameter[])null);
        //    else
        //        WriteDatabaseException(GetMethodName(), ex, query, parameters.ToArray());
        //}

        //public static void WriteDBException(Exception ex, string query, SqlParameter[] parameters)
        //{
        //    WriteDatabaseException(GetMethodName(), ex, query, parameters);
        //}

        //static void WriteDatabaseException(string methodName, Exception ex, string query, SqlParameter[] parameters)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("Query: " + query);

        //    if (parameters != null)
        //    {
        //        for (int i = 0; i < parameters.Length; i++)
        //            sb.AppendLine("Parameter: " + parameters[i].ParameterName + " = " + parameters[i].Value);
        //    }
        //    sb.AppendLine("Exception: " + ex.ToString());
        //    ApplicationTrace.Log(methodName, sb.ToString(), Status.Failed);
        //}

        static string GetDbCommandText(SqlCommand cmd)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Query: " + cmd.CommandText);

            if (cmd.Parameters != null)
            {
                for (int i = 0; i < cmd.Parameters.Count; i++)
                    sb.Append(" Parameter: " + cmd.Parameters[i].ParameterName + " = " + cmd.Parameters[i].Value + " , ");
            }
            return sb.ToString();
        }
        public static void WriteDbException(Type sourceType, string sourceMethod, Exception exception, SqlCommand cmd)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetDbCommandText(cmd));
                sb.Append(" Exception: " + exception.ToString());
                //ApplicationTrace.Log(GetMethodName(), sb.ToString(), Status.Failed);
                LogManager.GetLogger(sourceType).Error(sourceMethod, exception);
            }
            catch (Exception ex)
            {
                //ApplicationTrace.Log(string.Empty, ex1.ToString(), Status.Failed);
                LogManager.GetLogger(sourceType).Error(sourceMethod + "-" + exception.ToString(), ex);
            }
        }

        public static void WriteException(Type sourceType, string sourceMethod, Exception exception)
        {
            try
            {
                //ApplicationTrace.Log(GetMethodName(), ex.ToString(), Status.Failed);
                LogManager.GetLogger(sourceType).Error(sourceMethod, exception);
            }
            catch (Exception ex)
            {
                //ApplicationTrace.Log(string.Empty, ex1.ToString(), Status.Failed);
                LogManager.GetLogger(sourceType).Error(sourceMethod + "-" + exception.ToString(), ex);
            }
        }

        public static void WriteOperationStarted(Type sourceType, string sourceMethod, string message)
        {
            try
            {
                //ApplicationTrace.Log(GetMethodName(), message, Status.Started);
                LogManager.GetLogger(sourceType).Info(sourceMethod + "-" + message + " Started");
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(sourceType).Error(sourceMethod, ex);
            }
        }

        public static void WriteOperationCompleted(Type sourceType, string sourceMethod, string message)
        {
            try
            {
                //ApplicationTrace.Log(GetMethodName(), message, Status.Completed);
                LogManager.GetLogger(sourceType).Info(sourceMethod + "-" + message + " Completed");
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(sourceType).Error(sourceMethod, ex);
            }
        }

        public static void WriteDbCommand(Type sourceType, string sourceMethod, SqlCommand cmd)
        {
            try
            {
                //ApplicationTrace.Log(GetMethodName(), GetDbCommandText(cmd), Status.Started);
                LogManager.GetLogger(sourceType).Info(sourceMethod + "-" + GetDbCommandText(cmd));
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(sourceType).Error(sourceMethod, ex);
            }
        }


    }
}
