/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Dalle1.Dalle
	
    Copyright (C) 2003-2010  Alberto Fernández <infjaf@gmail.com>

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
using System.IO;

using Dalle.Formatos;
using Dalle.Utilidades;
namespace Dalle.Formatos.Dalle1
{
	public class Dalle : Parte
	{
		public Dalle ()
		{
			nombre = "dalle";
			descripcion = "Dalle";
			web = "http://dalle.sourceforge.net";
			compatible = true;
			parteFicheros = true;
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			throw new NotImplementedException ();
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			FileInfo fi = null;
			DirectoryInfo din = null;
			DirectoryInfo dout = new DirectoryInfo (dir);
			if (File.Exists (fichero))
			{
				fi = new FileInfo (fichero);
				Console.WriteLine ("Es un fichero");
			}
			if (Directory.Exists (fichero))
			{
				din = new DirectoryInfo (fichero);
				Console.WriteLine ("es un directorio");
			}
			if ((sal1 == null) || (sal1 == string.Empty)) {
				//
				if (din != null) {
					sal1 = din.Name;
				}
				if (fi != null) {
					sal1 = fi.Name;
				}
			}
			DalleSplitter ds = null;
			if (din != null) {
				ds = new DalleSplitter (din, dout, sal1, kb);
			}
			else if (fi != null) {
				ds = new DalleSplitter (fi, dout, sal1, kb);
			}
			if (ds !=null){
				ds.Do();
			}
			else {
				throw new Exception("Error");
			}
			
			
			
			
			//throw new NotImplementedException ();
		}
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero)) {
				return false;
			}
			return fichero.ToLower ().EndsWith (".sha512sum.dalle");
		}
	}
}

