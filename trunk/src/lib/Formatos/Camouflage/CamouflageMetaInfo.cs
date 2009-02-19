/*

    Dalle - A split/join file utility library	
    
	
    Copyright (C) 2004-2009
    Original author (C - code) - Dai SET <dai_set@yahoo.com>
    C# translation by - Alberto Fernández  <infjaf00@yahoo.es>

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
using System.Reflection;
using System.IO;

using Dalle.Utilidades;

namespace Dalle.Formatos.Camouflage
{
	public class CamouflageMetaInfo
	{
		private String password;
		private String version;
		private CamouflageEntry[] archivos;
		
		
		
		
		public string Password {
			get { 
				if (password == null)
					return String.Empty;
				return password;
			}
			set { password = value; }
		}
		
		public string Version {
			get {
				if (version == null){
					return "v1.2.1-dalle-" + Assembly.GetExecutingAssembly().GetName().Version;
				}
				return version;
			}
		}
		
		private CamouflageMetaInfo ()
		{

		}
		public CamouflageMetaInfo (int elements)
		{
			archivos = new CamouflageEntry [elements];
			for (int i=0; i < elements; i++){
				archivos[i] = new CamouflageEntry();
			}
		}
		
		public CamouflageEntry[] Archivos {
			get { return archivos;  }
		}
		
		public static CamouflageMetaInfo LoadFromFile(String fichero)
		{
		
			CamouflageMetaInfo info = new CamouflageMetaInfo();
			FileStream reader = null;
			try {
				reader = new FileStream (fichero, FileMode.Open);
				reader.Seek (-277, SeekOrigin.End);
				byte[] b1 = new byte[277];
				reader.Read (b1, 0, 277);
				int totalArchivos = UtArrays.LeerInt16(b1, 0);
				
				info.archivos = new CamouflageEntry[totalArchivos];			
				info.password = Aleatorizador.DesencriptarTexto (b1, 2, 255);
				info.version  = Aleatorizador.DesencriptarTexto (b1, 257, 20);
				
				
				int size2 = totalArchivos * 259;
				reader.Seek (-277 - size2, SeekOrigin.End);
				byte[] b2 = new byte [size2];
				reader.Read (b2, 0, size2);
				
			
			
			
				for (int i=1; i <= totalArchivos; i++){
					int tamano = UtArrays.LeerInt32(b2, b2.Length - (4*i));
					String nombre = Aleatorizador.DesencriptarTexto (b2, b2.Length - ((4*totalArchivos) + (255*i)), 255);
					info.archivos[i-1] = new CamouflageEntry();
					info.Archivos[i-1].Nombre = nombre;
					info.Archivos[i-1].Tamano = tamano;
				}
				
				return info;
			}
			catch (Exception){
				return null;
			}
			finally {
				if (reader != null){
					reader.Close();
				}
			}
		}
		public byte[] ToByteArray ()
		{
			int tamanoCola = 277 + 259 * archivos.Length;
			byte[] ret = new byte[tamanoCola];			
			
			//Guardar para cada archivo sus datos
			
			for (int i=0; i < archivos.Length; i++){

				UtArrays.EscribirInt (ret, (int) Archivos[i].Tamano, tamanoCola - 277 - (4*i) -4);
				
				byte[] nombre = Aleatorizador.EncriptarTexto(Archivos[i].Nombre, 255);
				
				for (int j = 0; j < nombre.Length; j++){
					ret[255 * (Archivos.Length - i - 1)+j] = nombre[j];
				}
			
			}
			
			
			
			// Guardar password encriptado en tamanoCola - 275
			byte[] pass = Aleatorizador.EncriptarTexto ("", 255);			
			
			for (int i=0; i < pass.Length; i++){
				ret[tamanoCola - 275 + i] = pass[i];
			}
			
			// Guardar version encriptada en tamanoCola - 20
			
			byte[] vers = Aleatorizador.EncriptarTexto (Version, 20);
			
			for (int i=0; i< vers.Length; i++){
				ret [tamanoCola -20 +i] = vers[i];
			}
			// Guardamos el numero total de archivos
			
			UtArrays.EscribirInt (ret, (short) archivos.Length, tamanoCola - 277);
			
			return ret;
		}
	
	
	}
}
