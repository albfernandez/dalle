/*

    Dalle - A split/join file utility library	
    Dalle.Utilidades.UtilidadesFicheros - Useful string functions.
	
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

namespace Dalle.Utilidades
{
	
	public class UtilidadesCadenas
	{
		/// <summary>Constructor privado. No se permite crear instancias
		/// de esta clase.</summary>
	
		private UtilidadesCadenas ()
		{		
		}
		
		/// <summary>Formatea un número a cadena de texto con un determinado
		/// número de dígitos. Rellena con ceros a la izquierda.</summary>
		/// <param name="numero">El número a formatear.</param>
		/// <param name="digitos">El número de digitos de la salida.</param>
		/// <returns>El número formateado</returns>
		
		public static string Format (int numero, int digitos)
		{
			String ret = "" + numero;			
			while (ret.Length < digitos)
				ret = "0" + ret;
				
			return ret;
		}
	}
}