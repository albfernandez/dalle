/*

    Dalle - A split/join file utility library	
    
	
    Copyright (C) 2004-2010 Alberto Fernández  <infjaf@gmail.com>
    Original author (C - code) - Dai SET <dai_set@yahoo.com>
    C# translation by - Alberto Fernández  <infjaf@gmail.com>

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

namespace Dalle.Formatos.Camouflage
{
	public class CamouflageEntry
	{
		private string nombre;
		private long tamano;
		private int permisos;
		private DateTime creado;
		private DateTime accedido;
		private DateTime modificado;
		
		public CamouflageEntry ()
		{		
		
		}
		public string Nombre {
			get { return nombre; }
			set { nombre = value; }
		}
		public long Tamano {
			get { return tamano; }
			set { tamano = value; }
		}
		public int Permisos {
			get { return permisos; }
			set { permisos = value; }
		}
		public DateTime Creado {
			get { return creado; }
			set { creado = value; }
		}
		public DateTime Accedido {
			get { return accedido; }
			set { accedido = value; }
		}
		public DateTime Modificado {
			get { return modificado; }
			set { modificado = value; }
		}		
	}

}
