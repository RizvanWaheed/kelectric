using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using Insight360.BO;
using System.Diagnostics;
using System.Data.SqlClient;
using Insight360.Common.BO;

namespace Insight360.DAL
{
    public static class ConfigDAL
    {
        public static List<string> FetchOrganizations()
        {
            string query = "select name from " + CommonDAL.GetCommonTableName(CommonDAL.Tables.Config_Organization)//CommonDAL.GetCommonStoredProcedureName(CommonDAL.StoredProcedures.Config_FetchOrganizationConfiguration);
                + " WHERE isenabled=1";
            List<string> list = new List<string>();

            DataTable table = DBUtil.FetchRecords(query, null, null, CommandType.Text);

            foreach (DataRow row in table.Rows)
            {
                if (!list.Contains(row["Name"].ToString()))
                {
                    list.Add(row["Name"].ToString());
                }
                //OrganizationConfiguration config = new OrganizationConfiguration();
                //SearchTermConfiguration searchTerm = new SearchTermConfiguration();
                //searchTerm.SearchTerm = row["SearchTerm_Term"].ToString();
                //searchTerm.Type = (SearchTermConfiguration.Types)(int)row["SearchTerm_Type"];
                //searchTerm.Enabled = Convert.ToBoolean(row["SearchTerm_Enabled"]);
                //config.Name = row["Name"].ToString();
                //config.Enabled = Convert.ToBoolean(row["Enabled"]);

                //if (list.Count == 0)
                //{
                //    config.SearchTerms.Add(searchTerm);
                //    list.Add(config);
                //}
                //else
                //{
                //    if (list[list.Count - 1].Name.Equals(config.Name, StringComparison.InvariantCultureIgnoreCase))
                //        list[list.Count - 1].SearchTerms.Add(searchTerm);
                //    else
                //    {
                //        config.SearchTerms.Add(searchTerm);
                //        list.Add(config);
                //    }
                //}
            }
            return list;
        }

        public static int GetFrequency(string DocumentSourceType)
        {
            string query = CommonDAL.GetCommonStoredProcedureName(CommonDAL.StoredProcedures.GetFrequency);
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@DocumentSourceType_Type", DocumentSourceType));
            var fre = DBUtil.ExecuteScalar(query, parameters, null, CommandType.StoredProcedure);
            return Convert.ToInt32(fre);
        }

        public static List<Location> GetLocations(int searchTermId)
        {
            List<Location> list = new List<Location>();
            string query = CommonDAL.GetCommonStoredProcedureName(CommonDAL.StoredProcedures.GetLocations);
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@SearchTermId", searchTermId));
            DataTable table = DBUtil.FetchRecords(query, parameters, null, CommandType.StoredProcedure);
            foreach (DataRow row in table.Rows)
            {
                Location loc = new Location();
                loc.Id = Convert.ToInt32(row["Id"]);
                loc.SearchTerm_Id = Convert.ToInt32(row["SearchTerm_Id"]);
                loc.latitude = Convert.ToDecimal(row["latitude"]);
                loc.longitude = Convert.ToDecimal(row["longitude"]);
                loc.name = row["name"].ToString();
                loc.TwitterGranuality = row["TwitterGranuality"].ToString();
                loc.TwitterPlaceId = row["TwitterPlaceId"].ToString();
                list.Add(loc);
            }
            return list;
        }

        public static List<Location> GetLocationsByDocumentSourceId(int id)
        {
            List<Location> list = new List<Location>();
            string query = CommonDAL.GetCommonStoredProcedureName(CommonDAL.StoredProcedures.GetLocations);

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@DocumentSourceId", id));

            DataTable table = DBUtil.FetchRecords(query, parameters, null, CommandType.StoredProcedure);

            foreach (DataRow row in table.Rows)
            {
                Location loc = new Location();

                loc.Id = Convert.ToInt32(row["Id"]);
                loc.SearchTerm_Id = Convert.ToInt32(row["SearchTerm_Id"]);
                loc.latitude = Convert.ToDecimal(row["latitude"]);
                loc.longitude = Convert.ToDecimal(row["longitude"]);
                loc.name = row["name"].ToString();
                loc.TwitterGranuality = row["TwitterGranuality"].ToString();
                loc.TwitterPlaceId = row["TwitterPlaceId"].ToString();

                list.Add(loc);
            }

            return list;
        }

