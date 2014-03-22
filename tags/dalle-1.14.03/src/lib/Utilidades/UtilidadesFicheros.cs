/*

    Dalle - A split/join file utility library	
    Dalle.Utilidades.UtilidadesFicheros - Useful file functions.
	
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

using System.IO;
using System;
using ICSharpCode.SharpZipLib.Checksums;
using Dalle.Checksums;

namespace Dalle.Utilidades
{	

	/// <summary>Esta clase proporciona métodos estáticos para copiar 
	/// datos de unos ficheros a otros.</summary>

	public class UtilidadesFicheros
	{
	
		/// <summary>Tamaño máximo del buffer de lectura/escritura.
		/// (Por defecto 1 Mb)</summary>
		
		private const long MAXIMOBUFFER = 0x100000; // 1 Mb
		
		/// <summary>Tamaño mínimo del buffer de lectura/escritura.
		/// (Por defecto 512 bytes)</summary>
		
		private const long MINIMOBUFFER = 0x200;    // 512 bytes
		
		/// <summary>Tamaño del buffer (1 Mb).</summary>
		
		private static long _bufferSize = 0x100000; // 1 Mb
		
		/// <summary>Tamaño del buffer</summary>
		
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
	
		public static void ComprobarSobreescribir (FileInfo fich)
		{
			if (!fich.Exists) 
			{
				throw new Dalle.Formatos.FileAlreadyExistsException(fich.FullName, fich.FullName);
			}
		}

		public static void ComprobarSobreescribir (string fich)
		{
			if (File.Exists (fich)){
				throw new Dalle.Formatos.FileAlreadyExistsException(fich, fich);
			}
		}

		
		/// <summary>
		/// Escribe unos datos en una posición del fichero, sobreescribiendo
		/// el contenido de ese fragmento y dejando intacto el 
		/// resto del fichero.
		/// </summary>
		/// <param name="fich">
		/// El nombre del fichero en el que se escribirá. </param>
		/// <param name="datos">El array que contiene los datos a escribir. Se 
		/// escribirá entero al fichero.</param>
		/// <param name="off">Posición del fichero a partir de la cual 
		/// se empezará a escribir.</param>
		
		public static void Sobreescribir (String fich, byte[] datos, long off)
		{
			FileStream writer = File.OpenWrite(fich);
			writer.Seek (off, SeekOrigin.Begin);
			writer.Write(datos, 0, datos.Length);
			writer.Close();
		}
		
		/// <summary>Copia el contenido de un fichero al final de otro</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <returns>El número de bytes copiados</returns>
		
		public static long CopiarTodo (String desde, String hacia)
		{
			return CopiarTodo (desde, hacia, new NullCRC());
		}
		
		/// <summary>Copia el contendio de un fichero al final de otro y realiza
		/// la suma de comprobación de los datos transferidos.</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <param name="crc">El checsum a aplicar a los datos.</param>
		/// <returns>El número de bytes copiados</returns>
		
		public static long CopiarTodo (String desde, String hacia, IChecksum crc)
		{			
			return CopiarIntervalo (desde, hacia, 0, new FileInfo(desde).Length, crc);
		}
		
		/// <summary>Copia el contendio de un fichero, a partir de una posición
		///	dada al final de otro y realiza
		/// la suma de comprobación de los datos transferidos.</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <param name="pos">El número de bytes que no se leerán del 
		/// principio de origen</param>
		/// <param name="crc">El checsum a aplicar a los datos.</param>
		/// <returns>El número de bytes copiados</returns>

		public static long CopiarIntervalo (String desde, String hacia, long pos, IChecksum crc)
		{
			return CopiarIntervalo (desde, hacia, pos, new FileInfo(desde).Length - pos, crc);
		}
		
		/// <summary>Copia el contendio de un fichero, a partir de una posición
		///	dada al final de otro.</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <param name="pos">El número de bytes que no se leerán del principio de origen</param>
		/// <returns>El número de bytes copiados</returns>
		
		public static long CopiarIntervalo (String desde, String hacia, long pos)
		{
			return CopiarIntervalo(desde, hacia, pos, new FileInfo(desde).Length - pos, new NullCRC());
		}
		/// <summary>Copia una parte de un fichero final de otro.</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <param name="from">El número de bytes que no se leerán del principio de origen</param>
		/// <param count="count">El tamaño del fragmento a copiar.</param>
		/// <returns>El número de bytes copiados</returns>
		
		public static long CopiarIntervalo (String desde, String hacia, long from, long count)
		{
			return CopiarIntervalo (desde, hacia, from, count, new NullCRC());
		}
		
		/// <summary>Copia una parte de un fichero final de otro y realiza
		/// el checksum de comprobación de los datos transferidos.</summary>
		/// <param name="desde">El fichero origen de datos.</param>
		/// <param name="hacia">El fichero destido de los datos.</param>
		/// <param name="from">El número de bytes que no se leerán del principio de origen</param>
		/// <param count="count">El tamaño del fragmento a copiar.</param>
		/// <param name="crc">El checsum a aplicar a los datos.</param>
		/// <returns>El número de bytes copiados</returns>

		public static long CopiarIntervalo (String desde, String hacia, long from, long count, IChecksum crc)
		{
			long totales = 0;
			long leidos = 0;
			FileStream writer = new FileStream (hacia, FileMode.Append, FileAccess.Write);
			writer.Seek (writer.Length, SeekOrigin.Begin);
			FileStream reader = new FileStream (desde, FileMode.Open, FileAccess.Read);
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
		
		
		/// <summary>Obtiene el contenido de un fichero a partir de una posición
		/// dada.</summary>
		/// <param name="fichero">El nombre del fichero del que se leerá.</param>
		/// <param name="seek">Posición a partir de la que se leerá (número de bytes
		/// al principio del fichero que se saltarán.</param>
		/// <returns>Los datos contenidos en fichero, desde la posición seek
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
		/// <param name="fichero">Nombre del fichero del que se leerán los datos.</param>
		/// <param name="seek">Posición inicial.</param>
		/// <param name="count">Cantidad de datos que se leerán.</param>
		
	
		public static byte[] LeerSeek (String fichero, long seek, long count)
		{
			FileStream reader = File.OpenRead (fichero);
			reader.Seek (seek, SeekOrigin.Begin);
			long tamano = (count < (reader.Length - seek)) ? count : (reader.Length - seek);
			
			byte[] ret = new byte[(int)tamano];
			reader.Read (ret, 0, (int) tamano);
			reader.Close();
			return ret;
		}
		
		
		
		
		
		public static void Append (String fichero, byte[] data, IChecksum crc)
		{
			if (crc != null){
				crc.Update (data);
			}
			FileStream writer = new FileStream (fichero, FileMode.Append, FileAccess.Write);
			writer.Seek (writer.Length, SeekOrigin.Begin);
			writer.Write(data, 0, data.Length);
			writer.Close();
		}
		
		/// <summary>Añade datos al final de un fichero.</summary>
		/// <param name="fichero">El nombre del fichero al que se añadirán los datos.</param>
		/// <param name="data">Los datos a añadir.</param>
		
		
		
		public static void Append (String fichero, byte[] data)
		{		
			Append (fichero, data, null);
		}
		
		/// <summary>Lee un byte del fichero.</summary>
		/// <param name="fichero">El nombre del fichero del que se leerá el byte.</param>
		/// <param name="pos">Posición del fichero que se leerá.</param>
		
		public static byte LeerByte (String fichero, long pos)
		{		
			FileStream reader = new FileStream (fichero, FileMode.Open, FileAccess.Read);
			reader.Seek (pos, SeekOrigin.Begin);
			byte ret = (byte)reader.ReadByte();
			reader.Close();
			return ret;
		}
		
		public static void CalcularCRC (string file, int offset, int count ,IChecksum crc)
		{
			FileStream reader = new FileStream (file, FileMode.Open);
			reader.Seek (offset, SeekOrigin.Begin);
			for (int i=0; i < count; i++){
				crc.Update (reader.ReadByte());
			}
			reader.Close();
		}
		
		/// <summary>Obtiene el stream para escribir en un fichero.
		/// </summar>
		/// <remarks>Crea la estructura de directorios necesaria.</remarks>
		/// <param name="file">El nombre del fichero a crear.</param>
		/// <returns>Un stream en el que escribir.</returns>
		
		public static Stream CreateWriter (string file)
		{
			file = ChangeDirectorySeparatorChar (file);
			UtilidadesFicheros.ComprobarSobreescribir (file);

			int last = file.LastIndexOf (Path.DirectorySeparatorChar);
			if (last < 0)
			{
				return File.Open (file, FileMode.CreateNew);
			}
			
			
			string dir = file.Substring (0, file.LastIndexOf(Path.DirectorySeparatorChar));
			DirectoryInfo directorio = new DirectoryInfo(dir);
			directorio.Create();			
			return File.Open(file, FileMode.CreateNew);
		}
		public static Stream CreateWriter (FileInfo fi)
		{
			return CreateWriter (fi.FullName);
		}
		
		public static long GenerateHash (string file, IChecksum crc)
		{
			FileStream reader = new FileStream (file, FileMode.Open);
			byte[] buffer = new byte[_bufferSize];
			long total = 0;
			long count = 0;
			
			do {
				count = reader.Read (buffer, 0, buffer.Length);
				crc.Update (buffer, 0, (int)count);
				total += count;
			} while (count > 0);
			
			
			reader.Close ();
			return total;
		}
		public static string ChangeDirectorySeparatorChar (string orig)
		{
			if (Path.DirectorySeparatorChar == '/')
			{
				return orig.Replace('\\', '/');
			}
			else if (Path.DirectorySeparatorChar == '\\') {
				return orig.Replace('/', '\\');
			}
			return orig;
		}
	}
}
