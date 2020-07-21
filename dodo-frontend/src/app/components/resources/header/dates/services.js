import i18n from 'i18next';

export const dateFormatted = (date, options) => {
	const thisDate = new Date(date)
	return thisDate.toLocaleDateString(i18n.language, options)
}

export const timeFormatted = (date, options) => {
	const thisDate = new Date(date)
	return thisDate.toLocaleTimeString(i18n.language, options)
}
