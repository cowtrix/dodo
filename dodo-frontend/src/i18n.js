import i18n from "i18next"
import LanguageDetector from 'i18next-browser-languagedetector';
import { initReactI18next } from "react-i18next"

import resources from "./locales/translations.json"

i18n
	.use(LanguageDetector)
	.use(initReactI18next) // passes i18n down to react-i18next
	.init({
		resources,
		// lng: "en",
		fallbackLng: 'en',
		keySeparator: false, // we do not use keys in form messages.welcome

		interpolation: {
			escapeValue: false // react already safes from xss
		},
		detection: { // Browser language detection API options here: https://github.com/i18next/i18next-browser-languageDetector#detector-options
			// order and from where user language should be detected
			order: ['navigator', 'querystring', 'cookie', 'localStorage', 'sessionStorage', 'htmlTag', 'path', 'subdomain'],

			// keys or params to lookup language from
			lookupQuerystring: 'lng',
			lookupCookie: 'i18next',
			lookupLocalStorage: 'i18nextLng',
			lookupFromPathIndex: 0,
			lookupFromSubdomainIndex: 0,

			// cache user language on
			caches: ['localStorage', 'cookie'],
			excludeCacheFor: ['cimode'], // languages to not persist (cookie, localStorage)

			// optional expire and domain for set cookie
			cookieMinutes: 525600, // 1Y
			cookieDomain: window.location.hostname,

			// optional htmlTag with lang attribute, the default is:
			htmlTag: document.documentElement,

			// optional set cookie options, reference:[MDN Set-Cookie docs](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie)
			cookieOptions: {path:'/'}
		}
	})

export default i18n
