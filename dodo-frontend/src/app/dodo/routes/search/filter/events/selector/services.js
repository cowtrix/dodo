export const formatEvents = events =>
	events.map(event => ({
		value: event,
		label: event
	}));
