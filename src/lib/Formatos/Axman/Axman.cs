/*

    Dalle - A split/join file utility library	
    Dalle.Formatos.Axman.Axman - 
	
    Copyright (C) 2003-2009  Alberto Fernández <infjaf00@yahoo.es>

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

using Dalle.Formatos;

namespace Dalle.Formatos.Axman
{
	public class Axman : Parte
	{
		public Axman() : base ("axman", "Axman", "http://www.mosaicware.com/", false, false)
		{
		}
		protected Axman (string nombre, string descripcion, string web, bool parteFicheros, bool compatible):
			base (nombre, descripcion, web, parteFicheros, compatible)
		{
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			// TODO:Implementar esta función.
		}
		protected override void _Partir (string fichero,string sal1, string dir, long kb)
		{
			// TODO:Implementar esta función.
		}
		public override bool PuedeUnir (string fichero)
		{
			//TODO: Implementar esta función
			return false;
		}
	}
}
