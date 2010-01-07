/*

    Dalle - A split/join file utility library
    Dalle.Utilidades.UtilidadesFicheros - Useful array functions.
	
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
using System.Text;

namespace Dalle.Utilidades
{
	/// <summary>Esta clase proporciona métodos estáticos para trabajar 
	/// con arrays de bytes.</summary>

	public class UtArrays
	{
	
		/// <summary> Constructor privado. No se permite crear instancias
		/// de esta clase.</summary>
		
		private UtArrays()
		{
		}
		
		/// <summary>Escribe una cadena de texto en el array desde una posición
		/// dada.</summary>
		/// <remarks>Si el texto no cabe en el array, se trunca.</remarks>
		/// <param name="bytes">El array donde se escribira el texto.</param>
		/// <param name="text">El texto a escibir</param>
		/// <param name="pos_ini">Posición a partir de la cual se escribirá
		/// el texto</param>
		
		public static void EscribirTexto (byte[] bytes, String text, int pos_ini)
		{
			if ((text.Length + pos_ini) > bytes.Length)
				text = text.Substring (0, bytes.Length - pos_ini);
				
			foreach (char c in text)
				bytes[pos_ini++] = Convert.ToByte(c);
		}
		
		/// <summary>Lee una cadena de texto desde una posición dada.</summary>
		/// <remarks>Lee hasta que se acabe el array o hasta que encuentre
		/// un valor nulo '\0' </remarks>
		/// <param name="bytes">El array en el que se leerá</param>
		/// <param name="pos_ini">Indice a partir del que se leerá</param>
		/// <returns>La cadena que se encuentra a partir de pos_ini</returns>
		
		public static string LeerTexto (byte[] bytes, int pos_ini)
		{
			return LeerTexto (bytes, pos_ini, System.Int32.MaxValue);
		}
		
		/// <summary>Lee una cadena de texto desde una posición dada.</summary>
		/// <remarks>Lee hasta que se acabe el array, hasta que encuentre
		/// un valor nulo '\0', o hasta length bytes </remarks>
		/// <param name="bytes">El array en el que se leerá</param>
		/// <param name="pos_ini">Indice a partir del que se leerá</param>
		/// <param name="length">El número máximo de bytes a leer.</param>
		/// <returns>La cadena que se encuentra a partir de pos_ini</returns>
		
		public static string LeerTexto (byte[] bytes, int pos_ini, int length)
		{
			StringBuilder sb = new StringBuilder ();
			for (int i = pos_ini; (i < bytes.Length) && (bytes[i] != 0) && ((i - pos_ini) < length); i++)
					sb.Append(Convert.ToChar (bytes[i]));
			return sb.ToString();
		}
		
		/// <summary>Lee un entero de 16 bits a partir de una posición.
		/// </summary>
		/// <param name="bytes">El array en el que se leerá</param>
		/// <param name="pos_ini">Posición en la que se encuentra el entero.
		/// </param>
		/// <returns>El entero</returns>
		
		public static short LeerInt16 (byte[] bytes, int pos_ini)
		{
			byte b0, b1;
			b0 = bytes [pos_ini];
			b1 = bytes [pos_ini + 1];
			
			return ((short) (b0 | (b1 << 8)));
		}
		
		
		/// <summary>Escribe un entero de 32 bits en la posición dada.</summary>
		/// <param name="bytes">El array en el que se escribirá</param>
		/// <param name="value">El valor entero que se escribirá</param>
		/// <param name="pos">La posición en la que se escribirá</param>
		
		public static void EscribirInt (byte[] bytes, short value, int pos)
		{
			int i0, i1;
			
			i0 = value & 0x000000FF;
			i1 = (value & 0x0000FF00) >> 8;
		
			bytes[pos] = (byte) i0;
			bytes[pos+1] = (byte) i1;
		}
		
			
		
		/// <summary>Lee un entero de 32 bits a partir de una posición.
		/// </summary>
		/// <param name="bytes">El array en el que se leerá</param>
		/// <param name="pos_ini">Posición en la que se encuentra el entero.
		/// </param>
		/// <returns>El entero</returns>
		
		public static int LeerInt32 (byte[] bytes, int pos_ini)
		{
			byte b0, b1, b2, b3;
			b0 = bytes[pos_ini];
			b1 = bytes[pos_ini + 1];
			b2 = bytes[pos_ini + 2];
			b3 = bytes[pos_ini + 3];
			
			return ((int) (b0 | (b1 << 8) | (b2 << 16) | (b3 << 24)));			
		}
		
		/// <summary>Escribe un entero de 32 bits en la posición dada.</summary>
		/// <param name="bytes">El array en el que se escribirá</param>
		/// <param name="value">El valor entero que se escribirá</param>
		/// <param name="pos">La posición en la que se escribirá</param>
		
		public static void EscribirInt (byte[] bytes, int value, int pos)
		{
			int i0, i1, i2, i3;
			
			i0 = value & 0x000000FF;
			i1 = (value & 0x0000FF00) >> 8;
			i2 = (value & 0x00FF0000) >> 16;
			i3 = unchecked ((value & (int)0xFF000000)>>24);
		
			bytes[pos] = (byte) i0;
			bytes[pos+1] = (byte) i1;
			bytes[pos+2] = (byte) i2;
			bytes[pos+3] = (byte) i3;
		}
		
		/// <summary>Lee un entero de 64 bits a partir de una posición.
		/// </summary>
		/// <param name="bytes">El array en el que se leerá</param>
		/// <param name="pos_ini">Posición en la que se encuentra el entero.
		/// </param>
		/// <returns>El entero</returns>
		
		public static long LeerInt64 (byte[] bytes, int pos_ini)
		{
			long v1;
			long i0, i1;
			
			i0 = LeerInt32(bytes, pos_ini);
			i1 = LeerInt32(bytes, pos_ini + 4);
			v1 = (i0 & 0xFFFFFFFF) | (i1 << 32);
			return v1;
		}
		
		/// <summary>Escribe un entero de 64 bits en la posición dada.</summary>
		/// <param name="bytes">El array en el que se escribirá</param>
		/// <param name="value">El valor entero que se escribirá</param>
		/// <param name="pos">La posición en la que se escribirá</param>
		
		public static void EscribirInt (byte[] bytes, long value, int pos)
		{
			long i0, i1;
			i0 = value & 0x00000000FFFFFFFF;
			i1 = (value >> 32) & 0x00000000FFFFFFFF;
			EscribirInt (bytes, (int) i0, pos);
			EscribirInt (bytes, (int) i1, pos+4);
			
		}
		
		public static void EscribirDateTime (byte[] bytes, DateTime value, int pos)
		{
		
			long i0, i1;
			i0 = value.Ticks & 0x00000000FFFFFFFF;
			i1 = (value.Ticks >> 32) & 0x00000000FFFFFFFF;
			EscribirInt (bytes, (int) i1, pos);
			EscribirInt (bytes, (int) i0, pos+4);
		
		}
		
		
		public static DateTime LeerDateTime (byte[] bytes, int pos_ini)
		{
			long v1;
			long i0, i1;
			
			i0 = LeerInt32(bytes, pos_ini);
			i1 = LeerInt32(bytes, pos_ini + 4);
			v1 = (i1 & 0xFFFFFFFF) | (i0 << 32);

			return new DateTime(v1);
		}
	}
}
