export const filterByEvent = (searchResults, events) =>
	searchResults
		.map(result =>
			events.find(event => event === result.metadata.type) ? result : null
		)
		.filter(x => x)
