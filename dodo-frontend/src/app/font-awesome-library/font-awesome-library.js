import { library } from '@fortawesome/fontawesome-svg-core';
import { faFacebook, faInstagram, faTwitter, faYoutube } from '@fortawesome/free-brands-svg-icons';
import {
  faBullseye,
  faChevronDown,
  faChevronRight,
  faChevronUp,
  faCrosshairs,
  faEdit,
  faSlidersH,
} from '@fortawesome/free-solid-svg-icons';

export const initiateFontAwesomeLibrary = () => {
	library.add(
		faTwitter,
		faInstagram,
		faFacebook,
		faYoutube,
		faBullseye,
		faChevronRight,
		faChevronDown,
		faChevronUp,
		faSlidersH,
		faCrosshairs,
		faEdit
	)
}