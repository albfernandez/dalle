%define name dalle
%define version 0.5.5
%define release 1
%define prefix /usr

Summary: 		Corta y une archivos en varios formatos.
Name: 			%{name}
Version: 		%{version}
Release: 		%{release}
Copyright: 		GPL
Group: 			Utilities/File
Url: 			http://dalle.sourceforge.net
Vendor:     	Alberto Fernandez  <infjaf00@yahoo.es>
Source: 		%{name}-%{version}.tar.bz2
BuildRoot:  	/var/tmp/%{name}-%{version}
BuildArch:		noarch
Prefix:			%{prefix}
Distribution:   Any
Packager:       Alberto Fernandez  <infjaf00@yahoo.es>

%description

%prep
%setup

%build
./configure --prefix=%{prefix} --build-release
make

%install
make install DESTDIR=%{buildroot}

%clean
rm -rf %{buildroot}

%files
%doc README COPYING ChangeLog NEWS
/usr/bin/openhacha.exe
/usr/bin/dalle-console.exe
/usr/bin/dalle-gtk.exe
/usr/bin/openhacha-text.exe
/usr/bin/dalle-crcs.exe
/usr/bin/dalle-crcs
/usr/bin/openhacha
/usr/bin/openhacha-text
/usr/bin/openhacha-gtk
/usr/bin/dalle-console
/usr/bin/dalle-gtk

/usr/lib/libDalle.dll

%changelog


* Mon Dec 29 2003 Alberto Fernandez <infjaf00@yahoo.es>
- Incluido openhacha-text.
- Construye sin opciones de depuracion.

* Mon Dec 22 2003 Alberto Fernandez <infjaf00@yahoo.es>
- Paquete adaptado al nuevo sistema de construccion.

* Thu Dec 18 2003 Alberto Fernandez <infjaf00@yahoo.es>
- Interfaz grafica actualizada a OpenHacha 0.2.

* Mon Dec 15 2003 Alberto Fernandez <infjaf00@yahoo.es>
- Interfaz grafica basada en OpenHacha 0.1.

* Fri Dec 12 2003 Alberto Fernandez <infjaf00@yahoo.es>
- El paquete RPM es relocalizable.

* Thu Dec 04 2003 Alberto Fernandez <infjaf00@yahoo.es>
- Primer paquete RPM