        public static List<string> GetFacebookPages()
        {
            List<string> list = new List<string>();

            string query = CommonDAL.GetCommonStoredProcedureName(CommonDAL.StoredProcedures.GetFacebookPages);
            DataTable table = DBUtil.FetchRecords(query, null, null, CommandType.StoredProcedure);

            foreach (DataRow row in table.Rows)
            {
                list.Add(row["PageId"].ToString());
            }

            return list;
        }


        public static List<KeyTerm> GetKeyTerms(int searchTermId)
        {
            List<KeyTerm> list = new List<KeyTerm>();
            string query = CommonDAL.GetCommonStoredProcedureName(CommonDAL.StoredProcedures.GetKeyTerms);
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@SearchTermId", searchTermId));
            DataTable table = DBUtil.FetchRecords(query, parameters, null, CommandType.StoredProcedure);
            foreach (DataRow row in table.Rows)
            {
                KeyTerm term = new KeyTerm();
                term.Id = Convert.ToInt32(row["Id"]);
                term.Name = row["KeyTerm"].ToString();
                list.Add(term);
            }
            return list;
        }

        public static List<DocumentSourceInfo> FetchDocumentSourceConfiguration(string DocumentSourceType)
        {
            string query = CommonDAL.GetCommonStoredProcedureName(CommonDAL.StoredProcedures.Config_FetchDocumentSourceConfiguration);

            List<DocumentSourceInfo> list = new List<DocumentSourceInfo>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@DocumentSourceType_Type", DocumentSourceType));

            DataTable table = DBUtil.FetchRecords(query, parameters, null, CommandType.StoredProcedure);


            foreach (DataRow row in table.Rows)
            {
                DocumentSourceInfo config = new DocumentSourceInfo();
                config.DocumentSourceId = Convert.ToInt32(row["DocumentSource_Id"]);
                config.Url = row["DocumentSource_Url"].ToString();
                config.DocumentSourceTypeId = Convert.ToInt32(row["DocumentSourceType_Id"]);
                config.Frequency = Convert.ToInt32(row["Frequency"]);
                config.IsEnabled = Convert.ToInt32(row["IsEnabled"]); //Dec 03, 2015 Ahsen Mehmood: Is Network enabled or not
                config.DisplayName = row["DisplayName"].ToString(); //Dec 28, 2015 Ahsen Mehmood: To know the source types of Facebook on runtime for mining
                config.ProfileType = (ProfileTypes)(int)row["FBProfileType"];

                //config.LastFetchTime = (DateTime)row["DocumentSource_LastFetch_Time"];
                //config.LastFetchId = (string)row["DocumentSource_LastFetch_Id"];

                //OrganizationInfo org = new OrganizationInfo();
                //org.Id = (int)row["Organization_Id"];
                //org.Name = row["Organization_Name"].ToString();
                //org.LastFetchTime = (DateTime)row["Organization_LastFetch_Time"];
                //org.LastFetchId = (string)row["Organization_LastFetch_Id"];
                //org.LastFetchId2 = (string)row["Organization_LastFetch_Id2"];

                SearchTermInfo searchTerm = new SearchTermInfo();
                bool searchTermValid = !(row["SearchTerm_Term"] is DBNull);

                if (searchTermValid)
                {
                    searchTerm.Id = (int)row["SearchTerm_Id"];
                    searchTerm.LastFetchTime = (DateTime)row["SearchTerm_LastFetch_Time"];
                    //searchTerm.UntillFetchTime = (DateTime)row["SearchTerm_Untill_Time"];
                    searchTerm.LastFetchId = (string)row["SearchTerm_LastFetch_Id"];
                    searchTerm.LastFetchId2 = (string)row["SearchTerm_LastFetch_Id2"];
                    searchTerm.SearchTerm = (string)row["SearchTerm_Term"];
                    searchTerm.Type = (SearchTermInfo.Types)(int)row["SearchTerm_Type"];
                    searchTerm.IsTermEnabled = Convert.ToInt32(row["IsTermEnabled"]); //Dec 03, 2015 Ahsen Mehmood: Is search term enabled or not
                    searchTerm.latitude = Convert.ToDouble(row["latitude"]);
                    searchTerm.longitude = Convert.ToDouble(row["longitude"]);
                }

                if (list.Count == 0)
                {
                    //if (searchTermValid)
                    //  org.SearchTerms.Add(searchTerm);
                    config.SearchTerms.Add(searchTerm);
                    list.Add(config);
                }
                else
                {
                    DocumentSourceInfo lastConfig = list[list.Count - 1];
                    if (lastConfig.DocumentSourceId == config.DocumentSourceId)
                    {
                        //OrganizationInfo lastOrg = lastConfig.Organizations[lastConfig.Organizations.Count - 1];
                        //if (lastOrg.Name.Equals(org.Name, StringComparison.InvariantCultureIgnoreCase))
                        //{
                        //    if (searchTermValid)
                        //        lastOrg.SearchTerms.Add(searchTerm);
                        //}
                        //else
                        //{
                        //    if (searchTermValid)
                        //        org.SearchTerms.Add(searchTerm);
                        //    lastConfig.Organizations.Add(org);
                        //}
                        if (searchTermValid)
                            lastConfig.SearchTerms.Add(searchTerm);
                    }
                    else
                    {
                        if (searchTermValid)
                            config.SearchTerms.Add(searchTerm);
                        //config.Organizations.Add(org);
                        list.Add(config);
                    }
                }
            }

            return list;
        }

