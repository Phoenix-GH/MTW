using System;
using Xamarin.Forms;
using MyTenantWorld;
using MyTenantWorld.iOS;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using System.Collections;
using System.Linq;

[assembly:ExportRenderer(typeof(DraggableViewCell), typeof(DragAndDropViewCellRenderer))]
namespace MyTenantWorld.iOS
{
	public class DragAndDropViewCellRenderer : ViewCellRenderer
	{
		private static NSIndexPath sourceIndexPath, destinationIndexPath;
		private static UITableView sourceTableView;

		private class LongPressGestureRecognizer : UILongPressGestureRecognizer
		{
			private static UIView dragAndDropView;

			private WeakReference<UITableView> TableView { get; set; }

			private WeakReference<ViewCell> ViewCell { get; set; }

	

			public static LongPressGestureRecognizer CreateGesture(UITableView tableView, ViewCell cell) {
				return new LongPressGestureRecognizer(OnRecognizing) {
					TableView = new WeakReference<UITableView>(tableView),
					ViewCell = new WeakReference<ViewCell>(cell),
				};
			}

			private static void OnRecognizing(UILongPressGestureRecognizer r) {
				LongPressGestureRecognizer recognizer = r as LongPressGestureRecognizer;
				UITableView tableView;
				recognizer.TableView.TryGetTarget(out tableView);
				ViewCell cell;
				recognizer.ViewCell.TryGetTarget(out cell);
				if (tableView == null || cell == null) {
					return;
				}
				OnRecognizing(recognizer, tableView, cell);
			}

			private static void OnRecognizing(LongPressGestureRecognizer recognizer, UITableView tableView, ViewCell cell) {
				NSIndexPath indexPath = tableView.IndexPathForRowAtPoint(recognizer.LocationInView(tableView));
				switch (recognizer.State) {
				case UIGestureRecognizerState.Began:
					tableView.ScrollEnabled = false;
					if (indexPath != null) {
						// Remember the source row
						sourceIndexPath = indexPath;
						destinationIndexPath = indexPath;
						var selectedCell=tableView.CellAt (indexPath);
						UIGraphics.BeginImageContext (selectedCell.ContentView.Bounds.Size);
						selectedCell.ContentView.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
						UIImage img = UIGraphics.GetImageFromCurrentImageContext ();
						UIGraphics.EndImageContext ();

						UIImageView iv = new UIImageView (img);
						dragAndDropView = new UIView ();
						dragAndDropView.Frame = iv.Frame;
						dragAndDropView.Add (iv);
						//dragAndDropView.BackgroundColor = UIColor.Blue;
						sourceTableView = tableView;

						UIApplication.SharedApplication.KeyWindow.Add(dragAndDropView);

						dragAndDropView.Center = recognizer.LocationInView (UIApplication.SharedApplication.KeyWindow);

						dragAndDropView.AddGestureRecognizer (selectedCell.GestureRecognizers [0]);
					}
		
					break;
				case UIGestureRecognizerState.Changed:
					dragAndDropView.Center = recognizer.LocationInView (UIApplication.SharedApplication.KeyWindow);
					destinationIndexPath = indexPath;
					break;
				case UIGestureRecognizerState.Cancelled:
				case UIGestureRecognizerState.Failed:
					sourceIndexPath = null;
					cell.View.BackgroundColor = Color.Transparent;
					break;
				case UIGestureRecognizerState.Ended:

					if (dragAndDropView == null)
						return;
					
					dragAndDropView.RemoveFromSuperview ();
					dragAndDropView = null;

				
					UIView view = UIApplication.SharedApplication.KeyWindow;
				
					UIView viewHit = view.HitTest (recognizer.LocationInView (view), null);

					int removeLocation = (int)sourceIndexPath.Item;
					int insertLocation = destinationIndexPath != null ? (int)destinationIndexPath.Item : -1;
				
					UITableView destinationTableView = viewHit as UITableView;

					if (viewHit is UITableView) {
						destinationTableView = viewHit as UITableView;

					} else {
						while (!(viewHit is UITableViewCell) && viewHit != null) {
							viewHit = viewHit.Superview;
						}

						UIView tempView = viewHit?.Superview;

						while (!(tempView is UITableView) && tempView !=null) 
						{
							tempView = tempView.Superview;
						}

						if (tempView != null) 
						{
							destinationTableView = tempView as UITableView;
							insertLocation = (int)destinationTableView.IndexPathForCell ((UITableViewCell)viewHit).Item;
						}

					}
				
					if (destinationTableView != null) 
					{
						if (DragAndDropListViewRenderer.ListMap.ContainsKey (tableView.Tag.ToString()) && DragAndDropListViewRenderer.ListMap.ContainsKey (destinationTableView.Tag.ToString())) 
						{
							var sourceList = (IList)DragAndDropListViewRenderer.ListMap [tableView.Tag.ToString()].Item2;
							var destinationList = (IList)DragAndDropListViewRenderer.ListMap [destinationTableView.Tag.ToString()].Item2;
							if (!tableView.Tag.Equals (destinationTableView.Tag) || removeLocation != insertLocation) 
							{
								if(sourceList.Contains(cell.BindingContext))
								{
									sourceList.Remove (cell.BindingContext);

									if (insertLocation != -1)
										destinationList.Insert (insertLocation, cell.BindingContext);
									else
										destinationList.Add (cell.BindingContext);
								}
								tableView.ReloadData ();
								destinationTableView.ReloadData ();
							}
						}
					}


					tableView.ScrollEnabled = true;

					break;
				}
			}

			private LongPressGestureRecognizer(Action<UILongPressGestureRecognizer> action) : base(action) {
			}
		}

		public override UITableViewCell GetCell(Cell item, UITableViewCell reusedCell, UITableView tableView) {
			UITableViewCell tableViewCell = base.GetCell(item, reusedCell, tableView);
			AddGestures(item as ViewCell, tableViewCell, tableView);
			return tableViewCell;
		}

		private void AddGestures(ViewCell cell, UITableViewCell tableViewCell, UITableView tableView) {
			tableViewCell.AddGestureRecognizer(LongPressGestureRecognizer.CreateGesture(tableView, cell));
		}

	}
}

