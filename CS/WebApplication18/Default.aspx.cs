using System;

namespace WebApplication18 {
    public partial class Default : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            CustomFileStorage newDashboardStorage = new CustomFileStorage(@"~/App_Data/Dashboards");
            ASPxDashboard1.SetDashboardStorage(newDashboardStorage);
        }
    }
}