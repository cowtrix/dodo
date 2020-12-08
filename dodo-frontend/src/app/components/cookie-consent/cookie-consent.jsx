import React from 'react'
import CookieConsent from "react-cookie-consent"

export const xrGreen = "#22A73D"

const overWriteStyles = {
	alignItems: 'center',
	background: xrGreen,
	zIndex: '9999'
}


export const CookieConsenter = (props) =>
	<CookieConsent {...props} style={overWriteStyles} >
		This website uses cookies to enhance the user experience. Please see our <a href={props.privacyPolicyHref} target="_blank" rel="noopener noreferrer">privacy policy</a> for more information.
	</CookieConsent>

