import { library } from '@fortawesome/fontawesome-svg-core'
import {
	faBullseye,
	faChevronRight,
	faSlidersH,
	faCrosshairs,
	faEdit,
	faToilet,
	faBath,
	faUtensils,
	faSink,
	faWheelchair,
	faCampground,
	faWarehouse,
	faBed,
	faMapSigns,
	faComment,
	faFirstAid,
	faHandsHelping,
	faHandPaper,
	faBabyCarriage,
	faWifi,
	faPlug,
	faParking
} from '@fortawesome/free-solid-svg-icons'

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
		faCrosshairs,
		faEdit,
		faToilet,
		faBath,
		faUtensils,
		faSink,
		faWheelchair,
		faCampground,
		faWarehouse,
		faBed,
		faMapSigns,
		faComment,
		faFirstAid,
		faHandsHelping,
		faHandPaper,
		faBabyCarriage,
		faWifi,
		faPlug,
		faParking,
	)
}
