
export const dateFormatted = (date, options) => {
	const thisDate = new Date(date)
	return thisDate.toLocaleDateString(undefined, options)
}
