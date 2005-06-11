/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Generico.InfoGenerico
	
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
	
using Dalle.Utilidades;

namespace Dalle.Formatos.Generico
{	
	
	public class InfoGenerico 
	{
		private string formato = string.Empty;
		private int fragmento = 0;
		private int digitos = 3;
		
		public string Formato{
			get{ return formato; }
			set{ formato = value; }
		}
		public int Fragmento{
			get{ return fragmento; }
			set{ fragmento = value;}
		}
		public int Digitos{
			get{ return digitos; }
			set{ digitos = value; }
		}
		public InfoGenerico()
		{		
		}
		public override string ToString()
		{
			return string.Format (
				formato, 
				UtilidadesCadenas.Format(Fragmento,Digitos)
			);
		}
		
		
	}
}
