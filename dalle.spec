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
Requires:   	mono >= 0.24, gtk-sharp >= 0.10

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

%post

# Reconstruimos todos los wrappers para poder tenerlo relocalizable.

echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/dalle-console
echo "mono $RPM_INSTALL_PREFIX/bin/dalle-console.exe \"\$@\"" >> "$RPM_INSTALL_PREFIX"/bin/dalle-console
echo "" >> "$RPM_INSTALL_PREFIX"/bin/dalle-console

echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/dalle-gtk
echo "mono $RPM_INSTALL_PREFIX"/bin/dalle-gtk.exe >> "$RPM_INSTALL_PREFIX"/bin/dalle-gtk
echo "" >> "$RPM_INSTALL_PREFIX"/bin/dalle-gtk

echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/openhacha
echo "$RPM_INSTALL_PREFIX"/bin/openhacha-gtk >> "$RPM_INSTALL_PREFIX"/bin/openhacha
echo "" >> "$RPM_INSTALL_PREFIX"/bin/openhacha

echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/openhacha-gtk
echo "mono $RPM_INSTALL_PREFIX/bin/openhacha-gtk.exe" >> "$RPM_INSTALL_PREFIX"/bin/openhacha-gtk
echo "" >> "$RPM_INSTALL_PREFIX"/bin/openhacha-gtk

echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/openhacha-text
echo "mono $RPM_INSTALL_PREFIX/bin/openhacha-text.exe" >> "$RPM_INSTALL_PREFIX"/bin/openhacha-text
echo "" >> "$RPM_INSTALL_PREFIX"/bin/openhacha-text

echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/dalle-crcs
echo "mon $RPM_INSTALL_PREFIX/bin/dalle-crcs.exe " >> "$RPM_INSTALL_PREFIX"/bin/dalle-crcs
echo "" >> "$RPM_INSTALL_PREFIX"/bin/dalle-crcs



%files
%doc README COPYING ChangeLog NEWS
/usr/bin/dalle-console.exe
/usr/bin/dalle-gtk.exe
/usr/bin/openhacha-text.exe
/usr/bin/openhacha-gtk.exe
/usr/bin/dalle-crcs.exe
/usr/bin/dalle-crcs
/usr/bin/openhacha
/usr/bin/openhacha-text
/usr/bin/openhacha-gtk
/usr/bin/dalle-console
/usr/bin/dalle-gtk

/usr/lib/libDalle.dll

%changelog

* Mon Jan 26 2004 Alberto Fernandez <infjaf00@yahoo.es>
- Por fin es relocalizable real.

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
