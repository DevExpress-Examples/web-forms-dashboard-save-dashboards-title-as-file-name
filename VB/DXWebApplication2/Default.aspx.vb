Imports DevExpress.DashboardCommon
Imports DevExpress.DashboardWeb
Imports DevExpress.DataAccess.Excel
Imports DevExpress.DataAccess.Sql
Imports System
Imports System.Web.Hosting

Namespace DXWebApplication2

    Public Partial Class [Default]
        Inherits Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
            Dim newDashboardStorage As CustomFileStorage = New CustomFileStorage("~/App_Data/Dashboards")
            ASPxDashboard1.SetDashboardStorage(newDashboardStorage)
            ' Uncomment this string to allow end users to create new data sources based on predefined connection strings.
            'ASPxDashboard1.SetConnectionStringsProvider(new DevExpress.DataAccess.Web.ConfigFileConnectionStringsProvider());
            Dim dataSourceStorage As DataSourceInMemoryStorage = New DataSourceInMemoryStorage()
            ' Registers an SQL data source.
            Dim sqlDataSource As DashboardSqlDataSource = New DashboardSqlDataSource("SQL Data Source", "NWindConnectionString")
            Dim query As SelectQuery = SelectQueryFluentBuilder.AddTable("SalesPerson").SelectAllColumnsFromTable().Build("Sales Person")
            sqlDataSource.Queries.Add(query)
            dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml())
            ' Registers an Object data source.
            Dim objDataSource As DashboardObjectDataSource = New DashboardObjectDataSource("Object Data Source")
            objDataSource.DataId = "Object Data Source Data Id"
            dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml())
            ' Registers an Excel data source.
            Dim excelDataSource As DashboardExcelDataSource = New DashboardExcelDataSource("Excel Data Source")
            excelDataSource.ConnectionName = "Excel Data Source Connection Name"
            excelDataSource.SourceOptions = New ExcelSourceOptions(New ExcelWorksheetSettings("Sheet1"))
            dataSourceStorage.RegisterDataSource("excelDataSource", excelDataSource.SaveToXml())
            ASPxDashboard1.SetDataSourceStorage(dataSourceStorage)
        End Sub

        Protected Sub DataLoading(ByVal sender As Object, ByVal e As DataLoadingWebEventArgs)
            If Equals(e.DataId, "Object Data Source Data Id") Then
                e.Data = Invoices.CreateData()
            End If
        End Sub

        Protected Sub ConfigureDataConnection(ByVal sender As Object, ByVal e As ConfigureDataConnectionWebEventArgs)
            If Equals(e.ConnectionName, "Excel Data Source Connection Name") Then
                Dim excelParameters As ExcelDataSourceConnectionParameters = CType(e.ConnectionParameters, ExcelDataSourceConnectionParameters)
                excelParameters.FileName = HostingEnvironment.MapPath("~/App_Data/Sales.xlsx")
            End If
        End Sub
    End Class
End Namespace
