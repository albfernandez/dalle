# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU Library General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.


BASEPATH=../../../..
include $(BASEPATH)/rules.make
PROGNAME=openhacha-text.exe
WRAPPERNAME=openhacha-text
TARGET=$(BUILDDIR)/$(PROGNAME)
WRAPPER=$(BUILDDIR)/$(WRAPPERNAME)


all: $(TARGET)

$(TARGET): $(LIBRARY) src/*.cs
	@echo $(LINEA)
	@echo "Compilando openhacha-text."
	@echo $(LINEA)
	$(MCS) $(MCS_FLAGS) $(RECURSOS_TRADUCCIONES) -out:$(TARGET) -r:$(LIBRARY) "-recurse:src/*.cs"
	@echo "#!/bin/sh" > $(WRAPPER)
	@echo "" >> $(WRAPPER)
	
	@echo -n "$(MONO) $(MONO_FLAGS) $(PREFIX)/lib/dalle/$(PROGNAME) " >>$(WRAPPER)
	@echo -n "\"$$" >> $(WRAPPER)
	@echo -n "@\"" >> $(WRAPPER)
	@echo "" >> $(WRAPPER)


install: $(TARGET)
	$(MKDIR) $(DESTDIR)/$(PREFIX)/lib/dalle
	$(INSTALL) $(TARGET) $(DESTDIR)/$(PREFIX)/lib/dalle
	$(INSTALL) $(WRAPPER) $(DESTDIR)/$(PREFIX)/bin

uninstall:
	$(DELETE) $(DESTDIR)/$(PREFIX)/bin/$(WRAPPERNAME)
	$(DELETE) $(DESTDIR)/$(PREFIX)/lib/dalle/$(PROGNAME)

clean:
	rm -f $(TARGET) $(WRAPPER)

.PHONY: clean uninstall
