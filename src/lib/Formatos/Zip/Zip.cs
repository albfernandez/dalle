/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Zip.Zip - Basic support for compressed zip files.
	
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
/*

7z a -v1m test.zip prueba.iso 

test.zip.001
test.zip.002
test.zip.003
test.zip.004
test.zip.005
test.zip.006
test.zip.007
test.zip.008
test.zip.009
test.zip.010
test.zip.011


zip -s 1m test2.zip prueba.iso

test2.z01
test2.z02
test2.z03
test2.z04
test2.z05
test2.z06
test2.z07
test2.z08
test2.z09
test2.z10
test2.zip

*/
using System;
using System.IO;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;	

using Dalle.Utilidades;
using Dalle.Streams;

namespace Dalle.Formatos.Zip
{

	// TODO: Depurar.
	
	public class Zip : Parte 
	{

		
		public Zip()
		{
		
			nombre = "zip";
			descripcion = "Fichero comprimido con ZIP";
			web = "_";
			compatible = false;
			parteFicheros = false;
			
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
			return fichero.ToUpper().EndsWith(".ZIP") || fichero.ToUpper().EndsWith(".ZIP.001");
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
			ZipInputStream s = new ZipInputStream (new JoinStream(GetJoinInfo(fichero)));
			ZipEntry entrada;
			while ((entrada = s.GetNextEntry ()) != null)
			{
				ret += entrada.Size;
			}
			s.Close();
			return ret;
		}

		private IJoinInfo GetJoinInfo (string fichero) 
		{
			IJoinInfo joinInfo = null;
			if (fichero.ToUpper().EndsWith (".ZIP")) {
				joinInfo = new ZipJoinInfo (fichero);
			} else if (fichero.ToUpper().EndsWith (".ZIP.001")) {
				joinInfo = JoinInfo.GetFromFile (fichero);
			}
			return joinInfo;
		}
		public void Descomprimir (string fichero, string dirDest)
		{
			long total = ObtenerTamanoDatos (fichero);
			long transferidos = 0;
			IJoinInfo joinInfo = GetJoinInfo(fichero);


			ZipInputStream s = new ZipInputStream (new JoinStream(joinInfo));
			ZipEntry entrada;
			OnProgress (0, 1);
			while ((entrada = s.GetNextEntry ()) != null)
			{
				String dirName = Path.GetDirectoryName (entrada.Name);
				String ficName = Path.GetFileName (entrada.Name);				
				if (!entrada.IsFile)
				{
					continue;
				}
				
				DirectoryInfo currDir = Directory.CreateDirectory (Path.Combine (dirDest, dirName));
				if (ficName != null && ficName.Length != 0)
				{
					FileInfo fi = new FileInfo (Path.Combine (currDir.FullName, ficName));
					Stream writer = UtilidadesFicheros.CreateWriter (fi);
					byte[] data = new byte[Consts.BUFFER_LENGTH];
					int leidos = 0;
					if (entrada.Size > 0) {
						while ((leidos = s.Read (data, 0, data.Length)) > 0) {
							writer.Write (data, 0, leidos);
							transferidos += leidos;
							OnProgress (transferidos, total);
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
                byte[] buffer = new byte[Consts.BUFFER_LENGTH];				
				
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
