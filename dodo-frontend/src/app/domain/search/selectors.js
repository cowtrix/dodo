import { path } from "ramda"

export const searchResults = state =>
	path(["domain", "search", "searchResults"], state)

export const searchResultsFiltered = state =>
	path(["domain", "search", "searchResultsFiltered"], state)

export const eventsFiltered = state =>
	path(["domain", "search", "events"], state)

export const distance = state => path(["domain", "search", "distance"], state)

export const latlong = state => path(["domain", "search", "latlong"], state)
