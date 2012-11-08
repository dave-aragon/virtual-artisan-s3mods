using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
namespace Misukisu.HomeBarBusiness
{

    // This class is by CmoMoney, thanks to him.
    public class BigObjectPickerDialog : ModalDialog
    {
        public const int CANCEL_BUTTON = 99576786;
        public const int ITEM_TABLE = 99576784;
        public const string kLayoutName = "cmo_BigObjectPicker";
        public const int kWinExportID = 1;
        public const int OKAY_BUTTON = 99576785;
        public const int TABLE_BACKGROUND = 99576788;
        public const int TABLE_BEZEL = 99576789;
        public const int TITLE_TEXT = 99576787;
        public Button mCloseButton;
        public Button mOkayButton;
        public List<ObjectPicker.RowInfo> mPreSelectedRows;
        public List<ObjectPicker.RowInfo> mResult;
        public ObjectPicker mTable;
        public Vector2 mTableOffset;
        public List<ObjectPicker.RowInfo> Result
        {
            get
            {
                return this.mResult;
            }
        }
        public BigObjectPickerDialog(bool modal, ModalDialog.PauseMode pauseMode, string title, string buttonTrue, string buttonFalse, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows, Vector2 position, bool viewTypeToggle, List<ObjectPicker.RowInfo> preSelectedRows, bool showHeadersAndToggle)
            : base("cmo_BigObjectPicker", 1, modal, pauseMode, null)
        {
            if (this.mModalDialogWindow != null)
            {
                Text text = this.mModalDialogWindow.GetChildByID(99576787u, false) as Text;
                text.Caption = title;
                this.mTable = (this.mModalDialogWindow.GetChildByID(99576784u, false) as ObjectPicker);
                this.mTable.ObjectTable.TableChanged += new TableContainer.TableChangedEventHandler(this.OnTableChanged);
                this.mTable.SelectionChanged += new ObjectPicker.ObjectPickerSelectionChanged(this.OnSelectionChanged);
                this.mTable.RowSelected += new ObjectPicker.ObjectPickerSelectionChanged(this.OnSelectionChanged);
                this.mOkayButton = (this.mModalDialogWindow.GetChildByID(99576785u, false) as Button);
                this.mOkayButton.TooltipText = buttonTrue;
                this.mOkayButton.Enabled = false;
                //this.mOkayButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnOkayButtonClick);
                UIManager.RegisterEvent<UIButtonClickEventArgs>(this.mOkayButton, 678582774u, new UIEventHandler<UIButtonClickEventArgs>(this.OnOkayButtonClick));
                base.OkayID = this.mOkayButton.ID;
                base.SelectedID = this.mOkayButton.ID;
                this.mCloseButton = (this.mModalDialogWindow.GetChildByID(99576786u, false) as Button);
                this.mCloseButton.TooltipText = buttonFalse;
                //this.mCloseButton.Click += new UIEventHandler<UIButtonClickEventArgs>(this.OnCloseButtonClick);
                UIManager.RegisterEvent<UIButtonClickEventArgs>(this.mCloseButton, 678582774u, new UIEventHandler<UIButtonClickEventArgs>(this.OnCloseButtonClick));
                base.CancelID = this.mCloseButton.ID;
                this.mTableOffset = this.mModalDialogWindow.Area.BottomRight - this.mModalDialogWindow.Area.TopLeft - (this.mTable.Area.BottomRight - this.mTable.Area.TopLeft);
                this.mTable.ShowHeaders = showHeadersAndToggle;
                this.mTable.ShowToggle = showHeadersAndToggle;
                this.mTable.ObjectTable.NoAutoSizeGridResize = true;
                this.mTable.Populate(listObjs, headers, numSelectableRows);
                this.mTable.ViewTypeToggle = viewTypeToggle;
                this.mPreSelectedRows = preSelectedRows;
                this.mTable.TablePopulationComplete += new VoidEventHandler(this.OnPopulationCompleted);
                if (!this.mTable.ShowToggle)
                {
                    Window window = this.mModalDialogWindow.GetChildByID(99576788u, false) as Window;
                    Window window2 = this.mModalDialogWindow.GetChildByID(99576789u, false) as Window;
                    this.mTable.Area = new Rect(this.mTable.Area.TopLeft.x, this.mTable.Area.TopLeft.y - 64f, this.mTable.Area.BottomRight.x, this.mTable.Area.BottomRight.y);
                    window2.Area = new Rect(window2.Area.TopLeft.x, window2.Area.TopLeft.y - 64f, window2.Area.BottomRight.x, window2.Area.BottomRight.y);
                    window.Area = new Rect(window.Area.TopLeft.x, window.Area.TopLeft.y - 64f, window.Area.BottomRight.x, window.Area.BottomRight.y);
                }
                this.mModalDialogWindow.Area = new Rect(this.mModalDialogWindow.Area.TopLeft, this.mModalDialogWindow.Area.TopLeft + this.mTable.TableArea.BottomRight + this.mTableOffset);
                Rect area = this.mModalDialogWindow.Area;
                float num = area.BottomRight.x - area.TopLeft.x;
                float num2 = area.BottomRight.y - area.TopLeft.y;
                if (!this.mTable.ShowToggle)
                {
                    num2 -= 50f;
                }
                float num3 = position.x;
                float num4 = position.y;
                if (num3 < 0f && num4 < 0f)
                {
                    Rect area2 = this.mModalDialogWindow.Parent.Area;
                    float num5 = area2.BottomRight.x - area2.TopLeft.x;
                    float num6 = area2.BottomRight.y - area2.TopLeft.y;
                    num3 = (float)Math.Round((double)((num5 - num) / 2f));
                    num4 = (float)Math.Round((double)((num6 - num2) / 2f));
                }
                area.Set(num3, num4, num3 + num, num4 + num2);
                this.mModalDialogWindow.Area = area;
                this.mModalDialogWindow.Visible = true;
            }
        }
        public void OnCloseButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            eventArgs.Handled = true;
            base.EndDialog(base.CancelID);
        }
        public override bool OnEnd(uint endID)
        {
            if (endID == base.OkayID)
            {
                if (!this.mOkayButton.Enabled)
                {
                    return false;
                }
                this.mResult = this.mTable.Selected;
            }
            else
            {
                this.mResult = null;
            }
            this.mTable.Populate(null, null, 0);
            return true;
        }
        public void OnOkayButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            eventArgs.Handled = true;
            base.EndDialog(base.OkayID);
        }
        public void OnPopulationCompleted()
        {
            this.mTable.Selected = this.mPreSelectedRows;
        }
        public void OnSelectionChanged(List<ObjectPicker.RowInfo> selectedRows)
        {
            Audio.StartSound("ui_tertiary_button");
            this.OnTableChanged();
        }
        public void OnTableChanged()
        {
            List<ObjectPicker.RowInfo> selected = this.mTable.Selected;
            if (selected != null && selected.Count > 0)
            {
                this.mOkayButton.Enabled = true;
                return;
            }
            this.mOkayButton.Enabled = false;
        }
        public static List<ObjectPicker.RowInfo> Show(string title, string buttonTrue, string buttonFalse, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows)
        {
            return BigObjectPickerDialog.Show(true, ModalDialog.PauseMode.PauseTask, title, buttonTrue, buttonFalse, listObjs, headers, numSelectableRows, new Vector2(-1f, -1f), false);
        }
        public static List<ObjectPicker.RowInfo> Show(string title, string buttonTrue, string buttonFalse, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows, bool viewTypeToggle)
        {
            return BigObjectPickerDialog.Show(true, ModalDialog.PauseMode.PauseTask, title, buttonTrue, buttonFalse, listObjs, headers, numSelectableRows, new Vector2(-1f, -1f), viewTypeToggle);
        }
        public static List<ObjectPicker.RowInfo> Show(bool modal, ModalDialog.PauseMode pauseType, string title, string buttonTrue, string buttonFalse, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows)
        {
            return BigObjectPickerDialog.Show(modal, pauseType, title, buttonTrue, buttonFalse, listObjs, headers, numSelectableRows, new Vector2(-1f, -1f), false);
        }
        public static List<ObjectPicker.RowInfo> Show(string title, string buttonTrue, string buttonFalse, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows, List<ObjectPicker.RowInfo> preSelectedRows, bool showHeadersAndToggle)
        {
            return BigObjectPickerDialog.Show(true, ModalDialog.PauseMode.PauseSimulator, title, buttonTrue, buttonFalse, listObjs, headers, numSelectableRows, new Vector2(-1f, -1f), true, preSelectedRows, showHeadersAndToggle);
        }
        public static List<ObjectPicker.RowInfo> Show(bool modal, ModalDialog.PauseMode pauseType, string title, string buttonTrue, string buttonFalse, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows, Vector2 position)
        {
            return BigObjectPickerDialog.Show(modal, pauseType, title, buttonTrue, buttonFalse, listObjs, headers, numSelectableRows, position, false);
        }
        public static List<ObjectPicker.RowInfo> Show(bool modal, ModalDialog.PauseMode pauseType, string title, string buttonTrue, string buttonFalse, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows, Vector2 position, bool viewTypeToggle)
        {
            return BigObjectPickerDialog.Show(modal, pauseType, title, buttonTrue, buttonFalse, listObjs, headers, numSelectableRows, position, viewTypeToggle, null);
        }
        public static List<ObjectPicker.RowInfo> Show(bool modal, ModalDialog.PauseMode pauseType, string title, string buttonTrue, string buttonFalse, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows, Vector2 position, bool viewTypeToggle, List<ObjectPicker.RowInfo> preSelectedRows)
        {
            return BigObjectPickerDialog.Show(modal, pauseType, title, buttonTrue, buttonFalse, listObjs, headers, numSelectableRows, position, viewTypeToggle, preSelectedRows, true);
        }
        public static List<ObjectPicker.RowInfo> Show(bool modal, ModalDialog.PauseMode pauseType, string title, string buttonTrue, string buttonFalse, List<ObjectPicker.TabInfo> listObjs, List<ObjectPicker.HeaderInfo> headers, int numSelectableRows, Vector2 position, bool viewTypeToggle, List<ObjectPicker.RowInfo> preSelectedRows, bool showHeadersAndToggle)
        {
            List<ObjectPicker.RowInfo> result;
            using (BigObjectPickerDialog bigObjectPickerDialog = new BigObjectPickerDialog(modal, pauseType, title, buttonTrue, buttonFalse, listObjs, headers, numSelectableRows, position, viewTypeToggle, preSelectedRows, showHeadersAndToggle))
            {
                bigObjectPickerDialog.StartModal();
                if (bigObjectPickerDialog.Result == null || bigObjectPickerDialog.Result.Count == 0)
                {
                    result = null;
                }
                else
                {
                    result = bigObjectPickerDialog.Result;
                }
            }
            return result;
        }
    }
}
