/*

	Dalle - A split/join file utility library
	Dalle.UI.DalleGtk.DalleGtk - Main Window
		
    Copyright (C) 2003  Alberto Fernández <infjaf00@yahoo.es>

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

*/
using System;
using System.Reflection;

using Gtk;
using Gdk;
using GtkSharp;

using I = Dalle.I18N.GetText;

namespace Dalle.UI.DalleGtk
{	
	public class DalleGtk : Gtk.Window
	{
		private static DalleGtk instance = null;
				
		public static DalleGtk Instance{
			get{
				if (instance == null){
					instance = new DalleGtk();
				}
				return instance;
			}
		}

		private DalleGtk () : base (Gtk.WindowType.Toplevel)
		{
			this.Title = "DalleGtk";
			this.SetDefaultSize(300,200);
			this.DeleteEvent += new DeleteEventHandler (WindowExit);
			
			this.InitComponents();
			this.Icon = new Gdk.Pixbuf (null, "gears.png");
			
			
		}
	
		public static void DoEvents ()
		{
			while (Application.EventsPending()){
				Application.RunIteration();
			}
		}
		private void WindowExit (object o, DeleteEventArgs args)
		{
			WindowExit ();
			args.RetVal = true;
		}
		private void WindowExit()
		{
			Application.Quit ();
			System.Environment.Exit(0);
		}
		private void InitComponents()
		{
			Gtk.Button SplitButton = new Gtk.Button (Gtk.Stock.Cut);
			Gtk.Button JoinButton  = new Gtk.Button (Gtk.Stock.Paste);
			SplitButton.Clicked += new EventHandler (this.SplitFiles);
			JoinButton.Clicked += new EventHandler (this.JoinFiles);

			Gtk.VBox vbox = new Gtk.VBox (false, 2);

			vbox.PackStart(SplitButton);
			vbox.PackStart(JoinButton);

			
			this.Add (vbox);
		}
		private void SplitFiles (object sender, EventArgs args)
		{
			SplitDialog.Instance.ShowAll();
		}
		private void JoinFiles (object sender, EventArgs args)
		{
			JoinDialog.Instance.ShowAll ();
		}
		
		public static void HideWindow (object o, DeleteEventArgs args)
		{
			Gtk.Window w = o as Gtk.Window;
			w.Hide();
			if (args != null)
				args.RetVal = true;
		}
		
		public static void Main (String[] args)
		{
			Application.Init();
			DalleGtk v = DalleGtk.Instance;
			v.SetSizeRequest (300,200);
			v.ShowAll();
			Application.Run ();
		}
	}
}
