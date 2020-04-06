import { path } from 'ramda'

export const searchResults = (state) =>
	path(['domain', 'searchResults'], state)
