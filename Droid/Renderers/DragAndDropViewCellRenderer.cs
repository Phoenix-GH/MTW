using System;
using Xamarin.Forms;
using MyTenantWorld.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using MyTenantWorld;

[assembly:ExportRenderer(typeof(DraggableViewCell), typeof(DragAndDropViewCellRenderer))]
namespace MyTenantWorld.Droid
{
	public class DragAndDropViewCellRenderer : ViewCellRenderer
	{
		protected override global::Android.Views.View GetCellCore (Xamarin.Forms.Cell item, global::Android.Views.View convertView, global::Android.Views.ViewGroup parent, global::Android.Content.Context context)
		{
			var cell = base.GetCellCore (item, convertView, parent, context) as ViewGroup;
		
			cell.SetOnDragListener (new ItemDragListener(cell));
			return cell;
		}
	}
}

