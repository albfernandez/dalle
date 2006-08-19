/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Hacha.ParteHachaPro - 
		Split and Join files in HachaPro format.
	
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


using System;
using System.IO;
using System.Collections;

using Dalle.Formatos;
using Dalle.Formatos.Zip;
using Dalle.Utilidades;
using Dalle.Checksums;
using I = Dalle.I18N.GetText;

namespace Dalle.Formatos.Hacha
{
	public class ParteHachaPro : Parte
	{
		private Dalle.Formatos.Zip.Zip zip = new Dalle.Formatos.Zip.Zip();
				
		public ParteHachaPro ()
		{
			nombre = "hachapro";
			descripcion = "Hacha Pro 3";
			web = "http://www.hacha.org";
			compatible = true;
			parteFicheros = true;
			
			zip.Progress += new ProgressEventHandler (this.OnProgress);
		}
		protected override void _Unir (string fichero, string dirDest)
		{
		
			string nombre = fichero.Substring (0, fichero.Length - 4);
			string destino = new FileInfo (fichero).Name;
			destino = destino.Substring (0, destino.Length - 4);
			destino = dirDest + Path.DirectorySeparatorChar + destino;
			
			UtilidadesFicheros.ComprobarSobreescribir(destino);
			
			ArrayList lst = LeerCRCS (nombre + ".crc");
			CRC crc = new NullHachaCRC();
			int fr = 0;
			String frag = String.Format (nombre +".{0:000}", fr);

			long totales = 0;
			long transferidos = 0;
			
			if (lst != null){
				for (int i=0; i < lst.Count; i++){
					String f = String.Format(nombre + ".{0:000}", i);
					if (!File.Exists (f)){
						// TODO: Lanzar una excepci�n personalizada.
						throw new Exception (String.Format(
							I._("{0}:File not found"),f));
					}
					totales += new FileInfo(f).Length;
				}
			}
			else{
				int i=0;
				String f = String.Format(nombre + ".{0:000}", i);
				while (File.Exists (f)){
					totales += new FileInfo(f).Length;
					f = String.Format (nombre + ".{0:000}", ++i);
				}
			}
			OnProgress (0, totales);
			while (File.Exists (frag)){
				
				if (lst != null)
					crc = new HachaCRC(new FileInfo (frag).Length);
				
				transferidos += UtilidadesFicheros.CopiarTodo(frag, destino, crc);
				OnProgress (transferidos, totales);
				if (lst != null){
					int esperado = (int) lst[fr];
					int obtenido = (int) crc.Value;
					if (esperado != obtenido){
						throw new Exception (String.Format (
							I._("Checksum verification failed on {0}!"), frag));
					}
				}
								
				fr++;
				frag = String.Format (nombre +".{0:000}", fr);
			}

			if (destino.ToUpper().EndsWith("_AXE") && zip.EsZipPorContenido(destino)){				
				zip.Unir(destino);
				File.Delete(destino);
			}			
		}
		protected override void _Partir (string fichero,string sal1, string dir, long kb)
		{
			
			// No utilizamos el parametro sal1.
			// TODO: Comprimir a zip primero?
			ArrayList vector = new ArrayList();
			string destino = dir + Path.DirectorySeparatorChar + (new FileInfo(fichero).Name);
			
			
			long tamano = new FileInfo (fichero).Length;
			long tFragmento = 1024 * kb;
			long transferidos = 0;
			int fragmento = 0;
			OnProgress (transferidos, tamano);
			do{
				long tF = ( (tamano - transferidos) > tFragmento)? tFragmento:(tamano - transferidos); 
				CRC crc = new HachaCRC(tF);
				string nombre = String.Format (destino + ".{0:000}", fragmento);
				UtilidadesFicheros.ComprobarSobreescribir (nombre);
				transferidos += UtilidadesFicheros.CopiarIntervalo 
						(fichero, nombre, transferidos, tFragmento, crc);
				fragmento++;
				vector.Add (crc);
				OnProgress(transferidos, tamano);
			} while (transferidos < tamano);
			
			
			EscribirCRCS (destino + ".crc" , vector);
			
		}
		
		private ArrayList LeerCRCS (string fich)
		{
			ArrayList lst = new ArrayList();
			
			if (! File.Exists(fich))
				return null;
			if ((new FileInfo(fich).Length % 4) !=0)
				return null;
			
			FileStream reader = new FileStream (fich, FileMode.Open);
			byte[] buf = new byte[new FileInfo(fich).Length];
			reader.Read (buf, 0, buf.Length);
			reader.Close();
			
			for (int i=4; i < buf.Length; i+=4)
				lst.Add (UtArrays.LeerInt32 (buf, i));
			
			return lst;
		
		}
		
		private void EscribirCRCS (string fich, ArrayList lst)
		{

			byte[] buf = new byte [4 * (lst.Count + 1)];
			
			UtArrays.EscribirInt (buf, lst.Count - 1, 0);
			
			int pos = 4;
			foreach (CRC crc in lst){
				UtArrays.EscribirInt (buf, (int) crc.Value, pos);
				pos+=4;
			}
			UtilidadesFicheros.ComprobarSobreescribir (fich);
			FileStream writer = new FileStream (fich, FileMode.Create);
			writer.Write (buf, 0, buf.Length);
			writer.Close();
		}
		public override bool PuedeUnir (string fichero)
		{
			if (! File.Exists (fichero) )
				return false;
			//TODO: Depurar un poco.
			return fichero.EndsWith(".000");
		}
	}
}
