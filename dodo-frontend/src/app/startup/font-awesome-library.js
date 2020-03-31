import { library } from '@fortawesome/fontawesome-svg-core'
import { faFacebook, faTwitter, faInstagram, faYoutube } from '@fortawesome/free-brands-svg-icons'


export const initiateFontAwesomeLibrary = () => {
	library.add(
		faTwitter,
		faInstagram,
		faFacebook,
		faYoutube
	)
}