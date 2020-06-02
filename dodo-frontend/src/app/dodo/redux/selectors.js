import { path } from 'ramda'

export const centerMap = (state) =>
	path(['app', 'centerMap'], state)
