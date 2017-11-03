using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MyTenantWorld.Droid;
using Android.Content;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MyTenantWorld;
using Android.Widget;

[assembly:ExportRenderer(typeof(DraggableListView), typeof(DragAndDropListViewRenderer))]
namespace MyTenantWorld.Droid
{
	public class DragAndDropListViewRenderer : ListViewRenderer , Android.Widget.AdapterView.IOnItemLongClickListener, Android.Views.View.IOnDragListener
	{

		internal static Dictionary<string,IList> ListMap=new Dictionary<string,IList>();

		protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.ListView> e)
		{
			base.OnElementChanged (e);

			if (e.NewElement != null) 
			{
				Control.OnItemLongClickListener = this;
				Control.SetOnDragListener (this);

				Control.Id = Control.GetHashCode ();

				ListMap.Add (Control.Id.ToString(),(IList)this.Element.ItemsSource);
			}

		
		}

		public bool OnItemLongClick (Android.Widget.AdapterView parent, Android.Views.View view, int position, long id)
		{

			var selectedItem = ((IList)Element.ItemsSource) [(int)id];
			TemporalObject tmpObj=new TemporalObject(view,selectedItem);
			ClipData data = ClipData.NewPlainText(string.Empty, string.Empty);
			DragShadowBuilder shadowBuilder = new global::Android.Views.View.DragShadowBuilder(view);

			view.StartDrag(data, shadowBuilder, tmpObj, 0);

			return true;
		}

		public bool OnDrag (Android.Views.View v, Android.Views.DragEvent e)
		{
			switch (e.Action) 
			{
			case  Android.Views.DragAction.Started:
				break;
			case  Android.Views.DragAction.Entered:
				//v.SetBackgroundColor(Android.Graphics.Color.Blue);
				break;
			case  Android.Views.DragAction.Exited:
				//v.SetBackgroundColor(Android.Graphics.Color.Transparent);
				break;
			case  Android.Views.DragAction.Drop:
				//v.SetBackgroundColor(Android.Graphics.Color.Transparent);
				TemporalObject passObj = (TemporalObject)e.LocalState;
				Android.Views.View view = passObj.View;
				object passedItem = passObj.Item;
				Android.Widget.ListView oldParent = (Android.Widget.ListView)view.Parent;

				BaseAdapter sourceAdapter = (oldParent.Adapter is IWrapperListAdapter)?((IWrapperListAdapter)oldParent.Adapter).WrappedAdapter as BaseAdapter:((BaseAdapter)oldParent.Adapter);


				Android.Widget.ListView newParent = Control;
				BaseAdapter destinationAdapter =(oldParent.Adapter is IWrapperListAdapter)?((IWrapperListAdapter)newParent.Adapter).WrappedAdapter as BaseAdapter:((BaseAdapter)oldParent.Adapter);

	
				if (DragAndDropListViewRenderer.ListMap.ContainsKey (oldParent.Id.ToString()) && DragAndDropListViewRenderer.ListMap.ContainsKey (newParent.Id.ToString())) 
				{
					var sourceList = (IList)DragAndDropListViewRenderer.ListMap [oldParent.Id.ToString()];
					var destinationList = (IList)DragAndDropListViewRenderer.ListMap [newParent.Id.ToString()];

						if(sourceList.Contains(passedItem))
						{
							sourceList.Remove (passedItem);
							destinationList.Add(passedItem);
						}

						destinationAdapter.NotifyDataSetChanged ();
						sourceAdapter.NotifyDataSetChanged ();
				}


				break;
			case  Android.Views.DragAction.Ended:
				break;
			}
			return true;
		}
		protected override void Dispose (bool disposing)
		{
			if(Control != null)
				ListMap.Remove (Control.Id.ToString());

			base.Dispose (disposing);
		}
	}
}