        public static List<string> GetFacebookPagesBySearchTermId(int searchTermId)
        {
            List<string> list = new List<string>();

            string query = CommonDAL.GetCommonStoredProcedureName(CommonDAL.StoredProcedures.GetFacebookPagesBySearchTermId);
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@SearchTermId", searchTermId));

            DataTable table = DBUtil.FetchRecords(query, parameters, null, CommandType.StoredProcedure);

            foreach (DataRow row in table.Rows)
            {
                list.Add(row["PageId"].ToString());
            }

            return list;
        }

        public static void UpdateDocumentSourceConfiguration(SearchTermInfo updated, int documentSourceId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("UPDATE ");
            query.Append(CommonDAL.GetCommonTableName(CommonDAL.Tables.Config_DocumentSource_SearchTerm));
            query.Append(" SET ");

            List<SqlParameter> parameters = new List<SqlParameter>();
            bool check = false;
            if (updated.LastFetchId != "0")
            {
                query.Append(" LastFetch_Id =@LastFetch_Id");
                parameters.Add(new SqlParameter("@LastFetch_Id", updated.LastFetchId));
                check = true;
            }
            if (updated.LastFetchId2 != "0")
            {
                if (check)
                    query.Append(",");
                query.Append(" LastFetch_Id2 =@LastFetch_Id2");
                parameters.Add(new SqlParameter("@LastFetch_Id2", updated.LastFetchId2));
                check = true;
            }

            if (updated.LastFetchTime.Year > 1900 && updated.LastFetchTime < DateTime.MaxValue)
            {
                if (check)
                    query.Append(",");
                query.Append(" LastFetch_Time =@LastFetch_Time");
                parameters.Add(new SqlParameter("@LastFetch_Time", updated.LastFetchTime));
                check = true;
            }
            if (check)
            {
                //parameters.Add(new SqlParameter("@Organization_Id", organizationId));
                parameters.Add(new SqlParameter("@DocumentSource_Id", documentSourceId));
                parameters.Add(new SqlParameter("@SearchTerm_Id", updated.Id));
                query.Append(" WHERE DocumentSource_Id = @DocumentSource_Id and SearchTerm_Id=@SearchTerm_Id");
                var guid = Guid.NewGuid().ToString();
           //     DBUtil.ExecuteNonQuery(query.ToString(), parameters, null, CommandType.Text, guid);
                DBUtil.CloseConnection(guid);
            }
        }

        //public static void UpdateDocumentSourceConfiguration(OrganizationInfo updated, int documentSourceId)
        //{
        //    StringBuilder query = new StringBuilder();
        //    query.Append("UPDATE ");
        //    query.Append(CommonDAL.GetCommonTableName(CommonDAL.Tables.Config_DocumentSource_Organization));
        //    query.Append(" SET ");

        //    List<SqlParameter> parameters = new List<SqlParameter>();
        //    bool check = false;
        //    if (updated.LastFetchId != "0")
        //    {
        //        query.Append(" LastFetch_Id =@LastFetch_Id");
        //        parameters.Add(new SqlParameter("@LastFetch_Id", updated.LastFetchId));
        //        check = true;
        //    }
        //    if (updated.LastFetchTime.Year > 1900 && updated.LastFetchTime < DateTime.MaxValue)
        //    {
        //        if (check)
        //            query.Append(",");
        //        query.Append(" LastFetch_Time =@LastFetch_Time");
        //        parameters.Add(new SqlParameter("@LastFetch_Time", updated.LastFetchTime));
        //        check = true;
        //    }
        //    if (check)
        //    {
        //        parameters.Add(new SqlParameter("@Organization_Id", updated.Id));
        //        parameters.Add(new SqlParameter("@DocumentSource_Id", documentSourceId));
        //        query.Append(" WHERE Organization_Id =@Organization_Id and DocumentSource_Id = @DocumentSource_Id");
        //        var guid = Guid.NewGuid( ).ToString( );
        //        DBUtil.ExecuteNonQuery( query.ToString( ) , parameters , null , CommandType.Text , guid );
        //        DBUtil.CloseConnection( guid );
        //    }
        //}

