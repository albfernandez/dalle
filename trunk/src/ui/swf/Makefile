BASEPATH=../../../
include $(BASEPATH)/rules.make

SUBDIRS=dalle-swf

all:
	@list='$(SUBDIRS)'; for d in $$list ; do \
	    (cd $$d && $(MAKE)) || exit 1 ; \
	done
install: all
	@list='$(SUBDIRS)'; for d in $$list ; do \
	    (cd $$d && $(MAKE) install) || exit 1 ; \
	done

uninstall: 
	@list='$(SUBDIRS)'; for d in $$list ; do \
	    (cd $$d && $(MAKE) uninstall) || exit 1 ; \
	done
clean:
	@list='$(SUBDIRS)'; for d in $$list ; do \
	    (cd $$d && $(MAKE) clean) || exit 1 ; \
	done


.PHONY: uninstall clean
