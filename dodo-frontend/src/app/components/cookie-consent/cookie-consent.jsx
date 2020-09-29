import React from 'react'
import CookieConsent from "react-cookie-consent"

export const xrGreen = "#22A73D"

const overWriteStyles = {
	background: xrGreen,
	zIndex: '9999'
}


export const CookieConsenter = (props) =>
	<CookieConsent {...props} style={overWriteStyles} >
		This website uses cookies to enhance the user experience.
	</CookieConsent>

