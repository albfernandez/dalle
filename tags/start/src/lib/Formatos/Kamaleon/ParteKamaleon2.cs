/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Kamaleon.ParteKamaleon2 - Join files in KamaleoN 2 format.
	
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
using System.IO;

namespace Dalle.Formatos.Kamaleon
{
	public class ParteKamaleon2 : ParteKamaleon
	{
		public ParteKamaleon2()
		{
			nombre = "kamaleon2";
			descripcion = "KamaleoN v 2.0";
			web = "http://www.kamaleonsoft.com";
			compatible = false;
			parteFicheros = false;

		}
		protected override void _Unir (string fichero, string dirDest)
		{
			base._Unir (fichero, dirDest);			
		}
		protected override void _Partir (String fichero,String salida1, String dir, long kb)
		{
			_Partir (fichero, salida1, dir, kb, "2");
		}
		public override bool PuedeUnir (String fichero)
		{
			return (VersionKamaleon(fichero) == "2");
		}
	}




}
