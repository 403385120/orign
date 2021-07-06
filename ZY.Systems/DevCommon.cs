using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZY.Systems
{
    public class DevCommon
    {
        public static void LoadLookUpEdit(ref DevExpress.XtraEditors.LookUpEdit lookUpEdit1, string FieldName, string Caption, bool Visable, int Width)
        {
            LookUpColumnInfo lookUpColumnInfo = new LookUpColumnInfo();
            lookUpEdit1.Properties.Columns.Add(lookUpColumnInfo);
            lookUpColumnInfo.FieldName = FieldName.Replace(" ", "");
            lookUpColumnInfo.Caption = Caption;
            lookUpColumnInfo.Visible = Visable;
            lookUpColumnInfo.Width = Width;
        }

        /// <summary>
        /// 创建XtraGrid列
        /// </summary>
        /// <param name="ExGrid"></param>
        /// <param name="fieldName"></param>
        /// <param name="caption"></param>
        /// <param name="width"></param>
        public static DevExpress.XtraGrid.Columns.GridColumn CreateXtraGridColumns(GridView ExGrid, string fieldName, string caption, int width = 70)
        {
            return CreateXtraGridColumns(ExGrid, fieldName, caption, width, true, -1, true, false, "", null, false, false, null, "", "DisableTextEditor", false);
        }

        /// <summary>
        /// 创建XtraGrid列
        /// </summary>
        /// <param name="ExGrid"></param>
        /// <param name="fieldName"></param>
        /// <param name="caption"></param>
        /// <param name="width"></param>
        /// <param name="visable"></param>
        public static DevExpress.XtraGrid.Columns.GridColumn CreateXtraGridColumns(GridView ExGrid, string fieldName, string caption, int width = 70, bool visable = true)
        {
            return CreateXtraGridColumns(ExGrid, fieldName, caption, width, visable, -1, false, true, "", null, false, false, null, "", "DisableTextEditor", false);
        }

        /// <summary>
        /// 创建XtraGrid列
        /// </summary>
        /// <param name="ExGrid"></param>
        /// <param name="fieldName"></param>
        /// <param name="caption"></param>
        /// <param name="width"></param>
        /// <param name="visable"></param>
        /// <param name="readOnly"></param>
        public static DevExpress.XtraGrid.Columns.GridColumn CreateXtraGridColumns(GridView ExGrid, string fieldName, string caption, int width = 70, bool visable = true, bool readOnly = false)
        {
            return CreateXtraGridColumns(ExGrid, fieldName, caption, width, visable, -1, readOnly, true, "", null, false, false, null, "", "DisableTextEditor", false);
        }

        /// <summary>
        /// 创建XtraGrid列
        /// </summary>
        /// <param name="ExGrid"></param>
        /// <param name="fieldName"></param>
        /// <param name="caption"></param>
        /// <param name="width"></param>
        /// <param name="visable"></param>
        /// <param name="readOnly"></param>
        /// <param name="allowEdit"></param>
        /// <param name="displayFormat"></param>
        public static DevExpress.XtraGrid.Columns.GridColumn CreateXtraGridColumns(GridView ExGrid, string fieldName, string caption, int width = 70, bool visable = true, bool readOnly = false, bool allowEdit = true, string displayFormat = "")
        {
            return CreateXtraGridColumns(ExGrid, fieldName, caption, width, visable, -1, readOnly, allowEdit, displayFormat, null, false, false, null, "", "DisableTextEditor", false);
        }

        /// <summary>
        /// 创建XtraGrid列
        /// </summary>
        /// <param name="ExGrid"></param>
        /// <param name="fieldName"></param>
        /// <param name="caption"></param>
        /// <param name="width"></param>
        /// <param name="columnHandle"></param>
        /// <param name="visable"></param>
        /// <param name="readOnly"></param>
        /// <param name="allowEdit"></param>
        /// <param name="displayFormat"></param>
        /// <param name="dataType"></param>
        /// <param name="isSum"></param>
        /// <param name="isFixed"></param>
        public static DevExpress.XtraGrid.Columns.GridColumn CreateXtraGridColumns(GridView ExGrid, string fieldName, string caption, int width = 70, bool visable = true, int columnHandle = -1, bool readOnly = false, bool allowEdit = true, string displayFormat = "", Type dataType = null, bool isSum = false, bool isFixed = false, DevExpress.XtraGrid.GridControl gridControl = null, string columnEdit = "", string textEditStyle = "DisableTextEditor", bool immediatePopup = false)
        {
            DevExpress.XtraGrid.Columns.GridColumn gridColumn = ExGrid.Columns.Add();
            ExGrid.Columns.Add(gridColumn);
            if (columnHandle >= 0)
            {
                gridColumn.VisibleIndex = columnHandle;
            }
            gridColumn.UnboundType = ConvertDataType(dataType);
            gridColumn.FieldName = fieldName;
            gridColumn.Name = fieldName;
            gridColumn.Caption = caption;
            gridColumn.Visible = !visable;
            gridColumn.Width = width;
            if (isFixed)
            {
                gridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            }
            if (isSum)
            {
                ExGrid.OptionsView.ShowFooter = isSum;
                ExGrid.Appearance.FooterPanel.BackColor = Color.FromArgb(255, 255, 222);
                ExGrid.Appearance.FooterPanel.ForeColor = Color.Blue;
                ExGrid.Appearance.FooterPanel.Options.UseBackColor = true;
                ExGrid.Appearance.FooterPanel.Options.UseFont = true;
                ExGrid.Appearance.FooterPanel.Options.UseForeColor = true;
                gridColumn.SummaryItem.FieldName = gridColumn.FieldName.Replace(" ", "");
                gridColumn.SummaryItem.SummaryType = SummaryItemType.Sum;
            }
            if (!string.IsNullOrEmpty(columnEdit))
            {
                string text = columnEdit.ToUpper();
                string a = text;
                if (a == "REPOSITORYITEMLOOKUPEDIT")
                {
                    RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
                    repositoryItemLookUpEdit.AutoHeight = false;
                    repositoryItemLookUpEdit.Buttons.AddRange(new EditorButton[1]
                    {
                    new EditorButton(ButtonPredefines.Combo)
                    });
                    string text2 = textEditStyle.ToUpper();
                    switch (text2)
                    {
                        case "DISABLETEXTEDITOR":
                            repositoryItemLookUpEdit.TextEditStyle = TextEditStyles.DisableTextEditor;
                            break;
                        case "STANDARD":
                            repositoryItemLookUpEdit.TextEditStyle = TextEditStyles.Standard;
                            break;
                        case "HIDETEXTEDITOR":
                            repositoryItemLookUpEdit.TextEditStyle = TextEditStyles.HideTextEditor;
                            break;
                    }
                    repositoryItemLookUpEdit.ImmediatePopup = immediatePopup;
                    gridControl.RepositoryItems.AddRange(new RepositoryItem[1]
                    {
                    repositoryItemLookUpEdit
                    });
                    gridColumn.ColumnEdit = repositoryItemLookUpEdit;
                }
            }
            if (!string.IsNullOrEmpty(displayFormat))
            {
                if (gridColumn.UnboundType == UnboundColumnType.DateTime)
                {
                    if (gridControl != null)
                    {
                        RepositoryItemDateEdit repositoryItemDateEdit = new RepositoryItemDateEdit();
                        repositoryItemDateEdit.AutoHeight = false;
                        repositoryItemDateEdit.Buttons.AddRange(new EditorButton[1]
                        {
                        new EditorButton(ButtonPredefines.Combo)
                        });
                        repositoryItemDateEdit.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
                        {
                        new EditorButton(ButtonPredefines.Combo)
                        });
                        repositoryItemDateEdit.DisplayFormat.FormatString = displayFormat;
                        repositoryItemDateEdit.DisplayFormat.FormatType = FormatType.Custom;
                        repositoryItemDateEdit.EditFormat.FormatString = displayFormat;
                        repositoryItemDateEdit.EditFormat.FormatType = FormatType.Custom;
                        repositoryItemDateEdit.Mask.EditMask = displayFormat;
                        gridControl.RepositoryItems.AddRange(new RepositoryItem[1]
                        {
                        repositoryItemDateEdit
                        });
                        gridColumn.ColumnEdit = repositoryItemDateEdit;
                    }
                    else
                    {
                        gridColumn.DisplayFormat.FormatString = displayFormat;
                        gridColumn.DisplayFormat.FormatType = FormatType.Custom;
                    }
                }
                else
                {
                    gridColumn.DisplayFormat.FormatString = displayFormat;
                    gridColumn.DisplayFormat.FormatType = FormatType.Custom;
                }
            }
            gridColumn.OptionsColumn.AllowEdit = allowEdit;
            gridColumn.OptionsColumn.ReadOnly = readOnly;
            gridColumn.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            gridColumn.AppearanceHeader.TextOptions.VAlignment = VertAlignment.Center;
            gridColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            gridColumn.AppearanceCell.TextOptions.VAlignment = VertAlignment.Center;
            gridColumn.AppearanceHeader.Options.UseTextOptions = true;
            gridColumn.AppearanceCell.Options.UseTextOptions = true;
            ExGrid.IndicatorWidth = 35;
            return gridColumn;
        }

        public static BandedGridColumn CreateGridBandColumns(AdvBandedGridView abGv, GridBand gb, string fieldName, string caption, int width = 70, bool visable = true, int columnHandle = -1, bool readOnly = false, bool allowEdit = true, string displayFormat = "", Type dataType = null, bool isSum = false, bool isFixed = false, DevExpress.XtraGrid.GridControl gridControl = null, string columnEdit = "", string textEditStyle = "DisableTextEditor", bool immediatePopup = false)
        {
            BandedGridColumn bandedGridColumn = abGv.Columns.Add();
            if (columnHandle >= 0)
            {
                bandedGridColumn.VisibleIndex = columnHandle;
            }
            bandedGridColumn.UnboundType = ConvertDataType(dataType);
            bandedGridColumn.FieldName = fieldName;
            bandedGridColumn.Name = fieldName;
            bandedGridColumn.Caption = caption;
            bandedGridColumn.Visible = !visable;
            bandedGridColumn.Width = width;
            if (isFixed)
            {
                bandedGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            }
            if (isSum)
            {
                abGv.OptionsView.ShowFooter = isSum;
                abGv.Appearance.FooterPanel.BackColor = Color.FromArgb(255, 255, 222);
                abGv.Appearance.FooterPanel.ForeColor = Color.Blue;
                abGv.Appearance.FooterPanel.Options.UseBackColor = true;
                abGv.Appearance.FooterPanel.Options.UseFont = true;
                abGv.Appearance.FooterPanel.Options.UseForeColor = true;
                bandedGridColumn.SummaryItem.FieldName = bandedGridColumn.FieldName.Replace(" ", "");
                bandedGridColumn.SummaryItem.SummaryType = SummaryItemType.Sum;
            }
            if (!string.IsNullOrEmpty(columnEdit))
            {
                string text = columnEdit.ToUpper();
                string a = text;
                if (a == "REPOSITORYITEMLOOKUPEDIT")
                {
                    RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
                    repositoryItemLookUpEdit.AutoHeight = false;
                    repositoryItemLookUpEdit.Buttons.AddRange(new EditorButton[1]
                    {
                    new EditorButton(ButtonPredefines.Combo)
                    });
                    string text2 = textEditStyle.ToUpper();
                    switch (text2)
                    {
                        case "DISABLETEXTEDITOR":
                            repositoryItemLookUpEdit.TextEditStyle = TextEditStyles.DisableTextEditor;
                            break;
                        case "STANDARD":
                            repositoryItemLookUpEdit.TextEditStyle = TextEditStyles.Standard;
                            break;
                        case "HIDETEXTEDITOR":
                            repositoryItemLookUpEdit.TextEditStyle = TextEditStyles.HideTextEditor;
                            break;
                    }
                    repositoryItemLookUpEdit.ImmediatePopup = immediatePopup;
                    gridControl.RepositoryItems.AddRange(new RepositoryItem[1]
                    {
                    repositoryItemLookUpEdit
                    });
                    bandedGridColumn.ColumnEdit = repositoryItemLookUpEdit;
                }
            }
            if (!string.IsNullOrEmpty(displayFormat))
            {
                if (bandedGridColumn.UnboundType == UnboundColumnType.DateTime)
                {
                    if (gridControl != null)
                    {
                        RepositoryItemDateEdit repositoryItemDateEdit = new RepositoryItemDateEdit();
                        repositoryItemDateEdit.AutoHeight = false;
                        repositoryItemDateEdit.Buttons.AddRange(new EditorButton[1]
                        {
                        new EditorButton(ButtonPredefines.Combo)
                        });
                        repositoryItemDateEdit.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
                        {
                        new EditorButton(ButtonPredefines.Combo)
                        });
                        repositoryItemDateEdit.DisplayFormat.FormatString = displayFormat;
                        repositoryItemDateEdit.DisplayFormat.FormatType = FormatType.Custom;
                        repositoryItemDateEdit.Mask.EditMask = displayFormat;
                        gridControl.RepositoryItems.AddRange(new RepositoryItem[1]
                        {
                        repositoryItemDateEdit
                        });
                        bandedGridColumn.ColumnEdit = repositoryItemDateEdit;
                    }
                    else
                    {
                        bandedGridColumn.DisplayFormat.FormatString = displayFormat;
                        bandedGridColumn.DisplayFormat.FormatType = FormatType.Custom;
                    }
                }
                else
                {
                    bandedGridColumn.DisplayFormat.FormatString = displayFormat;
                    bandedGridColumn.DisplayFormat.FormatType = FormatType.Custom;
                }
            }
            bandedGridColumn.OptionsColumn.AllowEdit = allowEdit;
            bandedGridColumn.OptionsColumn.ReadOnly = readOnly;
            bandedGridColumn.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            bandedGridColumn.AppearanceHeader.TextOptions.VAlignment = VertAlignment.Center;
            bandedGridColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            bandedGridColumn.AppearanceCell.TextOptions.VAlignment = VertAlignment.Center;
            bandedGridColumn.AppearanceHeader.Options.UseTextOptions = true;
            bandedGridColumn.AppearanceCell.Options.UseTextOptions = true;
            abGv.IndicatorWidth = 35;
            gb.Columns.Add(bandedGridColumn);
            gb.Width += bandedGridColumn.Width;
            abGv.Columns.Add(bandedGridColumn);
            return bandedGridColumn;
        }

        /// <summary>
        /// 高级列绑定 
        /// </summary>
        /// <param name="ExGrid"></param>
        /// <param name="fieldName"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static GridBand CreateAdvBandedGridViewColumns(AdvBandedGridView ExGrid, string gpCaption, int tWidth)
        {
            GridBand gridBand = new GridBand();
            gridBand.Width = tWidth;
            gridBand.Caption = gpCaption;
            gridBand.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            ExGrid.Bands.Add(gridBand);
            return gridBand;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="gridControl"></param>
        /// <param name="fieldName"></param>
        /// <param name="caption"></param>
        /// <param name="width"></param>
        /// <param name="visable"></param>
        /// <param name="columnHandle"></param>
        /// <param name="readOnly"></param>
        /// <param name="allowEdit"></param>
        /// <param name="displayFormat"></param>
        /// <param name="dataType"></param>
        /// <param name="isSum"></param>
        /// <param name="isFixed"></param>
        /// <param name="columnEdit"></param>
        /// <param name="textEditStyle"></param>
        /// <param name="immediatePopup"></param>
        /// <returns></returns>
        public static DevExpress.Xpf.Grid.GridColumn CreateXpfXtraGridColumns(DevExpress.Xpf.Grid.GridControl gridControl, string fieldName, string caption, int width = 70, bool visable = true, int columnHandle = -1, bool readOnly = false, bool allowEdit = true, string displayFormat = "", Type dataType = null, bool isSum = false, bool isFixed = false, string columnEdit = "", string textEditStyle = "DisableTextEditor", bool immediatePopup = false)
        {
            DevExpress.Xpf.Grid.GridColumn gridColumn = new DevExpress.Xpf.Grid.GridColumn();
            gridControl.Columns.Add(gridColumn);
            if (columnHandle >= 0)
            {
                gridColumn.VisibleIndex = columnHandle;
            }
            gridColumn.UnboundType = ConvertDataType(dataType);
            gridColumn.FieldName = fieldName;
            gridColumn.Header = caption;
            gridColumn.Visible = !visable;
            gridColumn.Width = (double)width;
            if (isFixed)
            {
                gridColumn.Fixed = DevExpress.Xpf.Grid.FixedStyle.Left;
            }
            if (!string.IsNullOrEmpty(displayFormat))
            {
                TextEditSettings textEditSettings = new TextEditSettings();
                textEditSettings.Mask = displayFormat;
                textEditSettings.DisplayFormat = displayFormat;
                textEditSettings.MaskUseAsDisplayFormat = true;
                gridColumn.EditSettings = textEditSettings;
            }
            gridColumn.AllowEditing = ((!allowEdit) ? DefaultBoolean.False : DefaultBoolean.True);
            gridColumn.ReadOnly = readOnly;
            return gridColumn;
        }

        public static DevExpress.Xpf.Grid.GridColumn CreateXpfXtraGridBandColumns(DevExpress.Xpf.Grid.GridControl gridControl, string fieldName, string caption, int width = 70, bool visable = true, int columnHandle = -1, bool readOnly = false, bool allowEdit = true, string displayFormat = "", Type dataType = null, bool isSum = false, bool isFixed = false, string columnEdit = "", string textEditStyle = "DisableTextEditor", bool immediatePopup = false)
        {
            DevExpress.Xpf.Grid.GridColumn gridColumn = new DevExpress.Xpf.Grid.GridColumn();
            GridControlBand gridControlBand = new GridControlBand();
            gridControlBand.Header = caption;
            gridControlBand.Columns.Add(gridColumn);
            gridControl.Bands.Add(gridControlBand);
            if (columnHandle >= 0)
            {
                gridColumn.VisibleIndex = columnHandle;
            }
            gridColumn.UnboundType = ConvertDataType(dataType);
            gridColumn.FieldName = fieldName;
            gridColumn.Header = caption;
            gridColumn.Visible = !visable;
            gridColumn.Width = (double)width;
            if (isFixed)
            {
                gridColumn.Fixed = DevExpress.Xpf.Grid.FixedStyle.Left;
            }
            if (!string.IsNullOrEmpty(displayFormat))
            {
                TextEditSettings textEditSettings = new TextEditSettings();
                textEditSettings.Mask = displayFormat;
                textEditSettings.DisplayFormat = displayFormat;
                textEditSettings.MaskUseAsDisplayFormat = true;
                gridColumn.EditSettings = textEditSettings;
            }
            gridColumn.AllowEditing = ((!allowEdit) ? DefaultBoolean.False : DefaultBoolean.True);
            gridColumn.ReadOnly = readOnly;
            return gridColumn;
        }

        public static DevExpress.Xpf.Grid.GridColumn CreateXpfXtraGridBandColumns(GridControlBand gcb, string fieldName, string caption, int width = 70, bool visable = true, int columnHandle = -1, bool readOnly = false, bool allowEdit = true, string displayFormat = "", Type dataType = null, bool isSum = false, bool isFixed = false, string columnEdit = "", string textEditStyle = "DisableTextEditor", bool immediatePopup = false)
        {
            DevExpress.Xpf.Grid.GridColumn gridColumn = new DevExpress.Xpf.Grid.GridColumn();
            gcb.Columns.Add(gridColumn);
            if (columnHandle >= 0)
            {
                gridColumn.VisibleIndex = columnHandle;
            }
            gridColumn.UnboundType = ConvertDataType(dataType);
            gridColumn.FieldName = fieldName;
            gridColumn.Header = caption;
            gridColumn.Visible = !visable;
            gridColumn.Width = (double)width;
            if (isFixed)
            {
                gridColumn.Fixed = DevExpress.Xpf.Grid.FixedStyle.Left;
            }
            if (!string.IsNullOrEmpty(displayFormat))
            {
                TextEditSettings textEditSettings = new TextEditSettings();
                textEditSettings.Mask = displayFormat;
                textEditSettings.DisplayFormat = displayFormat;
                textEditSettings.MaskUseAsDisplayFormat = true;
                gridColumn.EditSettings = textEditSettings;
            }
            gridColumn.AllowEditing = ((!allowEdit) ? DefaultBoolean.False : DefaultBoolean.True);
            gridColumn.ReadOnly = readOnly;
            return gridColumn;
        }

        public static GridControlBand CreateXpfAdvBandedGridViewColumn(DevExpress.Xpf.Grid.GridControl gridControl, string gpCaption, int tWidth)
        {
            GridControlBand gridControlBand = new GridControlBand();
            gridControlBand.Header = gpCaption;
            gridControl.Bands.Add(gridControlBand);
            return gridControlBand;
        }

        /// <summary>
        /// 是否选择XtraGrid 行
        /// </summary>
        /// <returns></returns>
        public static bool CheckSelectedRows(GridView exGrid)
        {
            if (exGrid.SelectedRowsCount >= 1)
            {
                return true;
            }
            MessageBox.Show(string.Format("请先选择你要操作的数据行"), "系统提示");
            return false;
        }

        public static bool CheckFocusedRow(GridView exGrid)
        {
            return (exGrid.FocusedRowHandle <= -1) ? true : false;
        }

        /// <summary>
        /// 提示框
        /// </summary>
        /// <param name="toolTipController"></param>
        /// <param name="control"></param>
        /// <param name="toolTip"></param>
        /// <param name="tipIconType"></param>
        public static void SetToolTipController(ToolTipController toolTipController, Control control, string toolTip, ToolTipIconType tipIconType)
        {
            ToolTipControllerShowEventArgs toolTipControllerShowEventArgs = toolTipController.CreateShowArgs();
            toolTipControllerShowEventArgs.ToolTip = toolTip;
            toolTipControllerShowEventArgs.IconType = tipIconType;
            toolTipControllerShowEventArgs.ImageIndex = -1;
            toolTipControllerShowEventArgs.IconSize = ToolTipIconSize.Small;
            toolTipControllerShowEventArgs.Rounded = true;
            toolTipControllerShowEventArgs.ShowBeak = true;
            toolTipControllerShowEventArgs.Show = true;
            toolTipController.ShowHint(toolTipControllerShowEventArgs, control);
        }

        /// <summary>
        /// 提示框
        /// </summary>
        /// <param name="toolTipController"></param>
        /// <param name="control"></param>
        /// <param name="toolTip"></param>
        public static void SetToolTipController(ToolTipController toolTipController, Control control, string toolTip)
        {
            SetToolTipController(toolTipController, control, toolTip, ToolTipIconType.Warning);
        }

        /// <summary>
        /// 隐藏提示框
        /// </summary>
        /// <param name="toolTipController"></param>
        public static void SetToolTipControllerHideHint(ToolTipController toolTipController)
        {
            toolTipController.HideHint();
        }

        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static UnboundColumnType ConvertDataType(Type dataType)
        {
            UnboundColumnType unboundColumnType = UnboundColumnType.Object;
            if (!(dataType == (Type)null))
            {
                if (!dataType.Equals(typeof(string)))
                {
                    if (!dataType.Equals(typeof(DateTime)))
                    {
                        if (!dataType.Equals(typeof(int)) && !dataType.Equals(typeof(long)) && !dataType.Equals(typeof(short)))
                        {
                            if (!dataType.Equals(typeof(Guid)))
                            {
                                if (!dataType.Equals(typeof(decimal)) && !dataType.Equals(typeof(double)) && !dataType.Equals(typeof(float)))
                                {
                                    if (!dataType.Equals(typeof(bool)))
                                    {
                                        return UnboundColumnType.Object;
                                    }
                                    return UnboundColumnType.Boolean;
                                }
                                return UnboundColumnType.Decimal;
                            }
                            return UnboundColumnType.Object;
                        }
                        return UnboundColumnType.Integer;
                    }
                    return UnboundColumnType.DateTime;
                }
                return UnboundColumnType.String;
            }
            return UnboundColumnType.Object;
        }

        /// <summary>
        /// DEV数据类型转成C#数据类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static Type ConvertUnboundColumnTypeToType(UnboundColumnType dataType)
        {
            Type typeFromHandle = typeof(object);
            switch (dataType)
            {
                case UnboundColumnType.String:
                    typeFromHandle = typeof(string);
                    break;
                case UnboundColumnType.DateTime:
                    typeFromHandle = typeof(DateTime);
                    break;
                case UnboundColumnType.Integer:
                    typeFromHandle = typeof(int);
                    break;
                case UnboundColumnType.Decimal:
                    typeFromHandle = typeof(float);
                    break;
                case UnboundColumnType.Boolean:
                    typeFromHandle = typeof(bool);
                    break;
                case UnboundColumnType.Bound:
                    typeFromHandle = typeof(object);
                    break;
            }
            return typeFromHandle;
        }

        public static SqlDbType ConvertSqlNameToSqlType(string sqlTypeString)
        {
            SqlDbType result = SqlDbType.Variant;
            if (sqlTypeString != null)
            {
                string text = sqlTypeString.ToLower();
                switch (text)
                {
                    case "int":
                        result = SqlDbType.Int;
                        break;
                    case "varchar":
                        result = SqlDbType.VarChar;
                        break;
                    case "bit":
                        result = SqlDbType.Bit;
                        break;
                    case "datetime":
                    case "date":
                        result = SqlDbType.DateTime;
                        break;
                    case "decimal":
                        result = SqlDbType.Decimal;
                        break;
                    case "double":
                        result = SqlDbType.Float;
                        break;
                    case "float":
                        result = SqlDbType.Float;
                        break;
                    case "image":
                        result = SqlDbType.Image;
                        break;
                    case "money":
                        result = SqlDbType.Money;
                        break;
                    case "ntext":
                        result = SqlDbType.NText;
                        break;
                    case "nvarchar":
                        result = SqlDbType.NVarChar;
                        break;
                    case "smalldatetime":
                        result = SqlDbType.SmallDateTime;
                        break;
                    case "smallint":
                        result = SqlDbType.SmallInt;
                        break;
                    case "text":
                        result = SqlDbType.Text;
                        break;
                    case "bigint":
                        result = SqlDbType.BigInt;
                        break;
                    case "binary":
                        result = SqlDbType.Binary;
                        break;
                    case "char":
                        result = SqlDbType.Char;
                        break;
                    case "nchar":
                        result = SqlDbType.NChar;
                        break;
                    case "numeric":
                        result = SqlDbType.Decimal;
                        break;
                    case "real":
                        result = SqlDbType.Real;
                        break;
                    case "smallmoney":
                        result = SqlDbType.SmallMoney;
                        break;
                    case "sql_variant":
                        result = SqlDbType.Variant;
                        break;
                    case "timestamp":
                        result = SqlDbType.Timestamp;
                        break;
                    case "tinyint":
                        result = SqlDbType.TinyInt;
                        break;
                    case "uniqueidentifier":
                        result = SqlDbType.UniqueIdentifier;
                        break;
                    case "varbinary":
                        result = SqlDbType.VarBinary;
                        break;
                    case "xml":
                        result = SqlDbType.Xml;
                        break;
                }
                return result;
            }
            return result;
        }

        public static Type ConvertSqlTypeToCsharpType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(long);
                case SqlDbType.Bit:
                    return typeof(bool);
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                    return typeof(DateTime);
                case SqlDbType.Float:
                    return typeof(double);
                case SqlDbType.Int:
                case SqlDbType.TinyInt:
                    return typeof(int);
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(decimal);
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                    return typeof(string);
                case SqlDbType.Real:
                    return typeof(float);
                case SqlDbType.SmallInt:
                    return typeof(short);
                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid);
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return typeof(object);
                case SqlDbType.Xml:
                    return typeof(object);
                default:
                    return null;
            }
        }

        public static string DbTypeToCS(string sqlType)
        {
            string[] array = new string[27]
            {
            "int",
            "varchar",
            "bit",
            "datetime",
            "decimal",
            "float",
            "image",
            "money",
            "ntext",
            "nvarchar",
            "smalldatetime",
            "smallint",
            "text",
            "bigint",
            "binary",
            "char",
            "nchar",
            "numeric",
            "real",
            "smallmoney",
            "sql_variant",
            "timestamp",
            "tinyint",
            "uniqueidentifier",
            "varbinary",
            "integer",
            "date"
            };
            string[] array2 = new string[27]
            {
            "int",
            "string",
            "bool",
            "DateTime",
            "Decimal",
            "Double",
            "Byte[]",
            "Double",
            "string",
            "string",
            "DateTime",
            "int",
            "string",
            "long",
            "Byte[]",
            "string",
            "string",
            "Decimal",
            "Single",
            "Single",
            "Object",
            "string",
            "Byte",
            "Guid",
            "Byte[]",
            "int",
            "DateTime"
            };
            int num = Array.IndexOf(array, sqlType.ToLower());
            return array2[num];
        }

        /// <summary>
        /// GridView 转 DataTable
        /// </summary>
        /// <param name="gv"></param>
        /// <returns></returns>
        public static DataTable ConvertGridViewToTable(GridView gv)
        {
            DataTable dataTable = new DataTable();
            for (int i = 0; i < gv.Columns.Count; i++)
            {
                if (gv.Columns[i].Visible)
                {
                    Type dataType = ConvertUnboundColumnTypeToType(gv.Columns[i].UnboundType);
                    DataColumn column = new DataColumn(gv.Columns[i].Caption, dataType);
                    if (!dataTable.Columns.Contains(gv.Columns[i].Caption))
                    {
                        dataTable.Columns.Add(column);
                    }
                }
            }
            for (int j = 0; j < gv.RowCount; j++)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int k = 0; k < gv.Columns.Count; k++)
                {
                    dataRow[k] = Convert.ToString(gv.GetRowCellValue(j, gv.Columns[k].FieldName));
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        /// <summary>
        /// 将list数据转换成datatable(包含数据)
        /// </summary>
        /// <typeparam name="T">数据对象</typeparam>
        /// <param name="lstData">list数据源</param>
        /// <param name="gv">AdvBandedGridView对象</param>
        /// <param name="dt">表结构</param>
        /// <param name="tableName">excel的sheet名称</param>
        /// <returns></returns>
        public static DataTable ConvertAdvBandedGridViewToTable<T>(List<T> lstData, AdvBandedGridView gv = null, string tableName = "sheet1") where T : class, new()
        {
            DataTable dataTable = null;
            try
            {
                dataTable = new DataTable();
                dataTable.TableName = tableName;
                if (gv != null)
                {
                    for (int i = 0; i < gv.Bands.Count; i++)
                    {
                        for (int j = 0; j < gv.Bands[i].Columns.Count; j++)
                        {
                            try
                            {
                                BandedGridColumn bandedGridColumn = gv.Bands[i].Columns[j];
                                if (bandedGridColumn.Visible)
                                {
                                    Type dataType = ConvertUnboundColumnTypeToType(bandedGridColumn.UnboundType);
                                    DataColumn dataColumn = new DataColumn();
                                    dataColumn.Caption = bandedGridColumn.FieldName;
                                    dataColumn.ColumnName = gv.Bands[i].Caption + bandedGridColumn.Caption;
                                    dataColumn.DataType = dataType;
                                    if (!dataTable.Columns.Contains(bandedGridColumn.Caption))
                                    {
                                        dataTable.Columns.Add(dataColumn);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                DataTable dataTable2 = ReflectConvert.ListToTable(lstData);
                foreach (DataRow row in dataTable2.Rows)
                {
                    DataRow dataRow2 = dataTable.NewRow();
                    for (int k = 0; k < dataTable.Columns.Count; k++)
                    {
                        try
                        {
                            string columnName = dataTable.Columns[k].ColumnName;
                            if (dataTable.Columns.Contains(columnName))
                            {
                                object obj = row[dataTable.Columns[k].Caption];
                                if (dataTable.Columns.Contains(columnName))
                                {
                                    dataRow2[columnName] = ((obj == null) ? DBNull.Value : obj);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    dataTable.Rows.Add(dataRow2);
                }
            }
            catch (Exception ex3)
            {
                string message = ex3.Message;
            }
            return dataTable;
        }

        /// <summary>
        /// 根据数据转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstData"></param>
        /// <param name="gv"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable ConvertGridViewToTable<T>(List<T> lstData, GridView gv = null, string tableName = "sheet1") where T : class, new()
        {
            DataTable dataTable = null;
            try
            {
                dataTable = new DataTable();
                dataTable.TableName = tableName;
                if (gv != null)
                {
                    for (int i = 0; i < gv.Columns.Count; i++)
                    {
                        try
                        {
                            if (gv.Columns[i].Visible)
                            {
                                Type dataType = ConvertUnboundColumnTypeToType(gv.Columns[i].UnboundType);
                                DataColumn dataColumn = new DataColumn();
                                dataColumn.Caption = gv.Columns[i].FieldName;
                                dataColumn.ColumnName = gv.Columns[i].Caption;
                                dataColumn.DataType = dataType;
                                if (!dataTable.Columns.Contains(gv.Columns[i].Caption))
                                {
                                    dataTable.Columns.Add(dataColumn);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                DataTable dataTable2 = ReflectConvert.ListToTable(lstData);
                foreach (DataRow row in dataTable2.Rows)
                {
                    DataRow dataRow2 = dataTable.NewRow();
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        try
                        {
                            if (dataTable2.Columns.Contains(dataTable.Columns[j].Caption))
                            {
                                object obj = row[dataTable.Columns[j].Caption];
                                string columnName = dataTable.Columns[j].ColumnName;
                                if (dataTable.Columns.Contains(columnName))
                                {
                                    dataRow2[columnName] = ((obj == null) ? DBNull.Value : obj);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    dataTable.Rows.Add(dataRow2);
                }
            }
            catch (Exception ex3)
            {
                string message = ex3.Message;
            }
            return dataTable;
        }

        /// <summary>
        /// 将list数据转换成datatable结构(不包含数据) 
        /// </summary>
        /// <param name="gv">gridview</param>
        /// <param name="tableName">excel的sheet名称</param>
        /// <returns></returns>
        public static DataTable ConvertGridViewToTableColumns(GridView gv = null, string tableName = "sheet1")
        {
            DataTable dataTable = null;
            try
            {
                dataTable = new DataTable();
                dataTable.TableName = tableName;
                if (gv != null)
                {
                    for (int i = 0; i < gv.Columns.Count; i++)
                    {
                        try
                        {
                            if (gv.Columns[i].Visible)
                            {
                                Type dataType = ConvertUnboundColumnTypeToType(gv.Columns[i].UnboundType);
                                DataColumn dataColumn = new DataColumn();
                                dataColumn.Caption = gv.Columns[i].FieldName;
                                dataColumn.ColumnName = gv.Columns[i].Caption;
                                dataColumn.DataType = dataType;
                                if (!dataTable.Columns.Contains(gv.Columns[i].Caption))
                                {
                                    dataTable.Columns.Add(dataColumn);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                string message = ex2.Message;
            }
            return dataTable;
        }

        /// <summary>
        /// 将list数据转换成datatable(包含数据)
        /// </summary>
        /// <typeparam name="T">数据对象</typeparam>
        /// <param name="lstData">list数据源</param>
        /// <param name="gv">gridview</param>
        /// <param name="dt">表结构</param>
        /// <param name="tableName">excel的sheet名称</param>
        /// <returns></returns>
        public static DataTable ConvertGridViewToTableRows<T>(List<T> lstData, GridView gv = null, DataTable dt = null, string tableName = "sheet1") where T : class, new()
        {
            try
            {
                if (dt == null)
                {
                    dt = new DataTable();
                    dt.TableName = tableName;
                    if (gv != null)
                    {
                        for (int i = 0; i < gv.Columns.Count; i++)
                        {
                            try
                            {
                                if (gv.Columns[i].Visible)
                                {
                                    Type dataType = ConvertUnboundColumnTypeToType(gv.Columns[i].UnboundType);
                                    DataColumn dataColumn = new DataColumn();
                                    dataColumn.Caption = gv.Columns[i].FieldName;
                                    dataColumn.ColumnName = gv.Columns[i].Caption;
                                    dataColumn.DataType = dataType;
                                    if (!dt.Columns.Contains(gv.Columns[i].Caption))
                                    {
                                        dt.Columns.Add(dataColumn);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                DataTable dataTable = ReflectConvert.ListToTable(lstData);
                foreach (DataRow row in dataTable.Rows)
                {
                    DataRow dataRow2 = dt.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        try
                        {
                            if (dataTable.Columns.Contains(dt.Columns[j].Caption))
                            {
                                object obj = row[dt.Columns[j].Caption];
                                string columnName = dt.Columns[j].ColumnName;
                                if (dt.Columns.Contains(columnName))
                                {
                                    dataRow2[columnName] = ((obj == null) ? DBNull.Value : obj);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    dt.Rows.Add(dataRow2);
                }
            }
            catch (Exception ex3)
            {
                string message = ex3.Message;
            }
            return dt;
        }
    }
}
