/*

	Dalle - A split/join file utility library
	Dalle.Formatos.IParte - Common interface for all formats.
	
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


namespace Dalle.Formatos
{

	/// <summary>Interface IParte:
	/// Define los m�todos que deben implementar todas las clases que parten
	/// y unen ficheros.</summary>
	
	public interface IParte
	{
		
		/// <summary>Nombre del formato. Debe ser �nico. Ej: "hacha1"</summary>
		
		string Nombre{
			get;
		}
		
		/// <summary>Breve descripci�n sobre el formato. Ej. "Hacha Pro v 3"
		/// </summary>
		
		string Descripcion{
			get;
		}
		
		/// <summary>Direcci�n web donde se puede encontrar el 
		/// programa original.</summary>
		
		string Web{
			get;
		}
		
		/// <summary>Indica si puede partir ficheros en este formato.</summary>
		
		bool ParteFicheros{
			get;
		}
		
		/// <summary>Indica si los ficheros partidos en este formato se 
		/// pueden unir con cat en *nix o con copy en DOS.</summary>
		
		bool Compatible{
			get;
		}


		/// <summary>Corta el fichero en fragmentos.</summary>
		/// <param name="fichero">El fichero original a cortar.</param>
		/// <param name="sal1">Nombre del primer fragmento, o base de ese nombre. 
		/// No todos los m�dulos hacen uso de este par�metro.</param>
		/// <param name="dir">Directorio en el que se guardar�n los fragmentos
		/// </param>
		/// <param name="kb">Tamaño máximo de los fragmentos, en kb.</param>
		
		void Partir (string fichero, string sal1, string dir, long kb);
		
		/// <summary>Corta el fichero en fragmentos.
		/// Los fragmentos se crean en el directorio que est� el original.
		/// </summary>
		/// <param name="fichero">El fichero original a cortar.</param>
		/// <param name="sal1">Nombre del primer fragmento, o base de ese nombre. 
		/// No todos los m�dulos hacen uso de este par�metro.</param>
		/// <param name="kb">Tama�o m�ximo de los fragmentos, en kb.</param>
		
		void Partir (string fichero, string salida1, long kb);
		
		/// <summary>Une el fichero y deja el resultado en un directorio 
		/// determinado.</summary>
		/// <param name="fichero">El fichero a unir.</param>
		/// <param name="dirDest">Directorio donde se guardar� el archivo
		/// resultante.</param>
		
		void Unir (string fichero, string dirDest);
		
		/// <summary>Une el fichero. El archivo obtenido se guarda en el 
		/// directorio del original.</summary>
		/// <param name="fichero">El fichero a unir.</param>
		
		void Unir (string fichero);
		
		/// <summary>Indica si puede unir el archivo que se le pasa como
		/// parámetro.</summary>
		/// <param name="fichero">El fichero que queremos unir</param>
		/// <returns>true si el m�dulo puede unirlo, false en otro caso.
		/// </returns>
		
		bool PuedeUnir (string fichero);
		
	}
}
