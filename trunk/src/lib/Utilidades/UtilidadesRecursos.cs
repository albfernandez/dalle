/*

    Dalle - A split/join file utility library	
    Dalle.Utilidades.Recursos - Resource utilities.
	
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

using System.Reflection;
using System.Resources;
using System;


namespace Dalle.Utilidades
{
	
	public class UtilidadesRecursos
	{
	
		/// <summary>ResourceManager encargado de cargar los recursos de
		/// imágenes.</summary>
		
		private static ResourceManager resPixmaps = null;
		
		/// <summary>Constructor privado: No se permite crear instancias
		/// de esta clase.</summary>		
		
		private UtilidadesRecursos()
		{
		}
		
		/// <summary>Obtiene un array de bytes a partir del nombre de la imagen.
		/// </summary>
		/// <param name="nombre">El nombre de la imagen a cargar.</param>
		/// <returns>La imagen, una por defecto si no existe una
		/// para <c>nombre</c>.</returns>
		
		public static byte[] GetImagen2 (String nombre)
		{
			if (resPixmaps == null){
				resPixmaps = new ResourceManager 
						("Pixmaps", typeof (UtilidadesRecursos).Assembly);
			}       		
			byte[]img = (byte[])resPixmaps.GetObject (nombre);
			return img;
		}
		
		public static byte[] GetImagen (String resource, Assembly assembly) 
		{
			System.IO.Stream s = assembly.GetManifestResourceStream (resource);
			if (s == null)
			{
				throw new ArgumentException ("'" + resource + "' is not a valid resource name of assembly '" + assembly + "'.");
			}
			byte[] ret = new byte[s.Length];			
			s.Read(ret, 0, ret.Length);
			s.Close();
			return ret;		
		}
		
		public static byte[] GetImagen (String resource) 
		{
			return GetImagen(resource, typeof (UtilidadesRecursos).Assembly);				
		}
	}
}
