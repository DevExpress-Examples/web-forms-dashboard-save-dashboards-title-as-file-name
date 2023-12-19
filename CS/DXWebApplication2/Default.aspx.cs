using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Sql;
using System;
using System.Web.Hosting;

namespace DXWebApplication2 {
    public partial class Default : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            CustomFileStorage newDashboardStorage = new CustomFileStorage(@"~/App_Data/Dashboards");
            ASPxDashboard1.SetDashboardStorage(newDashboardStorage);

            // Uncomment this string to allow end users to create new data sources based on predefined connection strings.
            //ASPxDashboard1.SetConnectionStringsProvider(new DevExpress.DataAccess.Web.ConfigFileConnectionStringsProvider());

            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
            
            // Registers an SQL data source.
            DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");
            SelectQuery query = SelectQueryFluentBuilder
                .AddTable("SalesPerson")
                .SelectAllColumnsFromTable()
                .Build("Sales Person");
            sqlDataSource.Queries.Add(query);
            dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());
            
            // Registers an Object data source.
            DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("Object Data Source");
            objDataSource.DataId = "Object Data Source Data Id";
            dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml());
            
            // Registers an Excel data source.
            DashboardExcelDataSource excelDataSource = new DashboardExcelDataSource("Excel Data Source");
            excelDataSource.ConnectionName = "Excel Data Source Connection Name";
            excelDataSource.SourceOptions = new ExcelSourceOptions(new ExcelWorksheetSettings("Sheet1"));
            dataSourceStorage.RegisterDataSource("excelDataSource", excelDataSource.SaveToXml());
            
            ASPxDashboard1.SetDataSourceStorage(dataSourceStorage);
        }

        protected void DataLoading(object sender, DataLoadingWebEventArgs e) {
            if(e.DataId == "Object Data Source Data Id") {
                e.Data = Invoices.CreateData();
            }
        }

        protected void ConfigureDataConnection(object sender, ConfigureDataConnectionWebEventArgs e) {
            if(e.ConnectionName == "Excel Data Source Connection Name") {
                ExcelDataSourceConnectionParameters excelParameters = (ExcelDataSourceConnectionParameters)e.ConnectionParameters;
                excelParameters.FileName = HostingEnvironment.MapPath(@"~/App_Data/Sales.xlsx");
            }
        }
    }
}