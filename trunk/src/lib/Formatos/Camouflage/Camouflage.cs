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
using System.IO;

using Dalle.Utilidades;
using Dalle.Streams;

namespace Dalle.Formatos.Camouflage
{
	public class Camouflage : Parte
	{
		public Camouflage ()
		{
			nombre = "camouflage";
			descripcion = "Camouflage";
			web = "";
			compatible = false;
			parteFicheros = true;
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			OnProgress (0, 1);
			long total = new FileInfo (fichero).Length;
			CamouflageMetaInfo info = CamouflageMetaInfo.LoadFromFile (fichero);
			
			if (info == null) {
				throw new Exception ();
			}
			
			
			long pos = 0;
			long largoPiel = info.Archivos[0].Tamano;
			string destino = dirDest + Path.DirectorySeparatorChar + fichero;
			
			
			int leidos = 0;
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			FileStream inStream = File.OpenRead (fichero);
			
			if (true) {
				// Este código extrae la piel
				destino = dirDest + Path.DirectorySeparatorChar + info.Archivos[0].Nombre;
				Stream os = UtilidadesFicheros.CreateWriter (destino);
				Stream sls = new SizeLimiterStream (inStream, largoPiel);
				while ((leidos = sls.Read (buffer, 0, buffer.Length)) > 0) 
				{
					os.Write (buffer, 0, leidos);
					pos += leidos;
					OnProgress (pos, total);
				}
				os.Close ();
				
				// Permisos y tiempos de acceso
				if (26 != inStream.Read (buffer, 0, 26)) 
				{
					throw new IOException ("Unexpected end of file");
				}
				pos += 26;
				
			}
			byte[] lc = new byte[4];
			byte[] chunk = new byte[1024 * 1024];
			Aleatorizador aleat = new Aleatorizador ();
			for (int i = 1; i < info.Archivos.Length; i++) {
				destino = dirDest + Path.DirectorySeparatorChar + info.Archivos[i].Nombre;
				Stream os = UtilidadesFicheros.CreateWriter (destino);
				int largoChunk = 0;
				do {
					if (4 != inStream.Read (lc, 0, 4)) {
						throw new IOException ("Unexpected end of file");
					}
					largoChunk = UtArrays.LeerInt32 (lc, 0);
					pos += 4;
					if (largoChunk > 0) {
						aleat.Reset ();
						leidos = inStream.Read (chunk, 0, largoChunk);
						if (leidos != largoChunk) {
							throw new IOException ("Unexpected end of file");
						}
						pos += largoChunk;
						aleat.Desencriptar (chunk, 0, leidos);
						os.Write (chunk, 0, leidos);						
					}
					OnProgress (pos, total);
				} while (largoChunk > 0);
				os.Close ();
				
				// Permisos y tiempos de acceso
				if (26 != inStream.Read (buffer, 0, 26)) {
					throw new IOException ("Unexpected end of file");
				}
				pos += 26;
			}
			inStream.Close ();
			OnProgress (total, total);
			
			
			

//byte[] perm = UtilidadesFicheros.LeerSeek (fichero, pos, 26);
				//info.Archivos[0].Permisos = UtArrays.LeerInt16 (perm, 0);
				//info.Archivos[0].Creado = UtArrays.LeerDateTime (perm, 2);
				//info.Archivos[0].Accedido = UtArrays.LeerDateTime (perm, 10);
				//info.Archivos[0].Modificado = UtArrays.LeerDateTime (perm, 18);
				
				//FileInfo fi = new FileInfo (destino);				
				/* 
				Debería funcionar pero el programa falla
				
				fi.CreationTime = info.Archivos[0].Creado;	
				fi.LastAccessTime = info.Archivos[0].Accedido;
				fi.LastWriteTime = info.Archivos[0].Modificado;
				*/		
				//fi.Attributes = (FileAttributes) info.Archivos[0].Permisos;
		}
		protected override void _Partir (string fichero,string salida1, string dir, long kb)
		{
		
			// Para cada archivo
			
				// Escribir sus chunks de datos
				// Escribir sus permisos y sus fechas
				
			// Escribir la cola  de información
		
			byte[] piel = UtilidadesRecursos.GetImagen("Piel-01.jpg");
			if ((salida1 == null) || (salida1 == string.Empty)){
				salida1 = new FileInfo(fichero).Name;
			}
			salida1 = dir + Path.DirectorySeparatorChar + salida1;
			UtilidadesFicheros.ComprobarSobreescribir (salida1);
			int tamanoOriginal = (int) new FileInfo(fichero).Length;
			long transferidos = 0;
			OnProgress (0,1);
			
			CamouflageMetaInfo info = new CamouflageMetaInfo(2);
			
			DateTime fecha = DateTime.Now;
			
			
			
			info.Archivos[0].Nombre = "Piel-01.jpg";
			info.Archivos[0].Tamano = piel.Length;
			info.Archivos[0].Permisos = (int) FileAttributes.Normal;
			info.Archivos[0].Creado = fecha;
			info.Archivos[0].Accedido = fecha;
			info.Archivos[0].Modificado = fecha;
			
			info.Archivos[1].Nombre = new FileInfo(fichero).Name;
			info.Archivos[1].Tamano = tamanoOriginal;
			info.Archivos[1].Permisos = (int) FileAttributes.Normal;
			info.Archivos[1].Creado = fecha;
			info.Archivos[1].Accedido = fecha;
			info.Archivos[1].Modificado = fecha;
			
		
			// Escribir la piel
			UtilidadesFicheros.Append (salida1, piel);
			
			// Escribir permisos y fechas de la piel
			
			
			byte[] perm = new byte[26];
			
			UtArrays.EscribirInt (perm, (short)info.Archivos[0].Permisos, 0);
			UtArrays.EscribirDateTime (perm, info.Archivos[0].Creado,  2);
			UtArrays.EscribirDateTime (perm, info.Archivos[0].Accedido, 10);
			UtArrays.EscribirDateTime (perm, info.Archivos[0].Modificado, 18);
			
			UtilidadesFicheros.Append(salida1, perm);
			
			Aleatorizador aleat = new Aleatorizador ();
			
			while (transferidos < tamanoOriginal){
			
				int chunkSize = 0;
				
				if ((tamanoOriginal - transferidos) > 1000000){
					chunkSize = 1000000;
				}
				else {
					chunkSize = (int) (tamanoOriginal - transferidos);
				}
								
				byte[] chunkSizeArray = new byte[4];
				UtArrays.EscribirInt (chunkSizeArray, chunkSize, 0);
				
				UtilidadesFicheros.Append (salida1, chunkSizeArray);
				
				byte[] chunk = UtilidadesFicheros.LeerSeek (fichero, transferidos, chunkSize);
				
				aleat.Reset();
				aleat.Desencriptar(chunk);
				UtilidadesFicheros.Append (salida1, chunk);
				transferidos += chunkSize;
				OnProgress (transferidos, tamanoOriginal);
			
			}
			
			byte[] magic = new byte[] { 0xff, 0xff, 0xff, 0xff };
			UtilidadesFicheros.Append(salida1, magic);
			perm = new byte[26];
			
			UtArrays.EscribirInt (perm, (short) info.Archivos[1].Permisos, 0);
			UtArrays.EscribirDateTime (perm, info.Archivos[1].Creado,  2);
			UtArrays.EscribirDateTime (perm, info.Archivos[1].Accedido, 10);
			UtArrays.EscribirDateTime (perm, info.Archivos[1].Modificado, 18);
			
			UtilidadesFicheros.Append (salida1, perm);
			
			UtilidadesFicheros.Append (salida1, info.ToByteArray());
			


			
		}

		public override bool PuedeUnir (string fichero)
		{
			FileStream reader = null;
			byte[] buf = new byte[20];
			if (!File.Exists (fichero))
			{
				return false;
			}

			try {
				
				//reader = new FileStream (fichero, FileMode.Open);
				reader = File.OpenRead (fichero);
				if (reader.Length < 300) {
					return false;
				}
				reader.Seek (-buf.Length, SeekOrigin.End);
				reader.Read (buf, 0, buf.Length);
			}
			catch (Exception e) {
				Console.WriteLine (e.Message);
				return false;
			}
			finally {
				if (reader != null) {
					reader.Close ();
			
				}
			}
			if (reader != null) {
				string textoDesencriptado = Aleatorizador.DesencriptarTexto(buf);
				return textoDesencriptado.StartsWith("v1.");
			}
			else{
				return false;
			}
			
		}
	
	}
}
