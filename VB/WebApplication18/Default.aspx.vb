Imports System

Namespace WebApplication18
    Partial Public Class [Default]
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
            Dim newDashboardStorage As New CustomFileStorage("~/App_Data/Dashboards")
            ASPxDashboard1.SetDashboardStorage(newDashboardStorage)
        End Sub
    End Class
End Namespace