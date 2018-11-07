<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="MasterDetailSetup" %>
<%@ Register Assembly="DevExpress.Web.v13.1" Namespace="DevExpress.Web.ASPxEditors"  TagPrefix="dxe" %>
<%@ Register Assembly="DevExpress.Web.v13.1" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>
<%@ Register Assembly="DevExpress.Web.v13.1" Namespace="DevExpress.Web.ASPxPager" TagPrefix="dxwp" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Modify detail data</title>
</head>
<body>
    <form id="form2" runat="server">
    
    <dxwgv:ASPxGridView ID="grid" ClientInstanceName="grid" runat="server" KeyFieldName="Id" OnCustomCallback="grid_CustomCallback" OnCustomDataCallback="grid_CustomDataCallback">
        <%-- BeginRegion Grid Columns --%>
        <Columns>
            <dxwgv:GridViewDataColumn FieldName="Name" VisibleIndex="0">
            </dxwgv:GridViewDataColumn>
            <dxwgv:GridViewDataColumn FieldName="IsChecked" VisibleIndex="1">
                <DataItemTemplate>
                    <input type="checkbox" <%#Eval("HtmlChecked")%> onclick="<%# GetMasterGridCallBack(Container.VisibleIndex, (int)Container.KeyValue) %>" />
                </DataItemTemplate>
            </dxwgv:GridViewDataColumn>
        </Columns>
        <%-- EndRegion --%>
        <Templates>
            <DetailRow>
                <dxwgv:ASPxGridView ID="detailGrid" runat="server" 
                    KeyFieldName="Id" Width="100%" OnBeforePerformDataSelect="detailGrid_DataSelect">
                    <%-- BeginRegion Grid Columns --%>
                    <Columns>
                        <dxwgv:GridViewDataColumn FieldName="Name" VisibleIndex="0">
                        </dxwgv:GridViewDataColumn>
                        <dxwgv:GridViewDataColumn FieldName="IsChecked" VisibleIndex="1">
                            <DataItemTemplate>
                                <input type="checkbox" <%#Eval("HtmlChecked")%> onclick="<%# GetGridDataCallBack((int)Container.KeyValue) %>" />
                            </DataItemTemplate>
                        </dxwgv:GridViewDataColumn>
                    </Columns>
                    <%-- EndRegion --%>
                    <Settings ShowColumnHeaders="false" />
                    <SettingsDetail IsDetailGrid="true"/>
                </dxwgv:ASPxGridView>
            </DetailRow>
        </Templates>
        <SettingsDetail ShowDetailRow="true"/>
        <SettingsCustomizationWindow Enabled="True"  />
    </dxwgv:ASPxGridView>
    </form>
</body>
</html>
