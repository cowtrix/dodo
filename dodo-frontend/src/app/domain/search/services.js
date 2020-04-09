export const filterByEvent = (searchResults, events) =>
	searchResults
		.map(result =>
			events.find(event => event === result.METADATA.TYPE) ? result : null
		)
		.filter(x => x)
