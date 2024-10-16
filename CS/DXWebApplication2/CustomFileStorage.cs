﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using DevExpress.DashboardWeb;
using DevExpress.DashboardCommon;

namespace DXWebApplication2 {
    public class CustomFileStorage : IEditableDashboardStorage {

        public string WorkingDirectory { get; set; }
        protected virtual DirectoryInfo Directory
        {
            get
            {
                string absolutePath = Path.IsPathRooted(WorkingDirectory) ? WorkingDirectory : HttpContext.Current.Server.MapPath(WorkingDirectory);
                return new DirectoryInfo(absolutePath);
            }
        }

        public CustomFileStorage(string workingDirectory) {
            WorkingDirectory = workingDirectory;
        }

        Dashboard CreateIntance(XDocument dashboard) {
            Dashboard instance = null;
            try {
                instance = new Dashboard();
                instance.LoadFromXDocument(dashboard);
                return instance;
            } catch {
                if (instance != null)
                    instance.Dispose();
                return null;
            }
        }

        protected virtual string ResolveFileName(string dashboardID) {
            var dashboardFileName = string.Format("{0}.xml", dashboardID);
            dashboardFileName = Path.GetInvalidFileNameChars().Aggregate(dashboardFileName, (current, c) => current.Replace(c.ToString(), string.Empty));
            return Path.Combine(Directory.FullName, dashboardFileName);
        }

        public IEnumerable<DashboardInfo> GetAvailableDashboardsInfo() {
            IEnumerable<string> dashboardsID = Directory.GetFiles().Select(file => Path.GetFileNameWithoutExtension(file.Name)).ToList();
            return dashboardsID.Select(id => {
                string name = null;
                using (Dashboard instance = CreateIntance(LoadDashboard(id))) {
                    if (instance != null)
                        name = instance.Title.Text;
                }
                return new DashboardInfo {
                    ID = id,
                    Name = name
                };
            });
        }
        public XDocument LoadDashboard(string dashboardID) {
            var fileName = ResolveFileName(dashboardID);
            if (File.Exists(fileName))
                return XDocument.Load(fileName);
            return null;
        }
        public void SaveDashboard(string dashboardID, XDocument dashboard) {
            var fileName = ResolveFileName(dashboardID);
            dashboard.Save(fileName);
        }
        public string AddDashboard(XDocument dashboard, string dashboardName) {
            using (Dashboard instance = CreateIntance(dashboard)) {
                if (instance != null) {
                    instance.Title.Text = dashboardName;
                    dashboard = instance.SaveToXDocument();

                    var fileName = ResolveFileName(dashboardName);
                    dashboard.Save(fileName);
                }
            }
            return dashboardName;
        }
    }
}