using System;
using System.Collections.Generic;
using Shuyz.Framework.Mvvm;
using ATL.Common;
using System.Data;


namespace XRayClient.BatteryCheckManager
{
    public class ProductionDataOffline : ObservableObject
    {
        private List<ProductionDataXray> _productionDataList = new List<ProductionDataXray>();

        public static BaseFacade baseFacade = new BaseFacade();

        public List<ProductionDataXray> ProductionDataList
        {
            get { return this._productionDataList; }
            set
            {
                this._productionDataList = value;
                RaisePropertyChanged("ProductionDataList");
            }
        }

        public bool AddProductionData(ProductionDataXray productionData)
        {
            string sql = $"INSERT INTO offline_production_data(`ProductSN`,`Data`,`keyValue`) VALUES('{productionData.ProductSN}','{productionData.JsonData}','{productionData.KeyValue}');";
            bool success = false;
            string exceptionMsg = string.Empty;
            try
            {
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                success = true;
            }
            catch (Exception _ex)
            {
                exceptionMsg = _ex.ToString();
            }
            if (!success) LogHelper.Error(exceptionMsg);

            return true;
        }

        public bool RemoveProductionData(string productSN)
        {
            string sql = $"DELETE FROM offline_production_data WHERE ProductSN='{productSN}';";
            bool success = false;
            string exceptionMsg = string.Empty;
            try
            {
                baseFacade.equipDB.ExecuteNonQuery(CommandType.Text, sql);
                success = true;
            }
            catch (Exception _ex)
            {
                exceptionMsg = _ex.ToString();
            }
            if (!success) LogHelper.Error(exceptionMsg);

            return true;
        }

        public bool RefreshProductionDataList()
        {
            string sql = "SELECT id, logDateTime, ProductSN, Data, keyValue FROM offline_production_data;";
            DataSet ds = baseFacade.equipDB.ExecuteDataSet(CommandType.Text, sql);

            List<ProductionDataXray> tmpList = new List<ProductionDataXray>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ProductionDataXray productionData = new ProductionDataXray();

                productionData.RecordID = (int)ds.Tables[0].Rows[i][0];
                productionData.LogDateTime = (DateTime)ds.Tables[0].Rows[i][1];
                productionData.ProductSN = (string)ds.Tables[0].Rows[i][2];
                productionData.JsonData = ds.Tables[0].Rows[i][3].ToString();
                productionData.KeyValue = (string)ds.Tables[0].Rows[i][4];
                tmpList.Add(productionData);
            }

            this._productionDataList = tmpList;

            return true;
        }
    }
}
