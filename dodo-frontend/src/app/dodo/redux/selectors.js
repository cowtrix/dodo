import { path } from 'ramda'

export const centerMap = (state) =>
	path(['app', 'centerMap'], state)


export const menuOpen = (state) =>
	path(['app', 'menuOpen'], state)
