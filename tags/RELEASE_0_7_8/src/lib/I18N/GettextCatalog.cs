/*

    Dalle - A split/join file utility library	
    Dalle.I18N.GetText - Internationalization related stuff.
	
    Copyright (C) 2008  Alberto Fernández <infjaf00@yahoo.es>

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
using Mono.Unix;
using System.IO;

namespace Dalle.I18N {
	public class GettextCatalog {

		public static void Init ()
		{
			if (Directory.Exists("/usr/share/locale/"))
			{
				Catalog.Init("dalle", "/usr/share/locale/");
			}
			else if (Directory.Exists("./locale"))
			{
				Catalog.Init("dalle", "./locale");
			}
			else 
			{
				Catalog.Init("dalle", "/usr/share/locale/");
			}
		}
		private GettextCatalog () 
		{

		}
	}
}
