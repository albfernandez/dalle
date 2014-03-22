/*

	Dalle - A split/join file utility library
	Dalle.UI.DalleGtk.DalleGtk - Main Window
		
    Copyright (C) 2003-2013  Alberto Fernández <infjaf@gmail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.


*/
using System;
using System.Reflection;

using Gtk;
using Gdk;
using GtkSharp;

using Mono.Unix;

namespace Dalle.UI.DalleGtk
{	
	public class DalleGtk : Gtk.Window
	{
		/*
		 * Los widgets de la ventana
		 */
		Gtk.VBox vbox;
		Gtk.VBox vbox2;
		
		Gtk.HBox hbox1;
		Gtk.HBox hbox2;
		Gtk.HBox hbox3;

		Gtk.Label texto;
		Gtk.Label texto2;
		
		Gtk.RadioButton SplitOption;	
		Gtk.RadioButton PasteOption;
		
		Gtk.HButtonBox hbbox;

		Gtk.Button run;
		Gtk.Button about;
		Gtk.Button exit;
		
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
			this.Title = Catalog.GetString("DalleGtk");
			this.SetDefaultSize(350,250);
			this.DeleteEvent += new DeleteEventHandler (WindowExit);
			
			this.InitComponents();
			this.WindowPosition = Gtk.WindowPosition.Center; 
			try{
				this.Icon = new Gdk.Pixbuf (Assembly.GetExecutingAssembly(), "gears.png");
			}
			catch (Exception){
			}
			
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
			vbox = new Gtk.VBox (false, 4);
			
			/*
			 * Añadiendo las cajas horizontales donde se van
			 * a distribuir los widgets
			 */
			hbox1 = new Gtk.HBox (false, 1);
			hbox2 = new Gtk.HBox (false, 1);
			hbox3 = new Gtk.HBox (false, 1);
			
			/*
			 * Primera caja horizontal
			 */
			texto = new Gtk.Label (
				String.Format (
				"<big><b>{0}</b></big>",
				Catalog.GetString("Welcome to Dalle.")));
			texto.UseMarkup = true;

			hbox1.PackStart(texto, false , false, 7);
			
			/*
			 * Segunda caja horizontal
			 */
			vbox2 = new Gtk.VBox (false, 3);
			texto2 = new Gtk.Label (Catalog.GetString("What do you want to do?"));
			
			PasteOption = new Gtk.RadioButton (Catalog.GetString ("Merge Files"));
			
			SplitOption = new Gtk.RadioButton (PasteOption, Catalog.GetString("Split Files"));

			vbox2.PackStart(texto2, false, false, 7);
			vbox2.PackStart(PasteOption, false , false, 7);
			vbox2.PackStart(SplitOption, false, false, 7);
			
			hbox2.PackStart(vbox2, false, false, 7);
			
			/*
			 * Tercera caja horizontal
			 */
			hbbox = new Gtk.HButtonBox();
			
			run = new Gtk.Button(Gtk.Stock.Execute);			
			exit = new Gtk.Button(Gtk.Stock.Quit);
			about = new Gtk.Button(Catalog.GetString("About"));
			

			hbbox.PackStart(about, false, false, 7);
			
			hbbox.PackStart(exit, false, false, 7);
			hbbox.PackStart(run, false, false, 7);			
			
			
			hbox3.PackEnd(hbbox, false ,false, 7);

			
			/*
			 * Empaquetando las cajas horizontales en la caja
			 * vertical que ocupa la ventana principal
			 */
			vbox.PackStart(hbox1, false, false, 7);
			vbox.PackStart(hbox2, false, false, 7);
			vbox.PackEnd(hbox3, false, false, 7);
			
			this.Add (vbox);

			/*
			 * Eventos
			 */
			run.Clicked += new EventHandler(run_click);
			exit.Clicked += new EventHandler(exit_click);
			about.Clicked += new EventHandler(about_click);
		}
		private void about_click (object sender, EventArgs args)
		{
			AboutDialog.Instance.ShowAll();
		}
		private void exit_click (object sender, EventArgs args)
		{
			this.WindowExit();
		}	
		private void run_click (object sender, EventArgs args)
		{
			if (SplitOption.Active == true){
				SplitDialog.Instance.ShowAll();
			}
			if (PasteOption.Active == true){
				JoinDialog.Instance.ShowAll();
			}
		}		
		public static void HideWindow (object o, DeleteEventArgs args)
		{
			Gtk.Window w = o as Gtk.Window;
			w.Hide();
			if (args != null){
				args.RetVal = true;
			}
		}		
		public static void Main (String[] args)
		{
			Dalle.I18N.GettextCatalog.Init();
			Application.Init();
			DalleGtk v = DalleGtk.Instance;
			v.ShowAll();
			Application.Run ();
		}
	}
}