        //public static void UpdateDocumentSourceConfiguration(DocumentSourceInfo updated)
        //{
        //    StringBuilder query = new StringBuilder();
        //    query.Append("UPDATE ");
        //    query.Append(CommonDAL.GetCommonTableName(CommonDAL.Tables.Config_DocumentSources));
        //    query.Append(" SET ");

        //    List<SqlParameter> parameters = new List<SqlParameter>();
        //    //bool check = false;
        //    //if (updated.LastFetchId != "0")
        //    //{
        //    //    query.Append(" LastFetch_Id =@LastFetch_Id");
        //    //    parameters.Add(new SqlParameter("@LastFetch_Id", updated.LastFetchId));
        //    //    check = true;
        //    //}
        //    //if (updated.LastFetchTime.Year > 1900 && updated.LastFetchTime < DateTime.MaxValue)
        //    //{
        //    //    if (check)
        //    //        query.Append(",");
        //    //    query.Append(" LastFetch_Time =@LastFetch_Time");
        //    //    parameters.Add(new SqlParameter("@LastFetch_Time", updated.LastFetchTime));
        //    //    check = true;
        //    //}
        //    //if (check)
        //    {
        //        parameters.Add(new SqlParameter("@DocumentSource_Id", updated.DocumentSourceId));
        //        query.Append(" WHERE Id = @DocumentSource_Id");
        //        var guid = Guid.NewGuid( ).ToString( );
        //        DBUtil.ExecuteNonQuery( query.ToString( ) , parameters , null , CommandType.Text , guid );
        //        DBUtil.CloseConnection(guid);
        //    }
        //}



        public static List<DocumentSourceTypeIndexingInfo> FetchDocumentSourceType(int? documentSourceTypeId)
        {
            string query = "SELECT dsti.DocumentSourceType_Id,dsti.ViewName,dst.Type,dsti.IsEnabled,dsti.FindSearchTerms,dsti.TopCount,dsti.WhereCondition,dsti.OrderBy FROM "
                + CommonDAL.GetCommonTableName(CommonDAL.Tables.Config_DocumentSourceType)
                + " dst inner join "
                + CommonDAL.GetCommonTableName(CommonDAL.Tables.Config_DocumentSourceType_Indexing)
                + " dsti on dsti.DocumentSourceType_Id=dst.Id"
                ;

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (documentSourceTypeId != null)
            {
                query += " WHERE dst.Id = @Id";
                parameters.Add(new SqlParameter("@Id", documentSourceTypeId));
            }
            List<DocumentSourceTypeIndexingInfo> list = new List<DocumentSourceTypeIndexingInfo>();

            DataTable table = DBUtil.FetchRecords(query, parameters, null, CommandType.Text);

            foreach (DataRow row in table.Rows)
            {
                DocumentSourceTypeIndexingInfo config = new DocumentSourceTypeIndexingInfo()
                {
                    DocumentSourceTypeId = (int)row["DocumentSourceType_Id"],
                    Type = row["Type"].ToString(),
                    ViewName = row["ViewName"].ToString(),
                    //config.DisplayName = row["DisplayName"].ToString();
                    //config.ImageUrl = row["ImageUrl"].ToString();
                    //config.QueryFetch = row["Query"].ToString();
                    //config.TableName = row["TableName"].ToString();
                    FindSearchTerms = Convert.ToBoolean(row["FindSearchTerms"]),
                    Enabled = Convert.ToBoolean(row["IsEnabled"]),
                    //config.IndexingEnabled = Convert.ToBoolean(row["IndexingEnabled"]);
                    TopCount = (int)row["TopCount"],
                    WhereCondition = row["WhereCondition"].ToString(),
                    OrderBy = row["OrderBy"].ToString(),

                };
                list.Add(config);
            }
            return list;
        }




    }
}
