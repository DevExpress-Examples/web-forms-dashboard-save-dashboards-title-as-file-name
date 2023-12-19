Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.IO
Imports System.Web.UI.WebControls
Imports System.Xml.Linq
Imports DevExpress.DashboardWeb
Imports DevExpress.DashboardCommon

Namespace DXWebApplication2

    Public Class CustomFileStorage
        Implements DevExpress.DashboardWeb.IEditableDashboardStorage

        Public Property WorkingDirectory As String

        Protected Overridable ReadOnly Property Directory As DirectoryInfo
            Get
                Dim absolutePath As String = If(System.IO.Path.IsPathRooted(Me.WorkingDirectory), Me.WorkingDirectory, System.Web.HttpContext.Current.Server.MapPath(Me.WorkingDirectory))
                Return New System.IO.DirectoryInfo(absolutePath)
            End Get
        End Property

        Public Sub New(ByVal workingDirectory As String)
            Me.WorkingDirectory = workingDirectory
        End Sub

        Private Function CreateIntance(ByVal dashboard As System.Xml.Linq.XDocument) As Dashboard
            Dim instance As DevExpress.DashboardCommon.Dashboard = Nothing
            Try
                instance = New DevExpress.DashboardCommon.Dashboard()
                instance.LoadFromXDocument(dashboard)
                Return instance
            Catch
                If instance IsNot Nothing Then instance.Dispose()
                Return Nothing
            End Try
        End Function

        Protected Overridable Function ResolveFileName(ByVal dashboardID As String) As String
            Dim dashboardFileName = String.Format("{0}.xml", dashboardID)
            dashboardFileName = System.IO.Path.GetInvalidFileNameChars().Aggregate(dashboardFileName, Function(current, c) current.Replace(c.ToString(), String.Empty))
            Return System.IO.Path.Combine(Me.Directory.FullName, dashboardFileName)
        End Function

        Public Function GetAvailableDashboardsInfo() As IEnumerable(Of DevExpress.DashboardWeb.DashboardInfo) Implements Global.DevExpress.DashboardWeb.IDashboardStorage.GetAvailableDashboardsInfo
            Dim dashboardsID As System.Collections.Generic.IEnumerable(Of String) = Me.Directory.GetFiles().[Select](Function(file) System.IO.Path.GetFileNameWithoutExtension(file.Name)).ToList()
            Return dashboardsID.[Select](Function(id)
                Dim name As String = Nothing
                Using instance As DevExpress.DashboardCommon.Dashboard = Me.CreateIntance(Me.LoadDashboard(id))
                    If instance IsNot Nothing Then name = instance.Title.Text
                End Using

                Return New DevExpress.DashboardWeb.DashboardInfo With {.ID = id, .Name = name}
            End Function)
        End Function

        Public Function LoadDashboard(ByVal dashboardID As String) As XDocument Implements Global.DevExpress.DashboardWeb.IDashboardStorage.LoadDashboard
            Dim fileName = Me.ResolveFileName(dashboardID)
            If System.IO.File.Exists(fileName) Then Return System.Xml.Linq.XDocument.Load(fileName)
            Return Nothing
        End Function

        Public Sub SaveDashboard(ByVal dashboardID As String, ByVal dashboard As System.Xml.Linq.XDocument) Implements Global.DevExpress.DashboardWeb.IDashboardStorage.SaveDashboard
            Dim fileName = Me.ResolveFileName(dashboardID)
            dashboard.Save(fileName)
        End Sub

        Public Function AddDashboard(ByVal dashboard As System.Xml.Linq.XDocument, ByVal dashboardName As String) As String Implements Global.DevExpress.DashboardWeb.IEditableDashboardStorage.AddDashboard
            Using instance As DevExpress.DashboardCommon.Dashboard = Me.CreateIntance(dashboard)
                If instance IsNot Nothing Then
                    instance.Title.Text = dashboardName
                    dashboard = instance.SaveToXDocument()
                    Dim fileName = Me.ResolveFileName(dashboardName)
                    dashboard.Save(fileName)
                End If
            End Using

            Return dashboardName
        End Function
    End Class
End Namespace
