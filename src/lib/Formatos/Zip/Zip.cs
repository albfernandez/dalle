/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Zip.Zip - Basic support for compressed zip files.
	
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
using System.IO;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;	

using Dalle.Utilidades;

namespace Dalle.Formatos.Zip
{

	// TODO: Depurar.
	
	public class Zip : Parte 
	{
		private static int buffSize = 262144;

		
		public Zip()
		{
		
			nombre = "zip";
			descripcion = "Fichero comprimido con ZIP";
			web = "_";
			compatible = false;
			parteFicheros = true;
			
		}
		public static int BufferSize{
			get{return buffSize;}
			set{
				if (value > 0)
					buffSize = value;
			}
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			Descomprimir(fichero, dirDest);
		}
		protected override void _Partir (string fichero,string salida1, string dir, long kb)
		{
			if ((salida1 == null) || (salida1 == string.Empty))
				salida1 = new FileInfo(fichero).Name + ".zip";
				
			Comprimir (fichero, dir + Path.DirectorySeparatorChar + salida1);
		}
		public override bool PuedeUnir (string fichero)
		{
			// TODO: Mejorar la deteccion del formato zip.
			// TODO: Hacer que detecte el contenido en lugar de la extension.
			// TODO: Cuidado con los ficheros zip cortados (tarda mucho).
			if ( ! File.Exists (fichero) )
				return false;
			return fichero.ToUpper().EndsWith(".ZIP");
		}
		
		public bool EsZipPorContenido (string fichero)
		{
			try{
				ZipFile zf = new ZipFile(fichero);
				zf.Close();
			} catch (Exception){
				return false;
			}
			return true;
		}
		
		public long ObtenerTamanoDatos (string fichero)
		{
			long ret = 0;
			ZipInputStream s = new ZipInputStream (File.OpenRead (fichero));
			ZipEntry entrada;
			while ((entrada = s.GetNextEntry()) != null){
				ret += entrada.Size;
			}
			s.Close();
			return ret;
		}
		
		public void Descomprimir (string fichero, string dirDest)
		{	
			
			ZipInputStream s = new ZipInputStream (File.OpenRead(fichero));
			long total = s.Length;
			ZipEntry entrada;
			OnProgress (0,1);
			while ((entrada = s.GetNextEntry()) != null){
				String dirName = Path.GetDirectoryName (entrada.Name);
				String ficName = Path.GetFileName (entrada.Name);
				
				DirectoryInfo currDir = Directory.CreateDirectory (Path.Combine(dirDest, dirName));
				if (ficName != null && ficName.Length != 0){
					FileInfo fi = new FileInfo (Path.Combine (currDir.FullName, ficName));
					FileStream writer = fi.Create();
					byte[] data = new byte[BufferSize];
					while (true){
						int size = s.Read(data, 0, data.Length);
						if (size > 0){
							writer.Write(data, 0, size);
							OnProgress (s.Position, total);
						}
						else{
							break;
						}
					}
					writer.Close();
					fi.LastWriteTime = entrada.DateTime;
				}
			}
			s.Close();
		}		
		// Comprimir
		
		public void Comprimir (string fichero, string salida)
		{
			Comprimir (fichero, salida, 9);
		}
		
		// TODO: Hacer una funcion que soporte pasarle como paramentros
		// multiples archivos y directorios para comprimir.
		
		public void Comprimir (string fichero, string salida, int level)
		{
			// TODO: A ver si podemos hacer que funcione el metodo STORED.
			// sino dejar todo con DEFLATED. (¿Se puede con los directorios?)
			
			level = (level > 9) ? 9 : level;
			level = (level < 1) ? 1 : level;
			ZipOutputStream zOutstream = new ZipOutputStream (File.Create(salida));

			//String basePath = "." +  Path.DirectorySeparatorChar;
			OnProgress (0, 1);
			if (File.Exists (fichero)){
				FileStream fStream = File.OpenRead (fichero);
				long transferidos = 0;
                		byte[] buffer = new byte[BufferSize];				
				
				ZipEntry entry = new ZipEntry (fichero);
				entry.DateTime = File.GetLastWriteTime(fichero);
				entry.Size = fStream.Length;

				zOutstream.PutNextEntry(entry);
				do{
					long leidos = fStream.Read(buffer, 0, buffer.Length);
					zOutstream.Write(buffer, 0, (int) leidos);
					transferidos+=leidos;
					// TODO:Comprobar esta función
					OnProgress (transferidos, fStream.Length);
				}while (transferidos < fStream.Length);
				fStream.Close();
			}				
			else {
                throw new FileNotFoundException(fichero);
           	}	
			zOutstream.Finish();
            zOutstream.Close();			
		}	
	}
}
