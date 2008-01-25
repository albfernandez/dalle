// created on 05/09/2004 at 16:25



using Gtk;
using System;
using System.Reflection;

namespace Dalle.UI.DalleGtk
{
	public class AboutDialog : Gtk.AboutDialog 
	{
	
		private static AboutDialog instance = null;

		public static AboutDialog Instance {
			get {
				if (instance == null){
					instance = new AboutDialog();
				}
				return instance;
			}
		}
		
		
		protected AboutDialog () : base ()
		{
			Assembly asm = Assembly.GetExecutingAssembly ();
			this.Name = (asm.GetCustomAttributes (
               typeof (AssemblyTitleAttribute), false) [0]
               as AssemblyTitleAttribute).Title;
          
          	this.Version = asm.GetName ().Version.ToString ();
          
          this.Comments = (asm.GetCustomAttributes (
               typeof (AssemblyDescriptionAttribute), false) [0]
               as AssemblyDescriptionAttribute).Description;
          
          this.Copyright = (asm.GetCustomAttributes (
               typeof (AssemblyCopyrightAttribute), false) [0]
               as AssemblyCopyrightAttribute).Copyright;
          this.Website = "http://dalle.sourceforge.net";
          this.License = "Licencia";
          
/*@"Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
\"Software\"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";*/
          
          this.Authors =  new string[] {
				"Alberto Fernández <infjaf00@yahoo.es>",
				"Ramón Rey Vicente <ramon.rey@hispalinux.es>",
				"Álvaro Peña <apg@esware.com>",
				"Eduardo García Cebollero <kiwnix@yahoo.es>", 
				"Daniel Martinez Contador <dmcontador@terra.es>",
				"Dai SET <dai__set@yahoo.com>"
			};
			this.Response += new ResponseHandler (close_click);
			this.DeleteEvent += new DeleteEventHandler (this.HideDialog);
			this.WindowPosition = WindowPosition.CenterOnParent;
	     
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
