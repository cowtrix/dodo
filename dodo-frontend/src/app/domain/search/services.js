export const filterByEvent = ({ searchResults, events }) =>
	searchResults
		.map(result =>
			events.find(event => event === result.metadata.type) ? result : null
		)
		.filter(x => x)

const createDate = date => {
	const dateResult = new Date(date)
	return dateResult.setDate(dateResult.getDate())
}

export const filterByWithinDate = ({
	searchResults,
	withinStartDate,
	withinEndDate
}) =>
	searchResults
		.map(result => {
			if (result.startDate) {
				const startDate = createDate(result.startDate)
				return startDate > withinStartDate && startDate < withinEndDate
					? result
					: null
			}
			return result
		})
		.filter(x => x)
