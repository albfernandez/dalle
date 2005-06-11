// created on 05/09/2004 at 16:25



using Gtk;
using System;
using System.Reflection;

namespace Dalle.UI.DalleGtk
{
	public class AboutDialog : Gtk.Dialog 
	{
	
		private static AboutDialog instance = null;
		
		public static AboutDialog Instance {
			get {
				if (instance == null){
					instance = new AboutDialog(DalleGtk.Instance);
				}
				return instance;
			}
		}
		
		
		protected AboutDialog (Gtk.Window parent) : base ("", parent, Gtk.DialogFlags.DestroyWithParent)
		{
			this.InitComponents();
			this.DeleteEvent += new DeleteEventHandler (this.HideDialog);
			try{
				this.Icon = new Gdk.Pixbuf (Assembly.GetExecutingAssembly() , "gears.png");
			}
			catch (Exception){
			}	
			this.SetSizeRequest (500, 350);	
			TransientFor = parent;
			Modal = true;
			Resizable = false;
			BorderWidth = 10;
			HasSeparator = false;			
			this.WindowPosition = Gtk.WindowPosition.CenterOnParent;
		}
		private void InitComponents ()
		{
			String[] authors = new String[] {
				"Alberto Fernández <infjaf00@yahoo.es>",
				"Ramón Rey Vicente <ramon.rey@hispalinux.es>",
				"Álvaro Peña <apg@esware.com>",
				"Eduardo García Cebollero <kiwnix@yahoo.es>", 
				"Daniel Martinez Contador <dmcontador@terra.es>",
				"Dai SET <dai__set@yahoo.com>"};
			
			
			
			string texto = "DalleGtk - " + Assembly.GetExecutingAssembly().GetName().Version;
			this.Title = texto;
			Gtk.HBox hboxTitle = new Gtk.HBox (true, 10);
			Gtk.Label lblPrograma = new Gtk.Label ("<big><b>" + texto +	"</b></big>");
			lblPrograma.UseMarkup = true;
			hboxTitle.PackStart (lblPrograma);
			 
			//Gtk.HBox libVersion = new Gtk.HBox (true, 10);
			//libVersion.PackStart (new Gtk.Label("libDalle - " ));
			
			Gtk.VBox boxAuthors = new Gtk.VBox (false,0);
			for (int i=0; i < authors.Length; i++){
				Gtk.Label lbl = new Gtk.Label(authors[i]);
				boxAuthors.PackStart(lbl);
			}
			
			//this.VBox.PackStart()
			this.VBox.PackStart(hboxTitle);
			//this.VBox.PackStart(libVersion);
			this.VBox.PackStart(boxAuthors);
			
			Gtk.Button close = new Gtk.Button(Gtk.Stock.Close);
			this.ActionArea.PackEnd(close);
			close.Clicked += new EventHandler (close_click);
		}
		private void close_click (object sender, EventArgs args)
		{
			this.Hide();
		}
		protected void HideDialog (object sender, DeleteEventArgs args)
		{	
			if (args != null)
				args.RetVal = true;
			this.Hide ();
		}
	
	}	
}
