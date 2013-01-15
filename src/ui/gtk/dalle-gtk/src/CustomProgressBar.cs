/*

	Dalle - A split/join file utility library
	Dalle.UI.DalleGtk.CustomProgressBar - 
		ProgressBar that shows text percentage.
		
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

using Gtk;

namespace Dalle.UI.DalleGtk
{
	
	public class CustomProgressBar : Gtk.ProgressBar
	{
		public CustomProgressBar () : base()
		{
			this.Text = "0 %";
		}
		
		public new double Fraction{
			get{
				return base.Fraction;
			}
			set{
				if ((value >=0) && (value <= 1)){
					base.Text = "" + ((int) (value * 100)) + " %";
					base.Fraction = value;					
				}				
			}
		}
	}
}
