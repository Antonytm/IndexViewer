﻿using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace IndexViewer
{
    using Sitecore.Configuration;
    using Sitecore.Globalization;


    public partial class SelectIndex : System.Web.UI.Page
    {
        #region properties

        private IndexType IndexType
        {
            get
            {
                return (IndexType)Enum.Parse(typeof(IndexType), IndexTypeSelector.SelectedValue);
            }
        }

        #endregion properties

        
        #region protected methods for events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitilizeIndexTypeSelector();
                ResetSelectors();
                OKButton.Enabled = false;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ErrorInfo.InnerHtml = ErrorResolver.CheckPageError();
        }

        protected void OKButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(IndexSelector.SelectedValue))
                {
                    SessionManager.Instance.ClearAll();

                    IIndexResolver resolver = ResolverFactory.GetIndexResolver(IndexType, DatabaseSelector.SelectedValue);

                    SessionManager.Instance.CurrentIndex = resolver.GetIndex(IndexSelector.SelectedValue);

                    Response.Write("<script language='javascript'>window.close();</script>");
                }
                else
                {
                    Response.Write(String.Format("<script language='javascript'>window.alert({0});</script>",
                            Translate.Text("'The index is not selected. Please select an index.'")));
                }
            }
            catch (Exception ex)
            {
                ErrorResolver.ResolveError(ex, this);
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Write("<script language='javascript'>window.close();</script>");
            }
            catch (Exception ex)
            {
                ErrorResolver.ResolveError(ex, this);
            }
        }

        protected void IndexTypeSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SessionManager.Instance.ClearAll();
                OKButton.Enabled = false;

                if (!String.IsNullOrEmpty(IndexTypeSelector.SelectedValue))
                {
                    if (IndexType == IndexType.DataIndex)
                    {
                        InitilizeDatabaseSelector();

                        DatabaseSelector.Enabled = true;
                        DatabaseSelectorPanel.Visible = true;
                    }
                    else
                    {
                        InitilizeIndexSelector();

                        DatabaseSelector.Enabled = false;
                        DatabaseSelectorPanel.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorResolver.ResolveError(ex, this);
            }
        }


        protected void DatabaseSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SessionManager.Instance.ClearAll();
                OKButton.Enabled = false;
                
                if (String.IsNullOrEmpty(DatabaseSelector.SelectedValue))
                {
                    IndexSelector.Items.Clear();
                    IndexSelector.Enabled = false;
                }
                else
                {
                    InitilizeIndexSelector();
                }
            }
            catch (Exception ex)
            {
                ErrorResolver.ResolveError(ex, this);
            }
        }


        protected void IndexSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SessionManager.Instance.ClearAll();

                if (!String.IsNullOrEmpty(IndexSelector.SelectedValue))
                {
                    IIndexResolver resolver = ResolverFactory.GetIndexResolver(IndexType, DatabaseSelector.SelectedValue);

                    SessionManager.Instance.CurrentIndex = resolver.GetIndex(IndexSelector.SelectedValue);

                    OKButton.Enabled = true;
                }
                else
                {
                    OKButton.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ErrorResolver.ResolveError(ex, this);
            }
        }

        #endregion protected methods for events


        #region private methods

        private void InitilizeIndexTypeSelector()
        {
            IndexTypeSelector.Items.Add(new ListItem(String.Empty, null));

            foreach (string enumItemName in Enum.GetNames(typeof(IndexType)))
            {
                IndexTypeSelector.Items.Add(new ListItem(enumItemName, enumItemName));
            }
        }

        private void InitilizeDatabaseSelector()
        {
            DatabaseSelector.Items.Add(new ListItem(String.Empty, null));

            foreach (string dbName in Factory.GetDatabaseNames())
            {
                DatabaseSelector.Items.Add(new ListItem(dbName, dbName));
            }
        }

        private void InitilizeIndexSelector()
        {
            IndexSelector.Items.Clear();

            IIndexResolver resolver = ResolverFactory.GetIndexResolver(IndexType, DatabaseSelector.SelectedValue);

            List<string> indexNames = resolver.GetIndexNames();
            indexNames.Insert(0, String.Empty);

            IndexSelector.DataSource = indexNames;
            IndexSelector.DataBind();

            IndexSelector.Enabled = true;
        }

        private void ResetSelectors()
        {
            DatabaseSelector.Items.Clear();
            DatabaseSelector.Enabled = false;

            IndexSelector.Items.Clear();
            IndexSelector.Enabled = false;
        }

        #endregion private methods

    }
}
