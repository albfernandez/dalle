/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Kamaleon.ParteKamaleon - 
		Split and Join files in KamaleoN 1 format
	
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

using System;
using System.IO;

using Dalle.Formatos;
using Dalle.Utilidades;
using Dalle.Checksums;

namespace Dalle.Formatos.Kamaleon
{
	public class ParteKamaleon : Parte
	{	
		protected CRC crc;
		public ParteKamaleon ()
		{
			nombre = "kamaleon1";
			descripcion = "KamaleoN v 1.0";
			web = "http://www.kamaleonsoft.com";
			parteFicheros = false;
			compatible = false;
			crc = new NullCRC();
		}
		
		protected override void _Unir (string fichero, string dirDest)
		{
			
			string baseDir = new FileInfo (fichero).DirectoryName;
			MetaInfoKamaleon metaInfo = new MetaInfoKamaleon (fichero);
			long transferidos = 0;
			if (metaInfo.PrimerInfo == null)
				throw new NoMetaInfoException();
				
				
				
			// Verificamos que todo está en su sitio.
			foreach (InfoFicheroKamaleon i in metaInfo.infos){
				OnProgress (0,1);
				string fich = baseDir + Path.DirectorySeparatorChar + i.NombreFragmento;
				
				
				if (! File.Exists (fich) ){
					throw new System.IO.FileNotFoundException("", fich);
				}
								
				byte primer = UtilidadesFicheros.LeerByte (fich, 0);
				byte ultimo = UtilidadesFicheros.LeerByte (fich, i.TamanoFragmento - 1);
				if ((primer != i.PrimerByte) || (ultimo != i.UltimoByte)){
					throw new Dalle.Formatos.FileFormatException();
				}				
			}			
			string f = metaInfo.PrimerInfo.NombreOriginal;
			UtilidadesFicheros.ComprobarSobreescribir (f);
			
			
			OnProgress (0, 1);
			foreach (InfoFicheroKamaleon i in metaInfo.infos){
				//Copiar la zona de datos del fragmento al archivo destino
				string fich = baseDir + Path.DirectorySeparatorChar + i.NombreFragmento;
				
				crc.Reset();				
				UtilidadesFicheros.CalcularCRC (fich, 0, i.TamanoPiel, crc);
				
				transferidos += UtilidadesFicheros.CopiarIntervalo
					(fich, f, i.TamanoPiel, i.TamanoDatos, crc);
				if (crc.Value != i.Checksum){
					throw new Dalle.Formatos.ChecksumVerificationException();
				}					
				OnProgress (transferidos, i.TamanoOriginal); 
			}

		}
		protected override void _Partir (string fichero,string salida1, string dir, long kb)
		{

			_Partir (fichero, salida1,dir, kb, "1");
		}
		
		protected void _Partir (string fichero, string s1, string dir,long kb, string version)
		{
			byte[] piel = UtilidadesRecursos.GetImagen("Piel-01.jpg");
			long tamano = 1024 * kb - piel.Length;
			if ((s1 == null) || (s1 == string.Empty)){
				s1 = new FileInfo(fichero).Name;
			}
			string salida1 = dir + Path.DirectorySeparatorChar + s1;
			int secuencia = 1;
			int tamanoOriginal = (int) new FileInfo(fichero).Length;
			long transferidos = 0;
			OnProgress (0,1);
			MetaInfoKamaleon mi = new MetaInfoKamaleon();
			do{
				
				byte[] b = UtilidadesFicheros.LeerSeek (fichero, transferidos, tamano);
				transferidos += b.Length;
				
				InfoFicheroKamaleon inf = InfoFicheroKamaleon.NewFromVersion (version);
				inf.NombreOriginal = fichero;
				inf.TamanoOriginal = tamanoOriginal;
				inf.Password = "";
				inf.NombreFragmento = string.Format (salida1 + "{0:000}.jpg", secuencia);
				inf.TamanoFragmento = piel.Length + b.Length;
				inf.PrimerByte = piel[0];
				inf.UltimoByte = b[b.Length - 1];
				inf.TamanoPiel = piel.Length;
				
				crc.Reset();
				
				UtilidadesFicheros.Append (inf.NombreFragmento, piel, crc);
				UtilidadesFicheros.Append (inf.NombreFragmento, b, crc);
				
				inf.Checksum = crc.Value;
				mi.Add (inf);
				
				// TODO: Comprobar que no se sobreescribe ningun fichero.
				
								
				secuencia++;
				
				OnProgress (transferidos, tamanoOriginal);
			}while (transferidos < tamanoOriginal);
			
			long t_ultimo = new FileInfo (mi.UltimoInfo.NombreFragmento).Length;
			if ((t_ultimo + mi.Size) > (kb * 1024) ){
				InfoFicheroKamaleon inf = InfoFicheroKamaleon.NewFromVersion (version);
				inf.NombreOriginal = fichero;
				inf.TamanoOriginal = tamanoOriginal;
				inf.Password = "";
				inf.NombreFragmento = string.Format (salida1 + "{0:000}.jpg", secuencia);
				inf.TamanoFragmento = piel.Length;
				inf.PrimerByte = piel[0];
				inf.UltimoByte = piel[piel.Length - 1];
				inf.TamanoPiel = piel.Length;				
				mi.Add (inf);				
				// TODO: Comprobar que no se sobreescribe ningun fichero.
				UtilidadesFicheros.ComprobarSobreescribir(inf.NombreFragmento);
				UtilidadesFicheros.Append (inf.NombreFragmento, piel);
				UtilidadesFicheros.Append (inf.NombreFragmento, mi.ToByteArray());
			}
			else{
				UtilidadesFicheros.Append (mi.GetNombreUltimoFragmento(), mi.ToByteArray());
			}
		}
		
		protected string VersionKamaleon (string fichero)
		{
			FileStream reader = null;
			try{			
				reader = new FileStream (fichero, FileMode.Open);
				reader.Seek(-0x13, SeekOrigin.End);
				byte[] buffer = new byte[0x13];
				reader.Read (buffer, 0, buffer.Length);
				
				
				String s = "";
				for (int i=0; i < buffer.Length; i++)
					s += Convert.ToChar(buffer[i]);
				String lista = s.Substring(10);
				String tam = s.Substring(0,10);
				
				long tamanoFichero = new FileInfo(fichero).Length;
				long tamanoDatos = Convert.ToInt64 (tam);
				if (tamanoFichero < tamanoDatos)
					return "";
				if ( (tamanoFichero - (tamanoDatos + 0x13)) % 560 != 0)
					return "";
				if (lista == "<-LISTA->")
					return "1";
				else if (lista == "<-LIST2->")
					return "2";
				else 
					return "";
			}
			catch (Exception)
			{
				return "";
			}
			finally{
				if (reader != null){
					reader.Close();
				}
			}	
			
		}
		public override bool PuedeUnir (string fichero)
		{
			if (! File.Exists (fichero) )
				return false;			
			return (VersionKamaleon(fichero) == "1");
		}
	}
}
