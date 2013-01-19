
MONO = mono
MCS = mcs
MAKEWEB = RenderFormatPages.exe
TEMPLATE = src/template/template.html.in
TEMPLATE_INDEX = src/template/index.html.in
SOURCEDIR = src
DESTDIR = html

all: $(MAKEWEB) $(SOURCEDIR)/formats.xml $(TEMPLATE) $(TEMPLATE_INDEX) $(DESTDIR)/styles.css
	$(MONO) $(MAKEWEB) src/formats.xml $(TEMPLATE_INDEX) $(TEMPLATE)  $(DESTDIR)
	@touch .touch

$(MAKEWEB) : RenderFormatPages.cs
	$(MCS) RenderFormatPages.cs
$(DESTDIR)/styles.css: $(SOURCEDIR)/template/styles.css
	cp $< $@
clean:
	rm -f $(DESTDIR)/*.html *.exe
	

.PHONY: clean
