Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.IO
Imports System.Web.UI.WebControls
Imports System.Xml.Linq
Imports DevExpress.DashboardWeb
Imports DevExpress.DashboardCommon

Namespace WebApplication18
    Public Class CustomFileStorage
        Implements IEditableDashboardStorage

        Public Property WorkingDirectory() As String
        Protected Overridable ReadOnly Property Directory() As DirectoryInfo
            Get
                Dim absolutePath As String = If(Path.IsPathRooted(WorkingDirectory), WorkingDirectory, HttpContext.Current.Server.MapPath(WorkingDirectory))
                Return New DirectoryInfo(absolutePath)
            End Get
        End Property

        Public Sub New(ByVal workingDirectory As String)
            Me.WorkingDirectory = workingDirectory
        End Sub

        Private Function CreateIntance(ByVal dashboard As XDocument) As Dashboard
            Dim instance As Dashboard = Nothing
            Try
                instance = New Dashboard()
                instance.LoadFromXDocument(dashboard)
                Return instance
            Catch
                If instance IsNot Nothing Then
                    instance.Dispose()
                End If
                Return Nothing
            End Try
        End Function

        Protected Overridable Function ResolveFileName(ByVal dashboardID As String) As String
            Dim dashboardFileName = String.Format("{0}.xml", dashboardID)
            dashboardFileName = Path.GetInvalidFileNameChars().Aggregate(dashboardFileName, Function(current, c) current.Replace(c.ToString(), String.Empty))
            Return Path.Combine(Directory.FullName, dashboardFileName)
        End Function

        Public Function GetAvailableDashboardsInfo() As IEnumerable(Of DashboardInfo) Implements IEditableDashboardStorage.GetAvailableDashboardsInfo
            Dim dashboardsID As IEnumerable(Of String) = Directory.GetFiles().Select(Function(file) Path.GetFileNameWithoutExtension(file.Name)).ToList()
            Return dashboardsID.Select(Function(id)
                                           Dim name As String = Nothing
                                           Using instance As Dashboard = CreateIntance(LoadDashboard(id))
                                               If instance IsNot Nothing Then
                                                   name = instance.Title.Text
                                               End If
                                           End Using
                                           Return New DashboardInfo With {
                                               .ID = id,
                                               .Name = name
                                           }
                                       End Function)
        End Function
        Public Function LoadDashboard(ByVal dashboardID As String) As XDocument Implements IEditableDashboardStorage.LoadDashboard
            Dim fileName = ResolveFileName(dashboardID)
            If File.Exists(fileName) Then
                Return XDocument.Load(fileName)
            End If
            Return Nothing
        End Function
        Public Sub SaveDashboard(ByVal dashboardID As String, ByVal dashboard As XDocument) Implements IEditableDashboardStorage.SaveDashboard
            Dim fileName = ResolveFileName(dashboardID)
            dashboard.Save(fileName)
        End Sub
        Public Function AddDashboard(ByVal dashboard As XDocument, ByVal dashboardName As String) As String Implements IEditableDashboardStorage.AddDashboard
            Using instance As Dashboard = CreateIntance(dashboard)
                If instance IsNot Nothing Then
                    instance.Title.Text = dashboardName
                    dashboard = instance.SaveToXDocument()
                    Dim fileName = ResolveFileName(dashboardName)
                    dashboard.Save(fileName)
                End If
            End Using
            Return dashboardName
        End Function
    End Class
End Namespace