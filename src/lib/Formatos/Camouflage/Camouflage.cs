/*

    Dalle - A split/join file utility library	
    
	
    Copyright (C) 2004
    Original author (C - code) - Dai SET <dai_set@yahoo.com>
    C# translation by - Alberto Fernández  <infjaf00@yahoo.es>

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
using System.IO;

using Dalle.Utilidades;

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
			CamouflageMetaInfo info = CamouflageMetaInfo.LoadFromFile (fichero);
			
			if (info == null){
				throw new Exception();
			}
			
			
			long pos = 0;
			long largoPiel = info.Archivos[0].Tamano;
			string destino = dirDest + Path.DirectorySeparatorChar + fichero;
			
			
			
			if (true){
				// Este código extrae la piel
				destino = dirDest + Path.DirectorySeparatorChar + info.Archivos[0].Nombre;
				UtilidadesFicheros.ComprobarSobreescribir (destino);
				UtilidadesFicheros.CopiarIntervalo (fichero, destino, 0, largoPiel);
				pos = largoPiel;
				byte[] perm = UtilidadesFicheros.LeerSeek (fichero, pos, 26);
				info.Archivos[0].Permisos = UtArrays.LeerInt16(perm, 0);
				info.Archivos[0].Creado = UtArrays.LeerDateTime (perm, 2);
				info.Archivos[0].Accedido = UtArrays.LeerDateTime (perm, 10);
				info.Archivos[0].Modificado = UtArrays.LeerDateTime (perm, 18);
				
				FileInfo fi = new FileInfo (destino);				
				/* 
				Debería funcionar pero el programa falla
				
				fi.CreationTime = info.Archivos[0].Creado;	
				fi.LastAccessTime = info.Archivos[0].Accedido;
				fi.LastWriteTime = info.Archivos[0].Modificado;
				*/
				
				fi.Attributes = (FileAttributes) info.Archivos[0].Permisos;
	
			}
			pos = largoPiel + 26;

			
			
			
			
			Aleatorizador aleat = new Aleatorizador();
			for (int i=1; i < info.Archivos.Length; i++){
				destino = dirDest + Path.DirectorySeparatorChar + info.Archivos[i].Nombre; 
				UtilidadesFicheros.ComprobarSobreescribir (destino);
				int largoChunk = 0;
				do {
					
					byte[] lc = UtilidadesFicheros.LeerSeek(fichero, pos, 4);
					pos+=4;
					largoChunk = UtArrays.LeerInt32(lc, 0);
					if (largoChunk > 0){
						aleat.Reset();
						byte[] chunk = UtilidadesFicheros.LeerSeek (fichero, pos, largoChunk);
						pos+=largoChunk;
						aleat.Desencriptar(chunk);
						UtilidadesFicheros.Append(destino, chunk);
					}
				} while (largoChunk > 0);	
				
				byte[] perm = UtilidadesFicheros.LeerSeek (fichero, pos, 26);
				
				info.Archivos[i].Permisos = UtArrays.LeerInt16(perm, 0);
				info.Archivos[i].Creado = UtArrays.LeerDateTime (perm, 2);
				info.Archivos[i].Accedido = UtArrays.LeerDateTime (perm, 10);
				info.Archivos[i].Modificado = UtArrays.LeerDateTime (perm, 18);	

				FileInfo fi = new FileInfo (destino);
				
				/*
				fi.CreationTime = info.Archivos[i].Creado;
				fi.LastAccessTime = info.Archivos[i].Accedido;
				fi.LastWriteTime = info.Archivos[i].Modificado;
				*/
				fi.Attributes = (FileAttributes) info.Archivos[i].Permisos;

			}
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
			if ( ! File.Exists (fichero) )
				return false;

			try{
				
				reader = new FileStream(fichero, FileMode.Open);
				if (reader.Length < 300){
					return false;
				}
				reader.Seek (-buf.Length, SeekOrigin.End);
				reader.Read (buf, 0, buf.Length);
			}
			catch (Exception e){
				Console.WriteLine(e.Message);
				return false;
			}
			finally{
				if (reader != null){
					reader.Close();
			
				}
			}
			if (reader != null){
				return Aleatorizador.DesencriptarTexto(buf).StartsWith("v1.");
			}
			else{
				return false;
			}
			
		}
	
	}
}
