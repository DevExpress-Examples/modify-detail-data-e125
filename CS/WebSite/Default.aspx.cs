#region Using
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.Web.ASPxGridView;
using System.Web.SessionState;
using System.ComponentModel;
#endregion
public partial class MasterDetailSetup : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {
        grid.DataSource = DataProvider.GetRootItems();
        grid.DataBind();
    }
    SetupItemProvider dataProvider;
    protected SetupItemProvider DataProvider { 
        get {
            if(dataProvider == null) {
                dataProvider = new SetupItemProvider();
            }
            return dataProvider; 
        } 
    }
    protected void detailGrid_DataSelect(object sender, EventArgs e) {
        ASPxGridView detailGrid = sender as ASPxGridView;
        detailGrid.DataSource = DataProvider.GetItems((int)detailGrid.GetMasterRowKeyValue());
    }
    protected void grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e) {
        int id = -1;
        if(int.TryParse(e.Parameters, out id)) {
            ModifyItems(id);
            grid.DataSource = DataProvider.GetRootItems();
            grid.DataBind();
        }
    }
    protected void grid_CustomDataCallback(object sender, ASPxGridViewCustomDataCallbackEventArgs e) {
        int id = -1;
        if(int.TryParse(e.Parameters, out id)) {
            ModifyItems(id);
        }
        e.Result = null;
    }
    protected string GetMasterGridCallBack(int visibleIndex, int id) {
        if(grid.DetailRows.IsVisible(visibleIndex)) {
            return string.Format("grid.PerformCallback({0})", id);
        }
        return GetGridDataCallBack(id);
    }
    protected string GetGridDataCallBack(int id) {
        return string.Format("grid.GetValuesOnCustomCallback({0}, null)", id);
    }
    protected void ModifyItems(int id) {
        SetupItem item = DataProvider.GetItemById(id);
        if(item == null) return;
        item.IsChecked = !item.IsChecked;
        foreach(SetupItem childItem in DataProvider.GetItems(id)) {
            childItem.IsChecked = item.IsChecked;
        }
    }
    #region DataSource
    static int SetupItemIdCounter = 0;

    public class SetupItem {
        int id;
        string name;
        bool isChecked;
        SetupItem parent;

        public SetupItem() : this(string.Empty, false) {}
        public SetupItem(string name, bool isChecked) : this(name, isChecked, null) { }
        public SetupItem(string name, bool isChecked, SetupItem parent) {
            this.id = ++SetupItemIdCounter;
            this.name = name;
            this.isChecked = isChecked;
            this.parent = parent;
        }
        public int Id { get { return id; } }
        public string Name { get { return name; } set { name = value; } }
        public bool IsChecked { get { return isChecked; } set { isChecked = value; } }
        public SetupItem Parent { get { return parent; } }
        public int ParentId { get { return Parent != null ? Parent.Id : -1; } }
        public string HtmlChecked { get { return IsChecked ? "checked" : string.Empty; } }
        protected internal void Assign(SetupItem source) {
            Name = source.Name;
        }
    }

    public class SetupItemProvider {
        HttpSessionState Session { get { return HttpContext.Current.Session; } }

        public BindingList<SetupItem> GetRootItems() { return GetItems(-1); }
        public BindingList<SetupItem> GetItems(int parentId) {
            BindingList<SetupItem> res = new BindingList<SetupItem>();
            foreach(SetupItem item in GetAllItems()) {
                if(item.ParentId == parentId) {
                    res.Add(item);
                }
            }
            return res;
        }
        public SetupItem GetItemById(int id) {
            foreach(SetupItem item in GetAllItems()) {
                if(item.Id == id) return item;
            }
            return null;
        }
        protected BindingList<SetupItem> GetAllItems() {
            BindingList<SetupItem> res = Session["SetupItems"] as BindingList<SetupItem>;
            if(res == null) {
                res = CreateData();
                Session["SetupItems"] = res;
            }
            return res;
        }

        BindingList<SetupItem> CreateData() {
            BindingList<SetupItem> res = new BindingList<SetupItem>();
            for(int i = 0; i < 10; i++) {
                SetupItem category = new SetupItem(string.Format("Category {0}", i + 1), false);
                res.Add(category);
                for(int j = 0; j < 5; j++) {
                    res.Add(new SetupItem(string.Format("Item {0} - {1}", i + 1, j + 1), false, category));
                }
            }
            return res;
        }
        public void Update(SetupItem data) {
            SetupItem item = GetItemById(data.Id);
            if(item != null) {
                item.Assign(data);
            }
        }
    }
    #endregion
}
