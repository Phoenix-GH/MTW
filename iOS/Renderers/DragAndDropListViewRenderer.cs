using System;
using Xamarin.Forms;
using MyTenantWorld.iOS;
using MyTenantWorld;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Collections;

[assembly:ExportRenderer(typeof(DraggableListView), typeof(DragAndDropListViewRenderer))]
namespace MyTenantWorld.iOS
{
	public class DragAndDropListViewRenderer : ListViewRenderer , IUIGestureRecognizerDelegate
	{

		internal static Dictionary<string,Tuple<UITableView,IList>> ListMap=new Dictionary<string,Tuple<UITableView,IList>>();

		private class ReorderableTableViewSource : UITableViewSource
		{
			public WeakReference<ListView> View { get; set; }

			public UITableViewSource Source { get; set; }

			#region A replacement UITableViewSource which enables drag and drop to reorder rows

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath) {
				return Source.GetCell(tableView, indexPath);
			}

			public override nfloat GetHeightForHeader(UITableView tableView, nint section) {
				return Source.GetHeightForHeader(tableView, section);
			}

			public override UIView GetViewForHeader(UITableView tableView, nint section) {
				return Source.GetViewForHeader(tableView, section);
			}

			public override nint NumberOfSections(UITableView tableView) {
				return Source.NumberOfSections(tableView);
			}

			public override void RowDeselected(UITableView tableView, NSIndexPath indexPath) {
				Source.RowDeselected(tableView, indexPath);
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath) {
				Source.RowSelected(tableView, indexPath);
			}

			public override nint RowsInSection(UITableView tableview, nint section) {
				return Source.RowsInSection(tableview, section);
			}

			public override string[] SectionIndexTitles(UITableView tableView) {
				return Source.SectionIndexTitles(tableView);
			}

			public override string TitleForHeader(UITableView tableView, nint section) {
				return Source.TitleForHeader(tableView, section);
			}

			#endregion

			public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath) {
				// Don't call the base method, which is the key: the same method in
				// ListViewRenderer.ListViewDataSource throws which prevents the rows to become moveable

				// TODO: do things to actually reorder in data model, such as raise a reorder event, etc.
			}
		}

		private new ListView Element => base.Element as ListView;
		protected override void OnElementChanged (ElementChangedEventArgs<ListView> e)
		{
			
			base.OnElementChanged (e);

			if (e.NewElement != null) 
			{
				// Make row reorderable
				Control.Tag = (nint)Control.GetHashCode ();

				ListMap.Add (Control.Tag.ToString(),new Tuple<UITableView,IList>(Control,(IList)this.Element.ItemsSource));

				Control.Source = new ReorderableTableViewSource { View = new WeakReference<ListView>(Element), Source = Control.Source };
			}

	
		}

		protected override void Dispose (bool disposing)
		{
			if(Control != null)
				ListMap.Remove (Control.Tag.ToString());

			base.Dispose(disposing);
		}

	}
}

