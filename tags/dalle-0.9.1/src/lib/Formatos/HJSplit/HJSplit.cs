/*

	Dalle - A split/join file utility library
	Dalle.Formatos.HJSplit.HJSplit - 
		Split and Join files in HJSplit format.
	
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


namespace Dalle.Formatos.HJSplit
{
	public class HJSplit : Parte {
	
		public HJSplit ()
		{
			nombre = "hjsplit";
			descripcion = "HJsplit";
			web = "http://www.freebyte.com/";
			parteFicheros = false;
			compatible = false;			
		}
		protected override  void _Partir (string fichero, string sal1, string dir, long kb){
		
		}
		
	
		protected override void _Unir (string fichero, string dirDest) {
			//bool zipped = false;
			// Calcular zipped
			
			
			
			
		
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

