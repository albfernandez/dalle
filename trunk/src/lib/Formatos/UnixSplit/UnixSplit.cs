/*

	Dalle - A split/join file utility library
	Dalle.Formatos.HJSplit.HJSplit - 
		Split and Join files in HJSplit format.
	
    Copyright (C) 2003-2011  Alberto Fernández <infjaf00@yahoo.es>

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


namespace Dalle.Formatos.UnixSplit
{
	public class UnixSplit : Parte {
	
		public UnixSplit ()
		{
			nombre = "unixsplit";
			descripcion = "unixsplit";
			web = "http://";
			parteFicheros = false;
			compatible = true;			
		}
		protected override  void _Partir (string fichero, string sal1, string dir, long kb){
		
		}
		
	
		protected override void _Unir (string fichero, string dirDest) {
	
			
			
		
		}
		public override bool PuedeUnir (string fichero)
		{
			return false;
			/*
			if (! File.Exists (fichero) )
				return false;
			
			try{
				CabeceraHacha_v1.LeerCabecera (fichero);
				return true;
			}
			catch (Exception){
				return false;
			}*/
		}
	
	}


}

