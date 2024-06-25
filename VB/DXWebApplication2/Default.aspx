<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Main.Master" CodeBehind="Default.aspx.vb" Inherits="DXWebApplication2.Default" %>
<%@ Register Assembly="DevExpress.Dashboard.v23.2.Web.WebForms, Version=23.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.DashboardWeb" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    function onBeforeRender(sender) {
        var control = sender.GetDashboardControl();
        control.registerExtension(new DevExpress.Dashboard.DashboardPanelExtension(control, { dashboardThumbnail: "./Content/DashboardThumbnail/{0}.png" }));
    }
</script>
    <dx:ASPxDashboard ID="ASPxDashboard1" runat="server" Width="100%" Height="100%" OnDataLoading="DataLoading" OnConfigureDataConnection="ConfigureDataConnection">
        <ClientSideEvents BeforeRender="onBeforeRender" />
    </dx:ASPxDashboard>
</asp:Content>
