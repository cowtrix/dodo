import { library } from '@fortawesome/fontawesome-svg-core'
import { faBullseye, faChevronRight, faSlidersH, faCrosshairs } from '@fortawesome/free-solid-svg-icons'

import { faFacebook, faTwitter, faInstagram, faYoutube } from '@fortawesome/free-brands-svg-icons'

export const initiateFontAwesomeLibrary = () => {
	library.add(
		faTwitter,
		faInstagram,
		faFacebook,
		faYoutube,
		faBullseye,
		faChevronRight,
		faSlidersH,
		faCrosshairs
	)
}