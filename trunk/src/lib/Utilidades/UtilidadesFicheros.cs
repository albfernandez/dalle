/*

    Dalle - A split/join file utility library	
    Dalle.Utilidades.UtilidadesFicheros - Useful file functions.
	
    Copyright (C) 2003  Alberto Fern�ndez <infjaf00@yahoo.es>

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

using System.IO;
using System;
using ICSharpCode.SharpZipLib.Checksums;
using Dalle.Checksums;

using I = Dalle.I18N.GetText;

namespace Dalle.Utilidades
{	

	/// <summary>Esta clase proporciona m�todos est�ticos para copiar 
	/// datos de unos ficheros a otros.</summary>

	public class UtilidadesFicheros
	{
	
		/// <summary>Tama�o m�ximo del buffer de lectura/escritura.
		/// (Por defecto 1 Mb)</summary>
		
		private const long MAXIMOBUFFER = 0x100000; // 1 Mb
		
		/// <summary>Tama�o m�nimo del buffer de lectura/escritura.
		/// (Por defecto 512 bytes)</summary>
		
		private const long MINIMOBUFFER = 0x200;    // 512 bytes
		
		/// <summary>Tama�o del buffer (1 Mb).</summary>
		
		private static long _bufferSize = 0x100000; // 1 Mb
		
		/// <summary>Tama�o del buffer</summary>
		
		public static long Buffer {
			get{ return _bufferSize; }
			set{				
				if ((value <= MAXIMOBUFFER) && (value >= MINIMOBUFFER)){
					_bufferSize = value;
				}
				else if (value > MAXIMOBUFFER){
					_bufferSize = MAXIMOBUFFER;
				}
				else{
					_bufferSize = MINIMOBUFFER;
				}
			}
		}
		
		/// <summary>Constructor privado. No se permite crear instancias
		/// de esta clase.</summary>

		private UtilidadesFicheros()
		{
		}
	

		public static void ComprobarSobreescribir (string fich)
		{
			if (File.Exists (fich)){
				string error = String.Format (
					I._("File {0} already exists"),
					fich);
				//TODO:Poner una excepci�n personalizada.
				throw new Exception (error);
			}
		}

		
		/// <summary>
		/// Escribe unos datos en una posici�n del fichero, sobreescribiendo
		/// el contenido de ese fragmento y dejando intacto el 
		/// resto del fichero.
		/// </summary>
		/// <param name="fich">
		/// El nombre del fichero en el que se escribir�. </param>
		/// <param name="datos">El array que contiene los datos a escribir. Se 
		/// escribir� entero al fichero.</param>
		/// <param name="off">Posici�n del fichero a partir de la cual 
		/// se empezar� a escribir.</param>
		
		public static void Sobreescribir (String fich, byte[] datos, long off)
		{
			FileStream writer = File.OpenWrite(fich);
			writer.Seek (off, SeekOrigin.Begin);
			writer.Write(datos, 0, datos.Length);
			writer.Close();
		}
		
		/// <summary>Copia el contendio de un fichero al final de otro</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <returns>El n�mero de bytes copiados</returns>
		
		public static long CopiarTodo (String desde, String hacia)
		{
			return CopiarTodo (desde, hacia, new NullCRC());
		}
		
		/// <summary>Copia el contendio de un fichero al final de otro y realiza
		/// la suma de comprobaci�n de los datos transferidos.</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <param name="crc">El checsum a aplicar a los datos.</param>
		/// <returns>El n�mero de bytes copiados</returns>
		
		public static long CopiarTodo (String desde, String hacia, IChecksum crc)
		{			
			return CopiarIntervalo (desde, hacia, 0, new FileInfo(desde).Length, crc);
		}
		
		/// <summary>Copia el contendio de un fichero, a partir de una posici�n
		///	dada al final de otro y realiza
		/// la suma de comprobaci�n de los datos transferidos.</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <param name="pos">El n�mero de bytes que no se leer�n del 
		/// principio de origen</param>
		/// <param name="crc">El checsum a aplicar a los datos.</param>
		/// <returns>El n�mero de bytes copiados</returns>

		public static long CopiarIntervalo (String desde, String hacia, long pos, IChecksum crc)
		{
			return CopiarIntervalo (desde, hacia, pos, new FileInfo(desde).Length - pos, crc);
		}
		
		/// <summary>Copia el contendio de un fichero, a partir de una posici�n
		///	dada al final de otro.</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <param name="pos">El n�mero de bytes que no se leer�n del principio de origen</param>
		/// <returns>El n�mero de bytes copiados</returns>
		
		public static long CopiarIntervalo (String desde, String hacia, long pos)
		{
			return CopiarIntervalo(desde, hacia, pos, new FileInfo(desde).Length - pos, new NullCRC());
		}
		/// <summary>Copia una parte de un fichero final de otro.</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <param name="from">El n�mero de bytes que no se leer�n del principio de origen</param>
		/// <param count="count">El tama�o del fragmento a copiar.</param>
		/// <returns>El n�mero de bytes copiados</returns>
		
		public static long CopiarIntervalo (String desde, String hacia, long from, long count)
		{
			return CopiarIntervalo (desde, hacia, from, count, new NullCRC());
		}
		
		/// <summary>Copia una parte de un fichero final de otro y realiza
		/// el checksum de comprobaci�n de los datos transferidos.</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <param name="from">El n�mero de bytes que no se leer�n del principio de origen</param>
		/// <param count="count">El tama�o del fragmento a copiar.</param>
		/// <param name="crc">El checsum a aplicar a los datos.</param>
		/// <returns>El n�mero de bytes copiados</returns>

		public static long CopiarIntervalo (String desde, String hacia, long from, long count, IChecksum crc)
		{
			long totales = 0;
			long leidos = 0;
			FileStream writer = new FileStream (hacia, FileMode.Append, FileAccess.Write);
			writer.Seek (writer.Length, SeekOrigin.Begin);
			FileStream reader = new FileStream (desde, FileMode.Open);
			reader.Seek (from, SeekOrigin.Begin);
			
			
			byte[] buffer = new byte[_bufferSize];
			
			long limite = 0;
			do{
				limite = (buffer.Length > (count - totales)) ? (count - totales) : buffer.Length;
				leidos = reader.Read (buffer, 0, (int)limite);
				crc.Update (buffer, 0, (int) leidos);
				totales += leidos;
				writer.Write (buffer, 0, (int)leidos);
			}while ((leidos != 0) && ((totales + from) < reader.Length));
			
			writer.Close();
			reader.Close();
			
			return totales;			
		}
		
		
		/// <summary>Obtiene el contenido de un fichero a partir de una posici�n
		/// dada.</summary>
		/// <param name="fichero">El nombre del fichero del que se leer�.</param>
		/// <param name="seek">Posici�n a partir de la que se leer� (n�mero de bytes
		/// al principio del fichero que se saltar�n.</param>
		/// <returns>Los datos contenidos en fichero, desde la posici�n seek
		/// hasta el final.</returns>
		
		public static byte[] LeerSeek (String fichero, long seek)
		{
			return LeerSeek (fichero, seek, new FileInfo(fichero).Length - seek);
		}
		
		/// <summary>Obtiene el contenido de un fichero como un array be bytes.
		/// </summary>
		/// <param name="fichero">El nombre del fichero a leer.</param>
		/// <returns>Los datos contenidos en el fichero.</returns>
		
		public static byte[] LeerTodo (String fichero)
		{
			return LeerSeek (fichero, 0, new FileInfo(fichero).Length);
		}

		/// <summary>Obtiene los datos de un fragmento de un fichero.</summary>
		/// <param name="fichero">Nombre del fichero del que se leer�n los datos.</param>
		/// <param name="seek">Posici�n inicial.</param>
		/// <param name="count">Cantidad de datos que se leer�n.</param>
		
	
		public static byte[] LeerSeek (String fichero, long seek, long count)
		{
			FileStream reader = new FileStream (fichero, FileMode.Open);
			reader.Seek (seek, SeekOrigin.Begin);
			long tamano = (count < (reader.Length - seek)) ? count : (reader.Length - seek);
			
			byte[] ret = new byte[(int)tamano];
			reader.Read (ret, 0, (int) tamano);
			reader.Close();
			return ret;
		}
		
		/// <summary>A�ade datos al final de un fichero.</summary>
		/// <param name="fichero">El nombre del fichero al que se a�adir�n los datos.</param>
		/// <param name="data">Los datos a a�adir.</param>
		
		public static void Append (String fichero, byte[] data)
		{		
			FileStream writer = new FileStream (fichero, FileMode.Append, FileAccess.Write);
			writer.Seek (writer.Length, SeekOrigin.Begin);
			writer.Write(data, 0, data.Length);
			writer.Close();
		}
		
		/// <summary>Lee un byte del fichero.</summary>
		/// <param name="fichero">El nombre del fichero del que se leer� el byte.</param>
		/// <param name="pos">Posici�n del fichero que se leer�.</param>
		
		public static byte LeerByte (String fichero, long pos)
		{		
			FileStream reader = new FileStream (fichero, FileMode.Open);
			reader.Seek (pos, SeekOrigin.Begin);
			byte ret = (byte)reader.ReadByte();
			reader.Close();
			return ret;
		}
	}
}
